import bcrypt from 'bcryptjs';
import jwt from 'jsonwebtoken';
import { config } from '../config';
import { UserModel } from '../models/userModel';
import { AuthPayload, LoginInput, RegisterInput } from '../types/authTypes';

export class AuthService {
  async register(payload: RegisterInput) {
    const existing = await UserModel.findByEmail(payload.email);
    if (existing) {
      throw new Error('Email already registered');
    }

    const passwordHash = await bcrypt.hash(payload.password, 12);
    const user = await UserModel.create({
      email: payload.email,
      passwordHash,
      username: payload.username,
      createdAt: new Date(),
    });

    return { id: user.id, email: user.email, username: user.username };
  }

  async login(payload: LoginInput) {
    const user = await UserModel.findByEmail(payload.email);
    if (!user) {
      throw new Error('Invalid credentials');
    }

    const isValid = await bcrypt.compare(payload.password, user.passwordHash);
    if (!isValid) {
      throw new Error('Invalid credentials');
    }

    const token = jwt.sign({ id: user.id, email: user.email } as AuthPayload, config.jwtSecret, {
      expiresIn: config.jwtExpiresIn,
    });

    return token;
  }
}

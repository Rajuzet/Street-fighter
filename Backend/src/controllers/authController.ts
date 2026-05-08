import { Request, Response } from 'express';
import { AuthService } from '../services/authService';

const authService = new AuthService();

export class AuthController {
  static async register(req: Request, res: Response) {
    const user = await authService.register(req.body);
    res.status(201).json({ user });
  }

  static async login(req: Request, res: Response) {
    const token = await authService.login(req.body);
    res.json({ token });
  }
}

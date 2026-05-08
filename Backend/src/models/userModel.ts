import { Pool } from 'pg';
import { pool } from '../db/connection';

export interface UserRecord {
  id: string;
  email: string;
  username: string;
  passwordHash: string;
  createdAt: Date;
}

export class UserModel {
  static async create(user: Omit<UserRecord, 'id'>) {
    const query = `INSERT INTO users (email, username, password_hash, created_at)
      VALUES ($1, $2, $3, $4)
      RETURNING id, email, username, created_at`;
    const values = [user.email, user.username, user.passwordHash, user.createdAt];
    const result = await pool.query(query, values);
    return result.rows[0];
  }

  static async findByEmail(email: string) {
    const query = `SELECT id, email, username, password_hash FROM users WHERE email = $1`;
    const result = await pool.query(query, [email]);
    return result.rows[0] && {
      id: result.rows[0].id,
      email: result.rows[0].email,
      username: result.rows[0].username,
      passwordHash: result.rows[0].password_hash,
    };
  }

  static async findById(id: string) {
    const query = `SELECT id, email, username, created_at FROM users WHERE id = $1`;
    const result = await pool.query(query, [id]);
    return result.rows[0];
  }
}

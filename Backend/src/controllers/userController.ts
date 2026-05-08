import { Request, Response } from 'express';
import { UserService } from '../services/userService';

const userService = new UserService();

export class UserController {
  static async getProfile(req: Request, res: Response) {
    const id = req.user?.id as string;
    const profile = await userService.getUserById(id);
    res.json({ profile });
  }
}

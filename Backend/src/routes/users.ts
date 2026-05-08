import { Router } from 'express';
import { UserController } from '../controllers/userController';
import { authMiddleware } from '../middleware/authMiddleware';

export const userRouter = Router();

userRouter.get('/me', authMiddleware, UserController.getProfile);

import { Router } from 'express';
import { AuthController } from '../controllers/authController';
import { validateAuthPayload } from '../middleware/validateAuthPayload';

export const authRouter = Router();

authRouter.post('/register', validateAuthPayload, AuthController.register);
authRouter.post('/login', validateAuthPayload, AuthController.login);

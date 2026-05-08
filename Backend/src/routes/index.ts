import { Router } from 'express';
import { authRouter } from './auth';
import { userRouter } from './users';
import { lobbyRouter } from './lobby';

export const apiRouter = Router();

apiRouter.use('/auth', authRouter);
apiRouter.use('/users', userRouter);
apiRouter.use('/lobby', lobbyRouter);

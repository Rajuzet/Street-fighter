import { Router } from 'express';

export const lobbyRouter = Router();

lobbyRouter.get('/', (_req, res) => {
  res.json({ message: 'Lobby service is online' });
});

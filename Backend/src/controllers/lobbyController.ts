import { Request, Response } from 'express';

export class LobbyController {
  static async getLobbyStatus(_req: Request, res: Response) {
    res.json({ status: 'ready', cause: 'lobby-service' });
  }
}

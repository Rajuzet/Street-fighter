import { NextFunction, Request, Response } from 'express';
import { z } from 'zod';

const authPayloadSchema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
  username: z.string().min(3).optional(),
});

export function validateAuthPayload(req: Request, res: Response, next: NextFunction) {
  const result = authPayloadSchema.safeParse(req.body);
  if (!result.success) {
    return res.status(400).json({ errors: result.error.flatten() });
  }

  next();
}

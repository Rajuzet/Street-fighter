import express from 'express';
import cors from 'cors';
import helmet from 'helmet';
import 'express-async-errors';
import { json } from 'body-parser';
import { apiRouter } from './routes';
import { errorHandler } from './middleware/errorHandler';
import { healthRouter } from './routes/health';
import { initializeDatabase } from './db/connection';

initializeDatabase();

const app = express();
app.use(helmet());
app.use(cors({ origin: true, credentials: true }));
app.use(json());
app.use(`/api/${process.env.API_VERSION || 'v1'}`, apiRouter);
app.use('/health', healthRouter);
app.use(errorHandler);

export default app;

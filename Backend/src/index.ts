import app from './app';
import { createServer } from 'http';
import { initializeSocketServer } from './sockets/socketServer';
import { logger } from './utils/logger';

const port = process.env.PORT || 4000;
const server = createServer(app);

initializeSocketServer(server);

server.listen(port, () => {
  logger.info(`Street-Fighter backend started on port ${port}`);
});

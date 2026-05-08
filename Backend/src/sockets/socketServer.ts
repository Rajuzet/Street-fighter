import { Server as HttpServer } from 'http';
import { Server, Socket } from 'socket.io';

export function initializeSocketServer(httpServer: HttpServer) {
  const io = new Server(httpServer, {
    cors: {
      origin: '*',
      methods: ['GET', 'POST'],
    },
  });

  io.on('connection', (socket: Socket) => {
    console.log(`Socket connected: ${socket.id}`);

    socket.on('lobby:join', (payload) => {
      socket.join(`lobby:${payload.lobbyId}`);
      socket.emit('lobby:joined', { lobbyId: payload.lobbyId });
    });

    socket.on('match:sync', (data) => {
      socket.to(`match:${data.matchId}`).emit('match:update', data);
    });

    socket.on('disconnect', () => {
      console.log(`Socket disconnected: ${socket.id}`);
    });
  });
}

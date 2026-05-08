import { UserModel } from '../models/userModel';

export class UserService {
  async getUserById(id: string) {
    const user = await UserModel.findById(id);
    if (!user) {
      throw new Error('User not found');
    }
    return { id: user.id, email: user.email, username: user.username, createdAt: user.created_at };
  }
}

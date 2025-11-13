export interface User {
  userId: number;
  username: string;
  role: 'Admin' | 'Voter';
}

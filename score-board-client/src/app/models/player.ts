import {Role} from './role';
import {Settings} from './settings';

export class Player {
  id: string;
  playerName: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  password: string;
  roleName: string;
  role: Role;
  mustChangePassword?: boolean;
  isActive?: boolean;
  lastLogin?: Date;
  registrationDate?: Date;
  activationDate?: Date;
  image: string;
  token?: string;
  settings?: Settings;
}

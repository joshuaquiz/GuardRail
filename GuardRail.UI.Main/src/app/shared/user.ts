import { UserType } from "./user-type";

export interface IUser {
  Guid: string;

  AccountGuid: string;

  FirstName: string;

  LastName: string;

  Phone: string;

  Email: string;

  PasswordResetDate: string;

  UserType: UserType;
}

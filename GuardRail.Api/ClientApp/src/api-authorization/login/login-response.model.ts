import { IUser } from '../authorize.service';

export class LoginResponse implements IUser {
  constructor(
    public readonly Succeeded: boolean,
    public readonly Email: string | null,
    public readonly Name: string | null,
    public readonly Token: string | null,
    public readonly NeedsPasswordReset: boolean) {
  }
}

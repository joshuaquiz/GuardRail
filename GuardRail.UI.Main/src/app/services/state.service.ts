import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private authToken: string | null = null;
  private accountId: string | null = null;

  public IsLoggedIn(): boolean {
    return this.authToken !== null;
  }

  public SetAuthToken(
    token: string): void {
    this.authToken = token;
  }

  public SetAccountId(
    accontId: string): void {
    this.accountId = accontId;
  }

  public GetAccountId(): string {
    if (this.accountId === null) {
      throw new Error('No account ID.');
    } else {
      return this.accountId;
    }
  }
}

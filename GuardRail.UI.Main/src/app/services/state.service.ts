import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';

export const LOCAL_STORAGE = new InjectionToken<Storage>(
  'Browser Local Storage',
  {
    providedIn: 'root',
    factory: () => localStorage,
  });

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private readonly AUTH_TOKEN_KEY = 'auth_token';
  private readonly ACCOUNT_ID_KEY = 'account_id';

  private authToken: string | null = null;
  private accountId: string | null = null;

  constructor(@Inject(LOCAL_STORAGE) private localStorage: Storage) {
    this.authToken = this.localStorage.getItem(this.AUTH_TOKEN_KEY);
    this.accountId = this.localStorage.getItem(this.ACCOUNT_ID_KEY);
  }

  public IsLoggedIn(): boolean {
    return this.authToken !== null;
  }

  public SetAuthToken(
    token: string): void {
    this.authToken = token;
    localStorage.setItem(this.AUTH_TOKEN_KEY, token);
  }

  public SetAccountId(
    accountId: string): void {
    this.accountId = accountId;
    localStorage.setItem(this.ACCOUNT_ID_KEY, accountId);
  }

  public GetAccountId(): string {
    if (this.accountId === null) {
      throw new Error('No account ID.');
    } else {
      return this.accountId;
    }
  }
}

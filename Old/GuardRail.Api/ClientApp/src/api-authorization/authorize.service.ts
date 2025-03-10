import { Injectable } from '@angular/core';

export enum AuthenticationResultStatus {
  Success,
  Redirect,
  Fail
}

export interface IUser {
  Token: string | null;
  Name: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  private userSubject: IUser | null = null;

  public isAuthenticated(): boolean {
    return !!this.getUser();
  }

  public getUser(): IUser | null {
    if (!!this.userSubject) {
      return this.userSubject;
    }

    const fromStorage = this.getUserFromStorage();
    if (!!fromStorage) {
      return fromStorage;
    }

    return null;
  }

  public getUserFromStorage(): IUser | null {
    const value = sessionStorage.getItem('user');
    let data: IUser | null = null;
    if (value) {
      data = JSON.parse(value);
    }

    return data;
  }

  public getAccessToken(): string | null {
    const user = this.getUser();
    if (!!user) {
      return user.Token;
    }

    return null;
  }

  public setUser(user: IUser): void {
    this.userSubject = user;
    sessionStorage.setItem('user', JSON.stringify(user));
  }

  public clearCredentials(): void {
    this.userSubject = null;
    sessionStorage.removeItem('user');
  }
}

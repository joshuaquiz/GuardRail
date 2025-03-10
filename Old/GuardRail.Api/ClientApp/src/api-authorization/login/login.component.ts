import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AuthorizeService } from '../authorize.service';
import { ApplicationPaths } from '../api-authorization.constants';
import { Login } from './login.model';
import { LoginResponse } from './login-response.model';

// The main responsibility of this component is to handle the user's login process.
// This is the starting point for the login process. Any component that needs to authenticate
// a user can simply perform a redirect to this component with a returnUrl query parameter and
// let the component perform the login and return back to the return url.
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  public LoginForm: FormGroup;
  public Loading = false;
  public Submitted = false;
  public Message: string | null = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authorizeService: AuthorizeService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly http: HttpClient) {
    this.LoginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  public async ngOnInit() {
    if (this.authorizeService.isAuthenticated()) {
      this.navigateToReturnUrl(this.getReturnUrl());
    }
  }

  public get f() { return this.LoginForm.controls; }

  public async login() {
    this.Submitted = true;

    // stop here if form is invalid
    if (this.LoginForm.invalid) {
      return;
    }

    this.Loading = true;
    this.loginPost(
        this.f.username.value,
        this.f.password.value)
      .subscribe(
        async (response: LoginResponse) => {
          if (response && response.Token && response.Succeeded) {
            this.authorizeService.setUser(response);
            await this.navigateToReturnUrl(this.getReturnUrl());
          } else if (!response.Succeeded && response.NeedsPasswordReset) {
            await this.redirectToResetPassword();
            this.Loading = false;
          } else {
            this.Message = 'Username or password is incorrect';
            this.Loading = false;
          }
        });
  }

  private loginPost(username: string, password: string): Observable<LoginResponse> {
    return this.http.post(
      '/api/login',
      new Login(
        username,
        password))
      .pipe(
        catchError(this.handleError),
        map((x: any) => new LoginResponse(
          x.succeeded,
          x.email,
          x.name,
          x.token,
          x.needsPasswordReset)));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, body was: `, error.error);
    }

    this.Message = error.status + ': ' + error.statusText;
    this.Loading = false;
    // Return an observable with a user-facing error message.
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }

  private navigateToReturnUrl(returnUrl: string): Promise<boolean> {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    return this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }

  public redirectToResetPassword(): Promise<boolean> {
    return this.navigateToReturnUrl(ApplicationPaths.ResetPassword + '?ReturnUrl=' + this.getReturnUrl());
  }

  private getReturnUrl(): string {
    const fromQuery = (this.activatedRoute.snapshot.queryParams as INavigationState).ReturnUrl;
    // If the url is coming from the query string, check that is either
    // a relative url or an absolute url
    if (fromQuery
      && !(fromQuery.startsWith(`${window.location.origin}/`)
        || /\/[^\/].*/.test(fromQuery))) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }

    return fromQuery || ApplicationPaths.DefaultLoginRedirectPath;
  }
}

interface INavigationState {
  ReturnUrl: string;
}

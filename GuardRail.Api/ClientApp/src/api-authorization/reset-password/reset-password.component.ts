import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthorizeService } from '../authorize.service';
import { ApplicationPaths } from '../api-authorization.constants';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  public PasswordResetForm: FormGroup;
  public Loading = false;
  public SendingNewCode = false;
  public Submitted = false;
  public Message: string | null = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authorizeService: AuthorizeService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router,
    private readonly http: HttpClient) {
    this.PasswordResetForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      resetCode: ['', Validators.required],
      newPassword: ['', Validators.required],
      newPasswordConfirm: ['', Validators.required]
    });
  }

  public async ngOnInit() {
    if (this.authorizeService.isAuthenticated()) {
      this.navigateToReturnUrl(this.getReturnUrl());
    }
  }

  public get f() { return this.PasswordResetForm.controls; }

  public async resetPassword(): Promise<void> {
    this.Submitted = true;

    // stop here if form is invalid
    if (this.PasswordResetForm.invalid) {
      return;
    }

    this.Loading = true;
    this.http.post<any>(`/api/reset-password`,
        {
          username: this.f.username.value,
          password: this.f.password.value
        })
      .subscribe(
        async user => {
          if (user && user.token) {
            this.authorizeService.setUser(user);
            await this.navigateToReturnUrl(this.getReturnUrl());
          } else {
            this.Message = 'Username or password is incorrect';
            this.Loading = false;
          }
        },
        error => {
          this.Message = error;
          this.Loading = false;
        });
  }

  public async sendNewCode(): Promise<void> { }

  private async navigateToReturnUrl(returnUrl: string): Promise<void> {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
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

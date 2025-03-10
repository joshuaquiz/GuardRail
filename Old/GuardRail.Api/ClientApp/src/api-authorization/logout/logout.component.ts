import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../authorize.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LogoutActions, ApplicationPaths } from '../api-authorization.constants';

// The main responsibility of this component is to handle the user's logout process.
// This is the starting point for the logout process, which is usually initiated when a
// user clicks on the logout button on the LoginMenu component.
@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {
  public Message: string | null = null;

  constructor(
    private readonly authorizeService: AuthorizeService,
    private readonly activatedRoute: ActivatedRoute,
    private readonly router: Router) { }

  public async ngOnInit() {
    const action = this.activatedRoute.snapshot.url[1];
    switch (action.path) {
      case LogoutActions.Logout:
        if (!!window.history.state.local) {
          await this.logout(this.getReturnUrl());
        } else {
          // This prevents regular links to <app>/authentication/logout from triggering a logout
          this.Message = 'The logout was not initiated from within the page.';
        }

        break;
      case LogoutActions.LogoutCallback:
        await this.processLogoutCallback();
        break;
      case LogoutActions.LoggedOut:
        this.Message = 'You successfully logged out!';
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }

  private async logout(returnUrl: string): Promise<void> {
    const isAuthenticated = this.authorizeService.isAuthenticated();
    if (isAuthenticated) {
      await this.authorizeService.clearCredentials();
      await this.navigateToReturnUrl(returnUrl);
    } else {
      this.Message = 'You successfully logged out!';
    }
  }

  private async processLogoutCallback(): Promise<void> {
    const url = window.location.href;
    await this.authorizeService.clearCredentials();
    await this.navigateToReturnUrl(this.getReturnUrl());
  }

  private async navigateToReturnUrl(returnUrl: string) {
    await this.router.navigateByUrl(returnUrl, {
      replaceUrl: true
    });
  }

  private getReturnUrl(state?: INavigationState): string {
    const fromQuery = (this.activatedRoute.snapshot.queryParams as INavigationState).ReturnUrl;
    // If the url is coming from the query string, check that is either
    // a relative url or an absolute url
    if (fromQuery &&
      !(fromQuery.startsWith(`${window.location.origin}/`) ||
        /\/[^\/].*/.test(fromQuery))) {
      // This is an extra check to prevent open redirects.
      throw new Error('Invalid return url. The return url needs to have the same origin as the current page.');
    }
    return (state && state.ReturnUrl) ||
      fromQuery ||
      ApplicationPaths.LoggedOut;
  }
}

interface INavigationState {
  ReturnUrl: string;
}

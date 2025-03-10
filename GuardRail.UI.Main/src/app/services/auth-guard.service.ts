import { Injectable, inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { StateService } from './state.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  private readonly router: Router = inject(Router);
  private readonly stateService: StateService = inject(StateService);

  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean | Promise<boolean> {

    if (this.stateService.IsLoggedIn()) {
      return true; // User is logged in, allow access
    } else {
      this.router.navigate(['/Login']); // Redirect to login page
      return false;
    }
  }
}

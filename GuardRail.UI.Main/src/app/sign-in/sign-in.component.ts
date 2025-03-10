import { Component, inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { StateService } from '../services/state.service';
import { HttpClientService } from '../services/http-client.service';
import { DialogButton } from '../shared/dialog/dialog-button';

@Component({
  selector: 'app-sign-in',
  standalone: false,
  
  templateUrl: './sign-in.component.html',
  styleUrl: './sign-in.component.css'
})
export class SignInComponent {
  private readonly router: Router = inject(Router);
  private readonly stateService: StateService = inject(StateService);
  private readonly httpClientService: HttpClientService = inject(HttpClientService);

  public email: string | null = null;
  public password: string | null = null;
  public readonly Buttons: DialogButton[] = [
    new DialogButton(
      "Sign In",
      this.signIn)
  ];

  public signIn(): void {
    this.httpClientService.Post<string>(
      `/User/SignIn?email=${this.email}`,
      {
        password: this.password
      })
      .subscribe(
        {
          next: data => {
            this.stateService.SetAuthToken(
              data);
            this.router.navigate(
              [
                '/Home'
              ]);
          },
          error: error =>
            alert(error)
        });
  }
}

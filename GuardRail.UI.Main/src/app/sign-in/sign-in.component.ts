import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Validators, ValidatorFn } from '@angular/forms';
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

  public Email: string | null = null;
  public Password: string | null = null;
  public readonly Buttons: DialogButton[] = [
    new DialogButton(
      "Sign In",
      () => {
        this.httpClientService.Post<string>(
          `/User/SignIn?email=${this.Email}`,
          {
            password: this.Password
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
            })
      })
  ];

  emailValidator: ValidatorFn = Validators.email;
  requiredValidator: ValidatorFn = Validators.required;
  minLengthValidator(length: number): ValidatorFn {
    return Validators.minLength(length);
  }
}

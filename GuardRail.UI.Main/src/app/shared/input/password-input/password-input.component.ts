import { Component, Input } from '@angular/core';

@Component({
  selector: 'PasswordInput',
  standalone: false,
  
  templateUrl: './password-input.component.html',
  styleUrl: './password-input.component.css'
})
export class PasswordInputComponent {
  @Input()
  public Label: string;

  @Input()
  public Value: string;
}

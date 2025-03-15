import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ValidatorFn } from '@angular/forms';

@Component({
  selector: 'PasswordInput',
  standalone: false,

  templateUrl: './password-input.component.html',
  styleUrl: './password-input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PasswordInputComponent),
      multi: true
    }
  ]
})
export class PasswordInputComponent {
  @Input() Label: string | null = '';
  @Input() value: string | null = '';
  @Input() validators: ValidatorFn[] = [];
  @Output() valueChange = new EventEmitter<string | null>();
}

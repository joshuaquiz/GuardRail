import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ValidatorFn } from '@angular/forms';

@Component({
  selector: 'TextInput',
  standalone: false,
  
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextInputComponent),
      multi: true
    }
  ]
})
export class TextInputComponent {
  @Input() Label: string | null = '';
  @Input() value: string | null = '';
  @Input() validators: ValidatorFn[] = [];
  @Output() valueChange = new EventEmitter<string | null>();
}

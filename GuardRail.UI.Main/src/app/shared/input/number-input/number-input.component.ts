import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR, ValidatorFn } from '@angular/forms';

@Component({
  selector: 'NumberInput',
  standalone: false,
  
  templateUrl: './number-input.component.html',
  styleUrl: './number-input.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => NumberInputComponent),
      multi: true
    }
  ]
})
export class NumberInputComponent {
  @Input() Label: string | null = '';
  @Input() value: string | null = '';
  @Input() validators: ValidatorFn[] = [];
  @Output() valueChange = new EventEmitter<string | null>();
}

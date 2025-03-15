import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ValidatorFn, ValidationErrors } from '@angular/forms';

@Component({
  selector: 'InputBase',
  standalone: false,
  
  templateUrl: './input-base.component.html',
  styleUrl: './input-base.component.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputBaseComponent),
      multi: true
    },
  ]
})
export class InputBaseComponent implements ControlValueAccessor {
  @Input() Label: string | null = '';
  @Input() value: string | null = '';
  @Input() type: string = 'text';
  @Input() validators: ValidatorFn[] = [];

  @Output() valueChange = new EventEmitter<string | null>();

  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(value: any): void {
    this.value = value || '';
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  onInputChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target) {
      this.value = target.value;
      this.onChange(this.value);
      this.valueChange.emit(this.value);
    }
  }

  hasErrors(): boolean {
    if (!this.validators || this.validators.length === 0) {
      return false;
    }
    const errors = this.getValidationErrors();
    return errors !== null;
  }

  getErrors(): string[] {
    const errors = this.getValidationErrors();
    if (!errors) {
      return [];
    }
    return Object.values(errors);
  }

  private getValidationErrors(): ValidationErrors | null {
    if (!this.validators || this.validators.length === 0) {
      return null;
    }
    for (const validator of this.validators) {
      const error = validator({ value: this.value } as any);
      if (error) {
        return error;
      }
    }
    return null;
  }
}

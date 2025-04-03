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
  @Input()
  public Label: string | null = '';

  @Input()
  public InputValue: string | null = '';

  @Input()
  public Type: string | null = '';

  @Input()
  public validators: ValidatorFn[] = [];

  @Output()
  public InputValueChange = new EventEmitter<string | null>();

  private isDirty: boolean = false;

  onChange: any = () => { };
  onTouched: any = () => { };

  writeValue(value: any): void {
    this.InputValue = value || '';
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
      this.InputValue = target.value;
      this.onChange(this.InputValue);
      this.InputValueChange.emit(this.InputValue);
      this.isDirty = true;
    }
  }

  hasErrors(): boolean {
    if (!this.validators || this.validators.length === 0) {
      return false;
    }
    const errors = this.getValidationErrors();
    return errors !== null && errors.length > 0;
  }

  getErrors(): string[] {
    const errors = this.getValidationErrors();
    if (!errors || errors.length === 0) {
      return [];
    }

    return this.getErrorMessage(errors);
  }

  private getValidationErrors(): ValidationErrors[] | null {
    if (!this.isDirty || !this.validators || this.validators.length === 0) {
      return null;
    }
    const errors: ValidationErrors[] = [];
    for (const validator of this.validators) {
      const error = validator({ value: this.InputValue } as any);
      if (error) {
        errors.push(error);
      }
    }

    return errors;
  }

  private getErrorMessage(errors: ValidationErrors[]): string[] {
    const errorMessages: string[] = [];
    for (const error of errors) {
      if (error['required']) {
        errorMessages.push('You must enter a value.');
      }
      if (error['email']) {
        errorMessages.push('The value must be in an email.');
      }
      if (error['minlength']) {
        errorMessages.push(`Minimum length is ${error['minlength'].requiredLength}`);
      }
    }

    return errorMessages;
  }
}

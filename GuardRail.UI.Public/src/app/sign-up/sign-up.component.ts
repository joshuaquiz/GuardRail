import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-sign-up',
  standalone: false,
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {
  signupForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient) {
    this.signupForm = this.fb.group({
      organizationName: ['', Validators.required],
      contactEmail: ['', [Validators.required, Validators.email]],
      contactPhone: ['', Validators.required],
      contactFirstName: ['', Validators.required],
      contactLastName: ['', Validators.required],
      // Payment information placeholder. In a real application, you'd integrate with a payment gateway.
      paymentInfo: this.fb.group({
        // Example placeholders; adjust based on your payment gateway requirements.
        cardNumber: [''],
        expiryDate: [''],
        cvv: ['']
      })
    });
  }

  onSubmit() {
    if (this.signupForm.valid) {
      const formData = this.signupForm.value;
      const createAccountRequest = {
        AccountName: formData.organizationName,
        FirstName: formData.contactFirstName,
        LastName: formData.contactLastName,
        Phone: formData.contactPhone,
        Email: formData.contactEmail
      };
      this.http.post<any>(`${environment.baseUrl}/account/CreateAccount`, createAccountRequest).subscribe({
        next: (response) => {
          console.log('Signup successful:', response);
          // Handle successful signup (e.g., redirect, display success message)
        },
        error: (error) => {
          console.error('Signup failed:', error);
          // Handle signup error (e.g., display error message)
        }
      });
    } else {
      this.markFormGroupTouched(this.signupForm);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpClientService {

  private http = inject(HttpClient);

  private handleError(
    error: HttpErrorResponse): Observable<never> {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred.
      // Handle it accordingly.
      console.error(
        'An error occurred:',
        error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, body was: ${error.error}`);
    }

    // Return an observable with a user-facing error message.
    return throwError(
      () =>
        new Error(
          'Something bad happened; please try again later.'));
  }

  Get<T>(url: string): Observable<T> {
    return new Observable<T>();
  }

  Post<T>(url: string, body: any): Observable<T> {
    return new Observable<T>();
  }

  Put<T>(url: string, body: any): Observable<T> {
    return new Observable<T>();
  }

  Patch<T>(url: string, body: any): Observable<T> {
    return new Observable<T>();
  }

  Delete<T>(url: string): Observable<T> {
    return new Observable<T>();
  }
}

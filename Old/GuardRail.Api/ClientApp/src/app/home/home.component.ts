import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HomePageLog } from './home-page-log.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public Message = '';
  public Loading = false;
  public Logs = new Array<HomePageLog>();

  constructor(
    private readonly http: HttpClient) {
  }

  public ngOnInit(): void {
    this.http.get('/api/logs/latest')
      .pipe(catchError(this.handleError))
      .subscribe(
        (data: any): void => {
          data.forEach((x: any) =>
            this.Logs.push(
              new HomePageLog(
                x.id,
                x.dateTime,
                x.logMessage)));
        });
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, body was: `, error.error);
    }

    this.Message = error.status + ': ' + error.statusText;
    this.Loading = false;
    // Return an observable with a user-facing error message.
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }
}

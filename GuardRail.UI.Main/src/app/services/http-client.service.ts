import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { IDashboardDataResponse } from '../shared/dashboard-data-response';
import { ILocation } from '../shared/location';
import { IAccessPoint } from '../shared/access-point';
import { AccessPointType } from '../shared/access-point-type';

@Injectable({
  providedIn: 'root'
})
export class HttpClientService {
  private mockData: { [method: string]: { [urlPattern: string]: (data: any | null) => any } } = {
    'POST': {
      '^/User/SignIn\\?email=.*$': (password: string | null): string => {
        return 'auth-token';
      }
    },
    'GET': {
      '^/Account/GetDashboardData$': (): IDashboardDataResponse => {
        return {
          Accounts: [
            {
              Guid: 'ac1',
              Name: 'Account 1',
              IsActive: true
            },
            {
              Guid: 'ac2',
              Name: 'Account 2',
              IsActive: true
            }
          ]
        };
      },
      '^/Location/ListLocations\\?accountId=ac1$': (): ILocation[] => {
        return [
          {
            Guid: 'l1',
            AccountId: 'ac1',
            Name: 'Location 1',
            Description: 'Description 1',
            Latitude: 1.0,
            Longitude: 1.0,
            GeoFenceDistance: 1.0,
            IsMobile: false
          },
          {
            Guid: 'l2',
            AccountId: 'ac1',
            Name: 'Location 2',
            Description: 'Description 2',
            Latitude: 1.0,
            Longitude: 1.0,
            GeoFenceDistance: 1.0,
            IsMobile: false
          }
        ];
      },
      '^/Location/ListLocations\\?accountId=ac2$': (): ILocation[] => {
        return [
          {
            Guid: 'l3',
            AccountId: 'ac2',
            Name: 'Location 3',
            Description: 'Description 3',
            Latitude: 1.0,
            Longitude: 1.0,
            GeoFenceDistance: 1.0,
            IsMobile: false
          },
          {
            Guid: 'l4',
            AccountId: 'ac2',
            Name: 'Location 4',
            Description: 'Description 4',
            Latitude: 1.0,
            Longitude: 1.0,
            GeoFenceDistance: 1.0,
            IsMobile: false
          }
        ];
      },
      '^/AccessPoint/ListAccessPoints\\?locationId=l1$': (): IAccessPoint[] => {
        return [
          {
            Guid: 'ap1',
            Name: 'Main Office Front Door',
            LocationGuid: 'l1',
            AccessPointType: AccessPointType.GuardRailCustom,
            Latitude: null,
            Longitude: null,
            GeoFenceDistance: .5,
            RequiresAllAccessMethods: true,
            AccessMethodTimeout: null,
            IsLocked: true,
            IsOpen: false
          },
          {
            Guid: 'ap2',
            Name: 'Main Office Back Door',
            LocationGuid: 'l1',
            AccessPointType: AccessPointType.GuardRailCustom,
            Latitude: null,
            Longitude: null,
            GeoFenceDistance: .5,
            RequiresAllAccessMethods: true,
            AccessMethodTimeout: null,
            IsLocked: true,
            IsOpen: false
          }
        ];
      },
      '^/AccessPoint/ListAccessPoints\\?locationId=l2$': (): IAccessPoint[] => {
        return [
          {
            Guid: 'ap3',
            Name: 'Lab Front Door',
            LocationGuid: 'l2',
            AccessPointType: AccessPointType.GuardRailCustom,
            Latitude: null,
            Longitude: null,
            GeoFenceDistance: .5,
            RequiresAllAccessMethods: true,
            AccessMethodTimeout: null,
            IsLocked: true,
            IsOpen: false
          },
          {
            Guid: 'ap4',
            Name: 'Lab Side Door',
            LocationGuid: 'l2',
            AccessPointType: AccessPointType.GuardRailCustom,
            Latitude: null,
            Longitude: null,
            GeoFenceDistance: .5,
            RequiresAllAccessMethods: true,
            AccessMethodTimeout: null,
            IsLocked: true,
            IsOpen: false
          },
          {
            Guid: 'ap5',
            Name: 'Lab Back Door',
            LocationGuid: 'l2',
            AccessPointType: AccessPointType.GuardRailCustom,
            Latitude: null,
            Longitude: null,
            GeoFenceDistance: .5,
            RequiresAllAccessMethods: true,
            AccessMethodTimeout: null,
            IsLocked: true,
            IsOpen: false
          }
        ];
      }
    }
  };

  private getMockData<T>(method: string, url: string, data: any | null = null): T {
    for (const urlPattern in this.mockData[method]) {
      const regex = new RegExp(urlPattern);
      if (regex.test(url)) {
        const result = this.mockData[method][urlPattern](data) as T;
        console.log(method + ' ' + url, result);
        return result;
      }
    }

    throw new Error(
      'No URL match ' + method + ' ' + url);
  }

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
    const mockResponse = this.getMockData<T>('GET', url);
    return of(mockResponse);
  }

  Post<T>(url: string, body: any): Observable<T> {
    const mockResponse = this.getMockData<T>('POST', url, body);
    return of(mockResponse);
  }

  Put<T>(url: string, body: any): Observable<T> {
    const mockResponse = this.getMockData<T>('PUT', url, body);
    return of(mockResponse);
  }

  Patch<T>(url: string, body: any): Observable<T> {
    const mockResponse = this.getMockData<T>('PATCH', url, body);
    return of(mockResponse);
  }

  Delete<T>(url: string): Observable<T> {
    const mockResponse = this.getMockData<T>('DELETE', url);
    return of(mockResponse);
  }
}

import { Component, inject, OnInit } from '@angular/core';
import { IAccount } from '../shared/account';
import { ILocation } from '../shared/location';
import { HttpClientService } from '../services/http-client.service';
import { IDashboardDataResponse } from '../shared/dashboard-data-response';

@Component({
  selector: 'home',
  standalone: false,
  
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  private readonly httpClient: HttpClientService = inject(HttpClientService);

  public Accounts: IAccount[] = [];
  public SelectedAccount: IAccount = {
    Guid: '',
    Name: '',
    IsActive: true
  };
  public Locations: ILocation[] = [];
  public SelectedLocation: ILocation = {
    Guid: '',
    AccountId: '',
    Name: '',
    Description: null,
    Latitude: null,
    Longitude: null,
    GeoFenceDistance: null,
    IsMobile: false
  };
  public SingleAccount: boolean = true;
  public ShowAccessPoints: boolean = false;
  public ShowLocations: boolean = false;
  public ShowUsers: boolean = false;

  public ngOnInit(): void {
    this.LoadDashboardData();
  }

  private LoadDashboardData(): void {
    this.httpClient.Get<IDashboardDataResponse>(
      '/Account/GetDashboardData')
      .subscribe(
        {
          next: d => this.handleDashboardData(d),
          error: error => alert(error)
        });
  }

  public LoadCurrentAccount(): void {
    this.httpClient.Get<ILocation[]>(
      `/Location/ListLocations?accountId=${this.SelectedAccount.Guid}`)
      .subscribe(
        {
          next: d => this.handleAccountLoad(d),
          error: error =>
            alert(error)
        });
  }

  private handleDashboardData(data: IDashboardDataResponse): void {
    this.Accounts = data.Accounts;
    this.SingleAccount = this.Accounts.length === 0;
    this.SelectedAccount = this.Accounts[0];
    this.LoadCurrentAccount();
  }

  private handleAccountLoad(data: ILocation[]): void {
    this.Locations = data;
    this.SelectedLocation = this.Locations.length > 0 ? this.Locations[0] : this.SelectedLocation;
  }
}

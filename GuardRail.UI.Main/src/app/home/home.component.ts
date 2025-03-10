import { Component, inject, OnInit } from '@angular/core';
import { IAccount } from '../shared/account';
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
  public SingleAccount: boolean = true;
  public MultipleAccounts: boolean = false;

  public ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    this.httpClient.Get<IDashboardDataResponse>(
      '/Account/GetDashboardData')
      .subscribe(
        {
          next: this.handleDashboardData,
          error: error =>
            alert(error)
        });
  }

  public LoadAccount(): void {
    this.httpClient.Get<IAccount[]>(
      `/Account/accountId=${this.SelectedAccount.Guid}`)
      .subscribe(
        {
          next: this.handleAccountLoad,
          error: error =>
            alert(error)
        });
  }

  private handleDashboardData(data: IDashboardDataResponse): void {
    this.Accounts = data.Accounts;
    this.SingleAccount = this.Accounts.length === 0;
    this.MultipleAccounts = this.Accounts.length > 1;
    this.SelectedAccount = this.Accounts[0];
    this.LoadAccount();
  }

  private handleAccountLoad(data: IAccount[]): void {
    this.Accounts = data;
  }
}

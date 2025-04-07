import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { IAccount } from '../shared/account';
import { HttpClientService } from '../services/http-client.service';
import { StateService } from '../services/state.service';
import { IDashboardDataResponse } from '../shared/dashboard-data-response';

@Component({
  selector: 'home',
  standalone: false,
  
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit, OnDestroy {
  private readonly httpClient: HttpClientService = inject(HttpClientService);
  private readonly stateService: StateService = inject(StateService);
  private readonly router: Router = inject(Router);

  private routerSubscription: Subscription | undefined;
  public Accounts: IAccount[] = [];
  public SelectedAccount: IAccount = {
    Guid: '',
    Name: '',
    IsActive: true
  };
  public SingleAccount: boolean = true;
  public ShowLocations: boolean = false;
  public ShowAccessPoints: boolean = false;
  public ShowUsers: boolean = false;

  public ngOnInit(): void {
    const initialUrl = this.router.url;
    this.handleRouteChange(initialUrl);
    this.routerSubscription = this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: any) => this.handleRouteChange(event.url));
    this.LoadDashboardData();
  }

  ngOnDestroy() {
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  private handleRouteChange(url: string): void {
    if (url === '/Home/Locations') {
      this.ShowAccessPoints = false;
      this.ShowUsers = false;
      this.ShowLocations = true;
    } else if (url === '/Home/AccessPoints') {
      this.ShowLocations = false;
      this.ShowUsers = false;
      this.ShowAccessPoints = true;
    } else if (url === '/Home/Users') {
      this.ShowLocations = false;
      this.ShowAccessPoints = false;
      this.ShowUsers = true;
    }
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

  public AccountSelectionChanged(e: Event): void {
    const selectElement = e.target as HTMLSelectElement;
    if (selectElement) {
      this.AccountChanged(this.Accounts[selectElement.selectedIndex]);
    }
  }

  public AccountChanged(account: IAccount): void {
    this.SelectedAccount = account;
    this.stateService.SetAccountId(this.SelectedAccount.Guid);
  }

  private handleDashboardData(data: IDashboardDataResponse): void {
    this.Accounts = data.Accounts;
    this.SingleAccount = this.Accounts.length === 0;
    this.AccountChanged(this.Accounts[0]);
  }
}

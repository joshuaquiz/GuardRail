import { Component, OnInit, inject } from '@angular/core';
import { StateService } from '../../services/state.service';
import { HttpClientService } from '../../services/http-client.service';
import { IUser } from '../../shared/user';

@Component({
  selector: 'users',
  standalone: false,
  
  templateUrl: './users.component.html',
  styleUrls: [
    './users.component.css',
    '../home.list.css'
  ]
})
export class UsersComponent implements OnInit {
  public SidebarOpen: boolean = false;
  public AddItem: boolean = false;
  public EditingSingle: boolean = false;
  public Deleting: boolean = false;
  public SingleItemSelected: boolean = false;
  public MultipleItemsSelected: boolean = false;

  private readonly stateService: StateService = inject(StateService);
  private readonly httpClientService: HttpClientService = inject(HttpClientService);

  public Users: IUser[] = [];

  public ngOnInit(): void {
    this.ListUsers();
  }

  public CloseSideBar(): void {
    this.AddItem = false;
    this.EditingSingle = false;
    this.Deleting = false;
    this.SidebarOpen = false;
  }

  public StartAdd(): void {
    this.EditingSingle = false;
    this.Deleting = false;
    this.AddItem = true;
    this.SidebarOpen = true;
  }

  public StartEdit(): void {
    this.AddItem = false;
    this.Deleting = false;
    this.EditingSingle = true;
    this.SidebarOpen = true;
  }

  public StartDelete(): void {
    this.AddItem = false;
    this.EditingSingle = false;
    this.Deleting = true;
    this.SidebarOpen = true;
  }

  public CreateUser(): void {
    this.httpClientService.Post(
      `/User/CreateLocation`,
      null)
      .subscribe(
        {
          next: this.ListUsers,
          error: error =>
            alert(error)
        });
  }

  public EditUser(): void {
    this.httpClientService.Post(
      `/User/EditUser`,
      null)
      .subscribe(
        {
          next: this.ListUsers,
          error: error =>
            alert(error)
        });
  }

  public DeleteUser(): void {
    this.httpClientService.Post(
      `/User/DeleteUser`,
      null)
      .subscribe(
        {
          next: this.ListUsers,
          error: error =>
            alert(error)
        });
  }

  public ListUsers(): void {
    this.httpClientService.Get<IUser[]>(
      `/User/ListUsers?accountId=${this.stateService.GetAccountId()}`)
      .subscribe(
        {
          next: data =>
            this.Users = data,
          error: error =>
            alert(error)
        });
  }
}

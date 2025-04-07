import { Component, OnInit, inject } from '@angular/core';
import { StateService } from '../../services/state.service';
import { HttpClientService } from '../../services/http-client.service';
import { CreateLocationRequest } from '../../shared/create-location-request';
import { ILocation } from '../../shared/location';

@Component({
  selector: 'locations',
  standalone: false,
  
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.css'
})
export class LocationsComponent implements OnInit {
  public SidebarOpen: boolean = false;
  public AddItem: boolean = false;
  public EditingSingle: boolean = false;
  public Deleting: boolean = false;
  public SingleItemSelected: boolean = false;
  public MultipleItemsSelected: boolean = false;

  private readonly stateService: StateService = inject(StateService);
  private readonly httpClientService: HttpClientService = inject(HttpClientService);

  public Locations: ILocation[] = [];

  public ngOnInit(): void {
    this.stateService.GetAccountId()
    this.ListLocations();
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

  public CreateLocation(): void {
    this.httpClientService.Post(
      `/Location/CreateLocation`,
      new CreateLocationRequest(
        this.stateService.GetAccountId(),
        '',
        null,
        null,
        null,
        null,
        false))
      .subscribe(
        {
          next: this.ListLocations,
          error: error =>
            alert(error)
        });
  }

  public EditLocation(): void {
    this.httpClientService.Post(
      `/Location/EditLocation`,
      new CreateLocationRequest(
        this.stateService.GetAccountId(),
        '',
        null,
        null,
        null,
        null,
        false))
      .subscribe(
        {
          next: this.ListLocations,
          error: error =>
            alert(error)
        });
  }

  public DeleteLocation(): void {
    this.httpClientService.Post(
      `/Location/DeleteLocation`,
      new CreateLocationRequest(
        this.stateService.GetAccountId(),
        '',
        null,
        null,
        null,
        null,
        false))
      .subscribe(
        {
          next: this.ListLocations,
          error: error =>
            alert(error)
        });
  }

  public ListLocations(): void {
    this.httpClientService.Get<ILocation[]>(
      `/Location/ListLocations?accountId=${this.stateService.GetAccountId()}`)
      .subscribe(
        {
          next: data =>
            this.Locations = data,
          error: error =>
            alert(error)
        });
  }
}

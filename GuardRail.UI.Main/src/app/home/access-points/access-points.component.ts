import { Component, OnInit, inject } from '@angular/core';
import { StateService } from '../../services/state.service';
import { HttpClientService } from '../../services/http-client.service';
import { ILocation } from '../../shared/location';
import { IAccessPoint } from '../../shared/access-point';
import { AccessPointType } from '../../shared/access-point-type';

@Component({
  selector: 'access-points',
  standalone: false,
  
  templateUrl: './access-points.component.html',
  styleUrls: [
    './access-points.component.css',
    '../home.list.css'
  ]
})
export class AccessPointsComponent implements OnInit {
  public SidebarOpen: boolean = false;
  public AddItem: boolean = false;
  public EditingSingle: boolean = false;
  public Deleting: boolean = false;
  public SingleItemSelected: boolean = false;
  public MultipleItemsSelected: boolean = false;

  private readonly stateService: StateService = inject(StateService);
  private readonly httpClientService: HttpClientService = inject(HttpClientService);

  public Locations: ILocation[] = [];
  public SelectedLocation: ILocation = {
    Guid: '',
    Name: '',
    AccountId: '',
    Description: null,
    Latitude: null,
    Longitude: null,
    GeoFenceDistance: null,
    IsMobile: false
  };

  public AccessPoints: IAccessPoint[] = [];
  public SelectedAccessPoint: IAccessPoint = {
    Guid: '',
    Name: '',
    LocationGuid: '',
    AccessPointType: AccessPointType.UnKnown,
    Latitude: null,
    Longitude: null,
    GeoFenceDistance: null,
    RequiresAllAccessMethods: false,
    AccessMethodTimeout: null,
    IsLocked: false,
    IsOpen: false
  };

  public ngOnInit(): void {
    this.ListLocations();
  }

  public LocationSelectionChanged(e: Event): void {
    const selectElement = e.target as HTMLSelectElement;
    if (selectElement) {
      this.LocationChanged(this.Locations[selectElement.selectedIndex]);
    }
  }

  public LocationChanged(location: ILocation): void {
    this.SelectedLocation = location;
    this.ListAccessPoints();
  }

  public AccessPointChanged(accessPoint: IAccessPoint): void {
    this.SelectedAccessPoint = accessPoint;
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

  public ListLocations(): void {
    this.httpClientService.Get<ILocation[]>(
      `/Location/ListLocations?accountId=${this.stateService.GetAccountId()}`)
      .subscribe(
        {
          next: data => {
            this.Locations = data;
            this.LocationChanged(this.Locations[0]);
          },
          error: error =>
            alert(error)
        });
  }

  public ListAccessPoints(): void {
    this.httpClientService.Get<IAccessPoint[]>(
      `/AccessPoint/ListAccessPoints?locationId=${this.SelectedLocation.Guid}`)
      .subscribe(
        {
          next: data => {
            this.AccessPoints = data;
          },
          error: error =>
            alert(error)
        });
  }

  public CreateAccessPoint(): void {

  }

  public EditAccessPoint(): void {

  }

  public DeleteAccessPoint(): void {

  }
}

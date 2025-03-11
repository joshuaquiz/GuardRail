import { Component, inject } from '@angular/core';
import { DialogButton } from '../../shared/dialog/dialog-button';
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
export class LocationsComponent {
  public AddItem: boolean = false;
  public EditingSingle: boolean = false;
  public Deleting: boolean = false;
  public SingleItemSelected: boolean = false;
  public MultipleItemsSelected: boolean = false;

  private readonly stateService: StateService = inject(StateService);
  private readonly httpClientService: HttpClientService = inject(HttpClientService);

  public readonly CreateLocationRequest: CreateLocationRequest =
    new CreateLocationRequest(
      this.stateService.GetAccountId(),
      '',
      null,
      null,
      null,
      null,
      false);
  public Locations: ILocation[] = [];

  public StartAdd(): void {
    this.AddItem = true;
  }

  public EndAdd(): void {
    this.AddItem = false;
  }

  public StartEdit(): void {
    this.EditingSingle = true;
  }

  public EndEdit(): void {
    this.EditingSingle = false;
  }

  public StartDelete(): void {
    this.Deleting = true;
  }

  public EndDelete(): void {
    this.Deleting = false;
  }

  public readonly AddButtons: DialogButton[] = [
    new DialogButton(
      "Add Location",
      this.CreateLocation)
  ];

  public readonly EditSingleButtons: DialogButton[] = [
    new DialogButton(
      "Edit Location",
      this.EditLocation)
  ];

  public readonly DeleteButtons: DialogButton[] = [
    new DialogButton(
      "Delete Location",
      this.DeleteLocation)
  ];

  public CreateLocation(): void {
    this.httpClientService.Post(
      `/Location/CreateLocation`,
      this.CreateLocationRequest)
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
      this.CreateLocationRequest)
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
      this.CreateLocationRequest)
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

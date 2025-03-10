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
  public SingleItemSelected: boolean = false;
  public MultipleItemsSelected: boolean = false;

  public readonly Buttons: DialogButton[] = [
    new DialogButton(
      "Add Location",
      this.CreateLocation)
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

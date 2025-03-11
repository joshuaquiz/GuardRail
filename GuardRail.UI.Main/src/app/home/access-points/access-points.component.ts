import { Component } from '@angular/core';
import { ILocation } from '../../shared/location';

@Component({
  selector: 'access-points',
  standalone: false,
  
  templateUrl: './access-points.component.html',
  styleUrl: './access-points.component.css'
})
export class AccessPointsComponent {
  public AddItem: boolean = false;
  public EditingSingle: boolean = false;
  public Deleting: boolean = false;
  public SingleItemSelected: boolean = false;
  public MultipleItemsSelected: boolean = false;
  public Locations: ILocation[] = [];

  public StartAdd(): void {
    this.AddItem = true;
  }

  public EndAdd(): void {
    this.AddItem = false;
  }

  public StartEdit(): void {
    this.AddItem = true;
  }

  public StartDelete(): void {
    this.Deleting = true;
  }
}

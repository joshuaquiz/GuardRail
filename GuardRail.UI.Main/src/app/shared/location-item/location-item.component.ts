import { Component, Input } from '@angular/core';
import { ILocation } from '../location';

@Component({
  selector: 'location-item',
  standalone: false,
  
  templateUrl: './location-item.component.html',
  styleUrl: './location-item.component.css'
})
export class LocationItemComponent {
  @Input()
  public Location: ILocation;
}

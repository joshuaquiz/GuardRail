import { Component } from '@angular/core';

@Component({
  selector: 'nav-bar',
  standalone: false,
  
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {
  public ShowAccessPoints: boolean = false;
  public ShowLocations: boolean = false;
  public ShowUsers: boolean = false;
}

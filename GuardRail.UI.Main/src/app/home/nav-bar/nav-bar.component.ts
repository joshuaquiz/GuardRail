import { Component, Input } from '@angular/core';

@Component({
  selector: 'NavBar',
  standalone: false,
  
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {
  @Input()
  public ShowAccessPoints!: boolean;

  @Input()
  public ShowLocations!: boolean;

  @Input()
  public ShowUsers!: boolean;

  constructor() {
    console.log('NavBar Component Loaded');
  }
}

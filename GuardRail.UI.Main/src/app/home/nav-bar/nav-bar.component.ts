import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'NavBar',
  standalone: false,
  
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.css'
})
export class NavBarComponent {
  constructor(private router: Router) { }
}

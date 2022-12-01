import { Component } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  public IsExpanded = false;

  public collapse() {
    this.IsExpanded = false;
  }

  public toggle() {
    this.IsExpanded = !this.IsExpanded;
  }
}

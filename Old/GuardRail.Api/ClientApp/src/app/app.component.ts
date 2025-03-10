import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../api-authorization/authorize.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public Title = 'GuardRail';
  public IsLoggedIn = false;

  constructor(private readonly authorizeService: AuthorizeService) {
  }

  public ngOnInit() {
    this.IsLoggedIn = this.authorizeService.isAuthenticated();
  }
}

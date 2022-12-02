import { Component, OnInit } from '@angular/core';
import { AuthorizeService } from '../authorize.service';

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.css']
})
export class LoginMenuComponent implements OnInit {
  public IsAuthenticated: boolean;
  public Name: string | null | undefined;

  constructor(private readonly authorizeService: AuthorizeService) {
    this.IsAuthenticated = false;
    this.Name = null;
  }

  public ngOnInit() {
    this.IsAuthenticated = this.authorizeService.isAuthenticated();
    this.Name = this.authorizeService.getUser()?.Name;
  }
}

import { Component, OnInit } from '@angular/core';
import { AuthorizeService, IUser } from '../authorize.service';

@Component({
  selector: 'app-login-menu',
  templateUrl: './login-menu.component.html',
  styleUrls: ['./login-menu.component.css']
})
export class LoginMenuComponent implements OnInit {
  public IsAuthenticated: boolean;
  public UserName: string | null | undefined;

  constructor(private readonly authorizeService: AuthorizeService) {
    this.IsAuthenticated = false;
    this.UserName = null;
  }

  public ngOnInit() {
    this.IsAuthenticated = this.authorizeService.isAuthenticated();
    this.UserName = this.authorizeService.getUser()?.Name;
  }
}

import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { NotFoundComponent } from './not-found/not-found.component';

import { AccessPointsComponent } from './home/access-points/access-points.component';
import { LocationsComponent } from './home/locations/locations.component';
import { UsersComponent } from './home/users/users.component';
import { NavBarComponent } from './home/nav-bar/nav-bar.component';
import { DialogComponent } from './shared/dialog/dialog.component';
import { LocationItemComponent } from './shared/location-item/location-item.component';

import { InputBaseComponent } from './shared/input/input-base/input-base.component';
import { TextInputComponent } from './shared/input/text-input/text-input.component';
import { PasswordInputComponent } from './shared/input/password-input/password-input.component';
import { NumberInputComponent } from './shared/input/number-input/number-input.component';

@NgModule({
  declarations: [
    AppComponent,
    SignInComponent,
    NotFoundComponent,
    AccessPointsComponent,
    LocationsComponent,
    UsersComponent,
    NavBarComponent,
    DialogComponent,
    LocationItemComponent,
    InputBaseComponent,
    TextInputComponent,
    PasswordInputComponent,
    NumberInputComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

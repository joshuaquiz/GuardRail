import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SignInComponent } from './sign-in/sign-in.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AuthGuardService } from './services/auth-guard.service';

import { HomeComponent } from './home/home.component';
import { AccessPointsComponent } from './home/access-points/access-points.component';
import { LocationsComponent } from './home/locations/locations.component';
import { UsersComponent } from './home/users/users.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'Login',
    pathMatch: 'full'
  },
  {
    path: 'Login',
    component: SignInComponent
  },
  {
    path: 'Home',
    canActivate: [AuthGuardService],
    component: HomeComponent,
    children: [
      {
        path: '',
        redirectTo: 'Locations',
        pathMatch: 'full'
      },
      {
        path: 'Locations',
        component: LocationsComponent
      },
      {
        path: 'AccessPoints',
        component: AccessPointsComponent
      },
      {
        path: 'Users',
        component: UsersComponent
      }
    ]
  },
  {
    path: '**',
    component: NotFoundComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

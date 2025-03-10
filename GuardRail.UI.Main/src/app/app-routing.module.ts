import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SignInComponent } from './sign-in/sign-in.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AuthGuardService } from './services/auth-guard.service';

import { AccessPointsComponent } from './home/access-points/access-points.component';
import { LocationsComponent } from './home/locations/locations.component';
import { UsersComponent } from './home/users/users.component';

const routes: Routes = [
  {
    path: 'Login',
    component: SignInComponent
  },
  {
    path: 'Home',
    canActivate: [AuthGuardService],
    children: [
      {
        path: '',
        redirectTo: 'Locations',
        pathMatch: 'full'
      },
      {
        path: 'AccessPointes',
        component: AccessPointsComponent
      },
      {
        path: 'Locations',
        component: LocationsComponent
      },
      {
        path: 'Users',
        component: UsersComponent
      }
    ]
  },
  {
    path: '**',
    component: NotFoundComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

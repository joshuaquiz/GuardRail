import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AboutComponent } from './about/about.component';
import { PricingComponent } from './pricing/pricing.component';
import { SignUpComponent } from './sign-up/sign-up.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { title: 'Home', path: 'home', pathMatch: 'full', component: HomeComponent },
  { title: 'About', path: 'about', pathMatch: 'full', component: AboutComponent },
  { title: 'Pricing', path: 'pricing', pathMatch: 'full', component: PricingComponent },
  { title: 'Sign Up', path: 'sign-up', pathMatch: 'full', component: SignUpComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

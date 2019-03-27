import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NotfoundComponent } from './shared/components/notfound/notfound.component';
import { LoginComponent } from './shared/components/login/login.component';
import { RegisterComponent } from './shared/components/register/register.component';

const routes: Routes = [
  { path: 'home', loadChildren: './modules/home/home.module#HomeModule' },                // Home Module Routing
  { path: 'account', loadChildren: './modules/account/account.module#AccountModule' },    // Account module routing
  { path: 'login', component: LoginComponent },                                           // Login component
  { path: 'register', component: RegisterComponent },                                     // Register component
  { path: 'notfound', component: NotfoundComponent },                                     // 404 Not Found Page Routing
  { path: '**', component: NotfoundComponent } 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

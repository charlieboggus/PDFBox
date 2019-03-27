import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NotfoundComponent } from './shared/components/notfound/notfound.component';

const routes: Routes = [
  { path: 'home', loadChildren: './modules/home/home.module#HomeModule' },      // Home Module Routing
  { path: 'notfound', component: NotfoundComponent },                           // 404 Not Found Page Routing
  { path: '**', component: NotfoundComponent } 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

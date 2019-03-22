import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotfoundComponent } from './shared/components/notfound/notfound.component';

const routes: Routes = [
  { path: 'home',  loadChildren: './modules/home/home.module#HomeModule' },               // Home Module routing
  { path: 'account', loadChildren: './modules/account/account.module#AccountModule' },    // Account Module routing
  { path: 'notfound', component: NotfoundComponent },                                     // 404 Not Found page routing
  { path: '**', component: NotfoundComponent }                                            // Any route not specified above will go to the 404 Not Found page
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

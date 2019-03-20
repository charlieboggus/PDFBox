import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './components/home/home.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { FaqComponent } from './components/faq/faq.component';
import { ContactComponent } from './components/contact/contact.component';

const routes: Routes = [
  { path: '',  component: HomeComponent },            // Home page routing
  { path: 'home', component: HomeComponent },         // Home page routing
  { path: 'privacy', component: PrivacyComponent },   // Privacy page routing
  { path: 'faq', component: FaqComponent },           // FAQ page routing
  { path: 'contact', component: ContactComponent }    // Contact page routing
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }

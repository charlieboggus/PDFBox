import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IndexComponent } from './components/index/index.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { FaqComponent } from './components/faq/faq.component';
import { ContactComponent } from './components/contact/contact.component';
import { ConvertComponent } from './components/convert/convert.component';
import { AboutusComponent } from './components/aboutus/aboutus.component';

const routes: Routes = [
  { path: '',  component: IndexComponent },             // Home page routing
  { path: 'home', component: IndexComponent },          // Home page routing
  { path: 'index', component: IndexComponent },         // Home page routing
  { path: 'privacy', component: PrivacyComponent },     // Privacy page routing
  { path: 'faq', component: FaqComponent },             // FAQ page routing
  { path: 'contact', component: ContactComponent },     // Contact page routing
  { path: 'convert', component: ConvertComponent },      // Convert page routing
  { path: 'aboutus', component: AboutusComponent },      // Convert page routing
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }

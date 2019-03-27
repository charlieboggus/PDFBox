import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Routing
import { HomeRoutingModule } from './home-routing.module';

// Components
import { IndexComponent } from './components/index/index.component';
import { ContactComponent } from './components/contact/contact.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { FaqComponent } from './components/faq/faq.component';

// Services
import { ContactService } from './services/contact.service';

@NgModule({
  declarations: [
    IndexComponent, 
    ContactComponent, 
    PrivacyComponent, 
    FaqComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HomeRoutingModule
  ],
  providers: [
    ContactService
  ]
})
export class HomeModule { }

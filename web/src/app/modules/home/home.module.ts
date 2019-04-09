import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileUploadModule } from 'ng2-file-upload';

// Routing
import { HomeRoutingModule } from './home-routing.module';

// Components
import { IndexComponent } from './components/index/index.component';
import { ContactComponent } from './components/contact/contact.component';
import { PrivacyComponent } from './components/privacy/privacy.component';
import { FaqComponent } from './components/faq/faq.component';
import { AboutusComponent} from './components/aboutus/aboutus.component';

// Services
import { ContactService } from './services/contact.service';
import { ConvertComponent } from './components/convert/convert.component';

@NgModule({
  declarations: [
    IndexComponent, 
    ContactComponent, 
    PrivacyComponent, 
    FaqComponent,
    ConvertComponent,
    AboutusComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    FileUploadModule,
    HomeRoutingModule
  ],
  providers: [
    ContactService
  ]
})
export class HomeModule { }

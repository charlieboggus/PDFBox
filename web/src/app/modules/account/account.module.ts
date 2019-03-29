import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { FileUploadModule } from 'ng2-file-upload';

// Routing
import { AccountRoutingModule } from './account-routing.module';

// Components
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DocumentsComponent } from './components/documents/documents.component';
import { UploadComponent } from './components/upload/upload.component';

@NgModule({
  declarations: [
    DashboardComponent, 
    DocumentsComponent, 
    UploadComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    FileUploadModule,
    AccountRoutingModule
  ]
})
export class AccountModule { }

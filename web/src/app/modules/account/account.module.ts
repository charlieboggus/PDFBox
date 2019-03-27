import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// Routing
import { AccountRoutingModule } from './account-routing.module';

// Components
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DocumentsComponent } from './components/documents/documents.component';

@NgModule({
  declarations: [
    DashboardComponent, 
    DocumentsComponent
  ],
  imports: [
    CommonModule,
    AccountRoutingModule
  ]
})
export class AccountModule { }

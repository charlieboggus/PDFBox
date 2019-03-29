import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';

import { AuthenticationGuard } from '../../shared/guards/auth.guard';
import { DocumentsComponent } from './components/documents/documents.component';
import { UploadComponent } from './components/upload/upload.component';

const routes: Routes = [
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthenticationGuard] },
  { path: 'documents', component: DocumentsComponent, canActivate: [AuthenticationGuard] },
  { path: 'upload', component: UploadComponent, canActivate: [AuthenticationGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }

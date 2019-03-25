import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';

import { User } from 'src/app/shared/models/user.model';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { AccountService } from '../../services/account.service';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  currentUser: User;
  managementForm: FormGroup;
  submitted: boolean = false;
  loading: boolean = false;
  deletePressed: boolean = false;

  constructor(private auth: AuthenticationService, private account: AccountService, private alerts: AlertService, private router: Router, private builder: FormBuilder) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit() {
    this.managementForm = this.builder.group({
      newUsername: [''],
      newEmail: [''],
      newPassword: ['']
    });
  }

  get form() { return this.managementForm.controls; }

  onFormSubmit() {
    this.submitted = true;
    this.loading = true;

    this.account.changeAccountDetails(this.currentUser.id, this.form.newUsername.value, this.form.newEmail.value, this.form.newPassword.value).subscribe(
      result => 
      {
        this.alerts.info(result['message'] + '. Please login again.', true);
        this.loading = false;
        this.auth.logout();
        this.router.navigate(['/login']);
      }, 
      error => 
      {
        this.alerts.error('Error: ' + error);
        this.submitted = false;
        this.loading = false;
      }
    );
  }

  onDeletePress() {
    this.deletePressed = true;
  }

  onDeleteConfirm() {
    this.account.deleteAccount(this.currentUser.id).subscribe(result => {
      this.alerts.info(result['message'], true);
    });
    this.auth.logout();
    this.router.navigate(['/register']);
  }

  onDeleteCancel() {
    this.deletePressed = false;
  }
}

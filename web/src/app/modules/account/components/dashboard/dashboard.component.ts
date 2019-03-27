import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { User } from '../../../../shared/models/user.model';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  currentUser: User;
  detailsForm:  FormGroup;
  delete: boolean;

  constructor(private router: Router, private builder: FormBuilder, private auth: AuthenticationService) {
    this.delete = false;
  }

  ngOnInit() {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.detailsForm = this.builder.group({
      newUsername: [''],
      newEmail: ['', Validators.email],
      newPassword: ['']
    });
  }

  get form() { return this.detailsForm.controls; }
  
  onSubmit() {
    this.auth.changeAccountDetails(this.form.newUsername.value, this.form.newEmail.value, this.form.newPassword.value);
  }

  onDeletePress() {
    this.delete = !this.delete;
  }

  onDeleteConfirm() {
    this.auth.deleteAccount();
  }

  onDeleteCancel() {
    this.delete = false;
  }
}

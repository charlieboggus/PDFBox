import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AuthenticationService } from '../../../../shared/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(private builder: FormBuilder, private route: ActivatedRoute, private router: Router, private auth: AuthenticationService, private alerts: AlertService) { }

  ngOnInit() {
    this.loginForm = this.builder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.auth.logout();
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get form() { return this.loginForm.controls; }

  onSubmit() {
    this.submitted = true;
    this.loading = true;

    if(this.loginForm.invalid) {
      this.loading = false;
      return;
    }

    // Attempt to login using values from form
    this.auth.login(this.form.username.value, this.form.password.value).pipe(first()).subscribe(result => {
      this.alerts.success(result['message']);
      this.alerts.clear();
      this.router.navigate([this.returnUrl]);
    }, error => {
      this.alerts.error(error);
      this.loading = false;
      this.submitted = false;
    });
  }
}

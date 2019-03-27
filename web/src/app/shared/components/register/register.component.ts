import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AuthenticationService } from '../../services/authentication.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(private builder: FormBuilder, private router: Router, private auth: AuthenticationService, private alerts: AlertService) { }

  ngOnInit() {
    this.registerForm = this.builder.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  get form() { return this.registerForm.controls; }

  onSubmit() {
    this.submitted = true;
    this.loading = true;
    if(this.registerForm.invalid) {
      this.loading = false;
      return;
    }

    this.auth.register(this.form.username.value, this.form.email.value, this.form.password.value).subscribe(result => {
      this.alerts.success(result['message'], true);
      this.router.navigate(['/login']);
    }, error => {
      this.alerts.error(error);
      this.submitted = false;
      this.loading = false;
    });
  }
}

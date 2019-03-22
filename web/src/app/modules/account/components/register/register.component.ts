import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;
  loading = false;
  submitted = false;
  message: string;
  error: boolean;

  constructor(private builder: FormBuilder, private route: ActivatedRoute, private router: Router, private auth: AuthenticationService) { }

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

    if(this.registerForm.invalid) {
      return;
    }

    this.loading = true;
    this.auth.register(this.form.username.value, this.form.email.value, this.form.password.value).subscribe(result => {
      this.error = false;
      this.message = result;
      this.router.navigate(['/login']);
    }, error => {
      this.error = true;
      this.message = error;
      this.submitted = false;
      this.loading = false;
    });
  }
}

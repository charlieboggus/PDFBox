import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

import { ContactService } from '../../services/contact.service';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  contactForm: FormGroup;
  formSubmitted: boolean;

  constructor(private builder: FormBuilder, private service: ContactService, private alerts: AlertService) {
    this.formSubmitted = false;
  }

  ngOnInit() {
    this.contactForm = this.builder.group({
      'name': ['', Validators.required],
      'email': ['', [Validators.required, Validators.email]],
      'message': ['', Validators.required]
    });
  }

  // Function to be called when the form is submitted
  onSubmit() {
    // Submit contact form data to API
    this.service.submitForm(JSON.stringify(this.contactForm.value)).subscribe(result => {
      this.formSubmitted = true;
      this.alerts.success(result['message'], false);
    }, error => {
      this.formSubmitted = true;
      this.alerts.error(error, false);
    });
  }

  get form() { return this.contactForm.controls; }
}

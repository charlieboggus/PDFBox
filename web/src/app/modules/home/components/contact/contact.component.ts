import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {

  // TODO: contact form functionality -- it's just for appearances right now

  contactForm: FormGroup;
  formSubmitted: boolean;

  constructor(private formBuilder: FormBuilder) {
    this.formSubmitted = false;
  }

  ngOnInit() {
    this.contactForm = this.formBuilder.group({
      'name': ['', Validators.required],
      'email': ['', Validators.required, Validators.email],
      'message': ['', Validators.required]
    });
  }

  // Function to be called when the form is submitted
  onSubmit() {
    this.formSubmitted = true;

    // If the contact form has any errors return from this function
    if(this.contactForm.invalid) {
      return;
    }
  }

  get form() { return this.contactForm.controls; }
}

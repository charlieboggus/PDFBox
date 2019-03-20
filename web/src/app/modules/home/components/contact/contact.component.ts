import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ContactService } from '../../services/contact.service';
import { ContactFormData } from '../../models/contactformdata.model';

@Component({
    selector: 'app-contact',
    templateUrl: './contact.component.html',
    styleUrls: ['./contact.component.css']
})
export class ContactComponent implements OnInit {
    
    contactForm: FormGroup;
    formSubmitted: boolean;
    success: boolean;

    constructor(private builder: FormBuilder, private service: ContactService) {
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
        this.service.submitContactForm(JSON.stringify(this.contactForm.value)).subscribe(result => {
            this.formSubmitted = true;
            this.success = result['success'];
        });
    }

    get form() { return this.contactForm.controls; }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { ContactFormData } from '../models/contactformdata.model';

@Injectable({
    providedIn: 'root'
})
export class ContactService {

    private serverUrl = 'https://localhost:5001/api/contact';

    private httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    constructor(private http: HttpClient) { }

    submitContactForm(data: string) {
        return this.http.post(this.serverUrl, data, this.httpOptions);
    }
}

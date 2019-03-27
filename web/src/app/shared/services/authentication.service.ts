import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { User } from '../models/user.model';
import { AlertService } from './alert.service';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private currentUserSubject: BehaviorSubject< User >;
  public currentUser: Observable< User >;

  constructor(private http: HttpClient, private router: Router, private alerts: AlertService) {
    this.currentUserSubject = new BehaviorSubject< User >(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }

  register(username: string, email: string, password: string) {
    return this.http.post< any >('http://localhost:5000/api/users/register', { username, email, password });
  }

  login(username: string, password: string) {
    return this.http.post< any >('http://localhost:5000/api/users/authenticate', { username, password }).pipe(map(result => {

      // Login is successful if there's a JWT token in the response
      if(result && result.token) {
        // store user details and token in local storage to keep user logged in
        localStorage.setItem('currentUser', JSON.stringify(result));
        this.currentUserSubject.next(result);
      }
      
      return result;
    }));
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  changeAccountDetails(username: string, email: string, password: string): void {
    this.http.put< any >(`http://localhost:5000/api/users/${ this.currentUserSubject.value.id }`, { username, email, password }).subscribe(result => {
      // Update the localstorage user stuff
      this.logout();
      this.alerts.success(result.message + "Please login using your new credentials.", true);
      this.router.navigate(['login']);
    }, error => {
      this.alerts.error(error.message, false);
    });
  }

  deleteAccount() {
    this.http.delete< any >(`http://localhost:5000/api/users/${ this.currentUserSubject.value.id }`)
      .toPromise()
      .then(result => {
        this.logout();
        this.alerts.success(result.message, true);
        this.router.navigate(['register']);
      }, error => {
        this.alerts.error(error);
      });
  }
}

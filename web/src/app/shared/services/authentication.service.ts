import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private currentUserSubject: BehaviorSubject< User >;
  public currentUser: Observable< User >;

  constructor(private http: HttpClient) {
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
    return this.http.post< any >('http://localhost:5000/api/users/auth', { username, password }).pipe(map(user => {

      // Login is successful if there's a JWT token in the response
      if(user && user.token) {
        // store user details and token in local storage to keep user logged in
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
      }
      
      return user;
    }));
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}

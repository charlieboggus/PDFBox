import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get< User[] >('http://localhost:5000/api/users')
  }

  getUser(id: number) {
    return this.http.get(`http://localhost:5000/api/users/${id}`);
  }

  register(user: User) {
    return this.http.post(`http://localhost:5000/api/users/register`, user);
  }

  update(user: User) {
    return this.http.put(`http://localhost:5000/api/users/${user.userId}`, user);
  }

  delete(id: number) {
    return this.http.delete(`http://localhost:5000/api/users/${id}`);
  }
}

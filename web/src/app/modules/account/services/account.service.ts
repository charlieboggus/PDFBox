import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http: HttpClient) { }

  updateAccountDetails(id: number, username: string, email: string, password: string) {
    return this.http.put< any >(`http://localhost:5000/api/users/${ id }`, { username, email, password });
  }

  deleteAccount(id: number) {
    return this.http.delete< any >(`http://localhost:5000/api/users/${ id }`);
  }
}

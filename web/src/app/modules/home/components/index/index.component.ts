import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { User } from 'src/app/shared/models/user.model';

@Component({
  selector: 'app-index',
  templateUrl: './index.component.html',
  styleUrls: ['./index.component.css']
})
export class IndexComponent implements OnInit {

  currentUser: User;

  constructor(private auth: AuthenticationService, private router: Router) {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
  }

  ngOnInit() {
  }

  redirectConvert() {
    this.router.navigate(['convert']);
  }

  redirectLogin() {
    this.router.navigate(['login']);
  }

  redirectRegister() {
    this.router.navigate(['register']);
  }

  redirectUpload() {
    this.router.navigate(['upload']);
  }

  redirectView() {
    this.router.navigate(['documents']);
  }
}

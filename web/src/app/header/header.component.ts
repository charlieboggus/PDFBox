import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {

  loggedIn: boolean;

  constructor() { }

  ngOnInit() {
    this.loggedIn = false;
  }

  ngOnDestroy() {
  }
}

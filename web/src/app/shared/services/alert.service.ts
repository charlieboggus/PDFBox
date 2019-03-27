import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';

import { Alert, AlertType } from '../models/alert.model';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private subject: Subject<Alert> = new Subject<Alert>();
  private keepAfterRouteChange: boolean = false;

  constructor(private router: Router) {
    router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        if (this.keepAfterRouteChange) {
          this.keepAfterRouteChange = false;
        } else {
          this.clear();
        }
      }
    });
  }

  getAlert(): Observable<any> {
    return this.subject.asObservable();
  }

  clear() {
    this.subject.next();
  }

  success(message: string, keepAfterRouteChange = false) {
    this.alert(AlertType.Success, message, keepAfterRouteChange);
  }

  error(message: string, keepAfterRouteChange = false) {
    this.alert(AlertType.Error, message, keepAfterRouteChange);
  }

  info(message: string, keepAfterRouteChange = false) {
    this.alert(AlertType.Info, message, keepAfterRouteChange);
  }

  warning(message: string, keepAfterRouteChange = false) {
    this.alert(AlertType.Warning, message, keepAfterRouteChange);
  }

  alert(type: AlertType, message: string, keepAfterRouteChange = false) {
    this.keepAfterRouteChange = keepAfterRouteChange;
    this.subject.next(<Alert>{ type: type, message: message });
  }
}

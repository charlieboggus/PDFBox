import { Component, OnInit } from '@angular/core';

import { Alert, AlertType } from '../../models/alert.model';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-alerts',
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css']
})
export class AlertsComponent implements OnInit {

  alerts: Alert[] = [];


  constructor(private alertService: AlertService) { }

  ngOnInit() {
    this.alertService.getAlert().subscribe((alert: Alert) => {
      if(!alert) {
        this.alerts = [];
        return;
      }

      this.alerts.push(alert);
    });
  }

  removeAlert(alert: Alert) {
    this.alerts = this.alerts.filter(x => x !== alert);
  }

  getCSS(alert: Alert) {
    if(!alert) {
      return;
    }

    switch(alert.type) {
      case AlertType.Success:
        return 'alert alert-success';
      
      case AlertType.Error:
        return 'alert alert-danger';

      case AlertType.Info:
        return 'alert alert-info';
      
      case AlertType.Warning:
        return 'alert alert-warning';
    }
  }
}

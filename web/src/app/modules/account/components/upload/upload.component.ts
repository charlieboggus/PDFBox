import { Component, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { FileUploader } from "ng2-file-upload";

import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  uploader: FileUploader = new FileUploader({ isHTML5: true });
  dropzoneHover: boolean = false;
  convertFiles: boolean = false;
  submitted: boolean = false;
  loading: boolean = false;

  constructor(private http: HttpClient, private alerts: AlertService) { }

  ngOnInit() {

  }

  onDropzoneHover(e: any): void {
    this.dropzoneHover = e;
  }

  onUploadSubmit() {
    this.submitted = true;
    this.loading = true;

    // Create form data for HTTP request
    let formData = new FormData();
    for (let i = 0; i < this.uploader.queue.length; i++) {
      let file = this.uploader.queue[i]._file;
      formData.append('file', file);
    }
    formData.append('convert', this.convertFiles.toString());

    // Post the form data to API
    this.http.post< any >('http://localhost:5000/api/documents/upload', formData).subscribe(result => {
      this.alerts.success(result.message);
      this.uploader.clearQueue();
      this.submitted = false;
      this.loading = false;
    }, error => {
      this.alerts.error(error);
      this.submitted = false;
      this.loading = false;
    });
  }
}

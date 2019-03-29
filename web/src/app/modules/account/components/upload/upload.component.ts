import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
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

  constructor(private http: HttpClient, private alerts: AlertService) { }

  ngOnInit() {

  }

  onDropzoneHover(e: any): void {
    console.log(this.convertFiles);
    this.dropzoneHover = e;
  }

  onUploadSubmit() {
    this.submitted = true;
    let formData = new FormData();

    // Populate form data
    for (let i = 0; i < this.uploader.queue.length; i++) {
      let file = this.uploader.queue[i]._file;
      formData.append('file', file);
    }
    formData.append('convert', this.convertFiles.toString());

    this.http.post< any >('http://localhost:5000/api/documents/upload', formData).subscribe(result => {
      this.alerts.success(result.message);
      this.uploader.clearQueue();
      this.submitted = false;
    }, error => {
      this.alerts.error(error);
      this.submitted = false;
    });
  }
}

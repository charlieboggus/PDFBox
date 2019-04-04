import { Component, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { FileUploader } from "ng2-file-upload";
import { saveAs } from 'file-saver';

import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-convert',
  templateUrl: './convert.component.html',
  styleUrls: ['./convert.component.css']
})
export class ConvertComponent implements OnInit {

  uploader: FileUploader = new FileUploader({ 
    isHTML5: true,
    allowedFileType: ['doc', 'docx', 'ppt', 'pptx', 'xls', 'xlsx']
  });
  dropzoneHover: boolean = false;
  submitted: boolean = false;
  loading: boolean = false;

  constructor(private http: HttpClient, private alerts: AlertService) { }

  ngOnInit() {
    this.uploader.onAfterAddingFile = f => { 
      if (this.uploader.queue.length > 1) { 
        this.uploader.removeFromQueue(this.uploader.queue[0]);
      } 
    };
  }

  onDropzoneHover(e: any): void {
    this.dropzoneHover = e;
  }

  onConvertSubmit() {
    this.submitted = true;
    this.loading = true;

    // Convert the uploaded file to form data that we can post to API
    let formData = new FormData();
    for (let i = 0; i < this.uploader.queue.length; i++)
    {
      let file = this.uploader.queue[i]._file;
      formData.append('file', file);
    }

    // Post the HTTP request
    this.http.post('http://localhost:5000/api/documents/convert', formData, { responseType: 'blob' })
    .subscribe(result => 
    {
      var blob = new Blob([result], { type: 'application/pdf' });
      saveAs(blob, 'document.pdf');
      this.alerts.success('Document successfully converted');
      this.submitted = false;
      this.loading = false;
      this.uploader.clearQueue();
    }, 
    error => 
    {
      this.alerts.error(error);
      this.submitted = false;
      this.loading = false;
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders, HttpRequest, HttpResponse } from '@angular/common/http';
import { saveAs } from 'file-saver';

import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { User } from 'src/app/shared/models/user.model';
import { DocumentModel } from 'src/app/shared/models/document.model';

@Component({
  selector: 'app-documents',
  templateUrl: './documents.component.html',
  styleUrls: ['./documents.component.css']
})
export class DocumentsComponent implements OnInit {

  currentUser: User;
  documents: DocumentModel[] = [];

  constructor(private auth: AuthenticationService, private alerts: AlertService, private http: HttpClient) { }

  ngOnInit() {
    this.auth.currentUser.subscribe(x => this.currentUser = x);
    this.onRefresh();
  }

  onRefresh() {
    this.documents = [];
    this.http.get<any>('http://localhost:5000/api/documents/details/all').subscribe(result => {
      for (let i = 0; i < result.length; i++) {
        let entry = result[i];

        let doc = new DocumentModel();
        doc.documentId = entry.documentId;
        doc.name = entry.name;
        doc.extension = entry.extension;
        doc.size = entry.size;
        doc.creationDate = entry.creationDate;

        this.documents.push(doc);
      }
    }, error => {
      this.alerts.error('Failed to fetch documents: ' + error);
    });
  }

  onDownload(doc: DocumentModel) {
    this.http.get(`http://localhost:5000/api/documents/${doc.documentId}`, { responseType: 'blob' }).subscribe(data => {
      var blob = new Blob([data], { type: this.getContentType(doc.extension) });
      saveAs(blob, doc.name);
      this.alerts.success('Successfully downloaded ' + doc.name);
    }, error => {
      this.alerts.error(error);
    });
  }

  onDelete(id: number) {
    this.http.delete<any>(`http://localhost:5000/api/documents/${id}`).subscribe(result => {
      this.alerts.success(result.message);
      this.onRefresh();
    }, error => {
      this.alerts.error('Failed to delete document: ' + error);
    });
  }

  onDeleteAll() {
    this.http.delete<any>('http://localhost:5000/api/documents/all').subscribe(result => {
      this.alerts.success(result.message);
      this.onRefresh();
    }, error => {
      this.alerts.error('Failed to delete documents: ' + error);
    });
  }

  private getContentType(ext: string): string {
    switch (ext) {
      case ".txt": return "text/plain";
      case ".csv": return "text/csv";
      case ".pdf": return "application/pdf";
      case ".doc": return "application/vnd.ms-word";
      case ".docx": return "application/vnd.ms-word";
      case ".xls": return "application/vnd.ms-excel";
      case ".xlsx": return "application/vnd.openxmlformats.officedocument.spreadsheetml.sheet";
      case ".png": return "image/png";
      case ".jpg": return "image/jpeg";
      case ".jpeg": return "image/jpeg";
      case ".gif": return "image/gif";

      default: return "text/plain";
    }
  }
}

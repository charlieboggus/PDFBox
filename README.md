# PDFBox: SP19 AMIS 3610 Group Project
- **Team Members:** Charlie Boggus, Carson Susich, Kyle Kempf, Kyrne Beauford, Sean Walston

- **Project Name:** PDFBox

- **Project Presentation:** [view](https://docs.google.com/presentation/d/1nXvlwAUlC4b8naXIDQge1ErG8gWHoYxhnVSQ5wy7md0/edit?usp=sharing) or [download](https://github.com/charlieboggus/SP19-AMIS-3610-Group-Project/blob/master/3610%20Presentation.pptx)

- **Project Write-Up:** [download](https://github.com/charlieboggus/SP19-AMIS-3610-Group-Project/blob/master/Write%20up.docx)

- **Live Demo**: [link](https://pdfbox.azurewebsites.net/)

# Project Description:
This project was launched in order to convert Microsoft Office files to PDF format.  The launch was inspired by the frustration of losing formatting when Microsoft Office documents are opened on a different computer. Our design is built around efficiency and simplicity.

# API Documentation:

API Route | Description
----------|------------
**HTTP POST:** /api/contact/submit | API method for submitting contact form data. The API then uses an SMTP client to send the contact form data as an email.
**HTTP POST:** /api/users/register | API method for registering a new user account.
**HTTP POST:** /api/users/authenticate | API method for authenticating a user account.
**HTTP GET:** /api/users/all | API method for getting details about all user accounts.
**HTTP GET:** /api/users/{ id } | API method for getting details about a particular user account.
**HTTP PUT:** /api/users/{ id } | API method to update the account details for a particular user account.
**HTTP DELETE:** /api/users/{ id } | API method to delete a particular user account
**HTTP GET:** /api/documents/details/all | API method to get the details of every stored document.
**HTTP GET:** /api/documents/details/{ id } | API method to get the details of a particular document.
**HTTP GET:** /api/documents/{ id } | API method to download a particular stored document.
**HTTP POST:** /api/documents/upload | API method to upload a document
**HTTP POST:** /api/documents/convert | API method to convert a document
**HTTP DELETE:** /api/documents/all | API method to delete all documents associated with a particular user account.
**HTTP DELETE:** /api/documents/{ id } | API method to delete a particular document

# Misc:
This project's backend was developed using [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) version 2.2

This project's web frontend was generated with [Angular CLI](https://github.com/angular/angular-cli) version 7.3.3.

This project is licensed under the MIT license. See [LICENSE](https://github.com/charlieboggus/SP19-AMIS-3610-Group-Project/blob/master/LICENSE) for detailed license text.

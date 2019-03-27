import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Application Routing
import { AppRoutingModule } from './app-routing.module';

// Modules
import { HomeModule } from './modules/home/home.module';
import { AccountModule } from './modules/account/account.module';

// Components
import { AppComponent } from './app.component';

import { AlertComponent } from './shared/components/alert/alert.component';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { NotfoundComponent } from './shared/components/notfound/notfound.component';
import { LoginComponent } from './shared/components/login/login.component';
import { RegisterComponent } from './shared/components/register/register.component';

// Services, Guards, & Interceptors
import { AlertService } from './shared/services/alert.service';
import { AuthenticationService } from './shared/services/authentication.service';
import { AuthenticationGuard } from './shared/guards/auth.guard';
import { ErrorInterceptor } from './shared/interceptors/error.interceptor';
import { JwtInterceptor } from './shared/interceptors/jwt.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    AlertComponent,
    NavbarComponent,
    FooterComponent,
    NotfoundComponent,
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    HomeModule,
    AccountModule,
    AppRoutingModule
  ],
  providers: [
    AlertService,
    AuthenticationService,
    AuthenticationGuard,
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

// Application Routing
import { AppRoutingModule } from './app-routing.module';

// Modules
import { HomeModule } from './modules/home/home.module';
import { AccountModule } from './modules/account/account.module';

// Components
import { AppComponent } from './app.component';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { NotfoundComponent } from './shared/components/notfound/notfound.component';

// Services
import { AuthenticationGuard } from './shared/guards/auth.guard';
import { AuthenticationService } from './shared/services/authentication.service';
import { ErrorInterceptor } from './shared/services/error.interceptor';
import { JwtInterceptor } from './shared/services/jwt.interceptor';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    FooterComponent,
    NotfoundComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    HomeModule,
    AccountModule,
    AppRoutingModule
  ],
  providers: [
    AuthenticationService,
    AuthenticationGuard,
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

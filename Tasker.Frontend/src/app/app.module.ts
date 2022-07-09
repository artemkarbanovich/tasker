import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { RegistrationComponent } from './core/components/account/registration/registration.component';
import { LoginComponent } from './core/components/account/login/login.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from './core/interceptors/token.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './shared/modules/material/material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NavbarComponent } from './core/components/navbar/navbar.component';
import { MatSliderModule } from '@angular/material/slider';
import { HomeComponent } from './core/components/home/home.component';
import { ConfirmationComponent } from './shared/components/confirmation/confirmation.component';
import { ObjectiveListComponent } from './core/components/objective/objective-list/objective-list.component';
import { ObjectiveEditComponent } from './core/components/objective/objective-edit/objective-edit.component';
import { ObjectiveCreateComponent } from './core/components/objective/objective-create/objective-create.component';

@NgModule({
  declarations: [
    AppComponent,
    RegistrationComponent,
    LoginComponent,
    NavbarComponent,
    HomeComponent,
    ConfirmationComponent,
    ObjectiveListComponent,
    ObjectiveEditComponent,
    ObjectiveCreateComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MaterialModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    MatSliderModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

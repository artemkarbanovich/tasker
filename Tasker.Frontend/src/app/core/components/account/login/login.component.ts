import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { LoginEmailRequest } from 'src/app/core/models/requests/login-email-request';
import { LoginUsernameRequest } from 'src/app/core/models/requests/login-username-request';
import { AccountService } from 'src/app/core/services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  public loginForm: FormGroup;
  public hidePassword: boolean = true;
  public errorType: string | null = null;
  public errorMessage: string  | null = null;

  constructor(private accountService: AccountService, private formBuilder: FormBuilder, private router: Router) { }

  public ngOnInit(): void { 
    this.initializeForm();
  }

  public login(): void {
    let emailOrUsername = this.loginForm.controls['emailOrUsername'].value;

    if(emailOrUsername.includes('@')) {
      let loginEmail: LoginEmailRequest = {
        email: emailOrUsername,
        password: this.loginForm.controls['password'].value
      };

      this.accountService.loginByEmail(loginEmail).pipe(
        catchError(response => {
          if(response.error.message.includes('email')) {
            this.errorType = 'Email';
            this.errorMessage = response.error.message;
          } else if(response.error.message.includes('password')) {
            this.errorType = 'Password';
            this.errorMessage = response.error.message;
          }
          return throwError(() => response);
        })
      ).subscribe(() => {
        this.router.navigate(['']);
      });
    } else {
      let loginUsername: LoginUsernameRequest = {
        username: emailOrUsername,
        password: this.loginForm.controls['password'].value
      };

      this.accountService.loginByUsername(loginUsername).pipe(
        catchError(response => {
          if(response.error.message.includes('username')) {
            this.errorType = 'Username';
            this.errorMessage = response.error.message;
          } else if(response.error.message.includes('password')) {
            this.errorType = 'Password';
            this.errorMessage = response.error.message;
          }
          return throwError(() => response);
        })
      ).subscribe(() => {
        this.router.navigate(['']);
      });
    }
  }

  private initializeForm(): void {
    this.loginForm = this.formBuilder.group({
      emailOrUsername: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(32)]]
    });

    this.loginForm.controls['emailOrUsername'].valueChanges.subscribe(() => {
      if(this.errorType === 'Email' || this.errorType === 'Username') {
        this.errorType = null;
        this.errorMessage = null;
      }
    });

    this.loginForm.controls['password'].valueChanges.subscribe(() => {
      if(this.errorType === 'Password') {
        this.errorType = null;
        this.errorMessage = null;
      }
    });
  }
}

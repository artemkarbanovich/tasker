import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { RegisterRequest } from 'src/app/core/models/requests/register-request';
import { AccountService } from 'src/app/core/services/account.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {
  public registrationForm: FormGroup;
  public hidePassword: boolean = true;
  public hideConfirmPassword: boolean = true;
  public errorType: string | null = null;
  public errorMessage: string  | null = null;

  constructor(private accountService: AccountService, private formBuilder: FormBuilder,
    private router: Router, private snackBar: MatSnackBar) { }

  public ngOnInit(): void {
    this.initializeForm();
  }

  public register(): void {
    let registerRequest: RegisterRequest = {
      email: this.registrationForm.controls['email'].value,
      username: this.registrationForm.controls['username'].value,
      password: this.registrationForm.controls['password'].value
    };
    
    this.accountService.register(registerRequest).pipe(
      catchError(response => {
        if(response.error.message.includes('Email')) {
          this.errorType = 'Email';
          this.errorMessage = response.error.message;
        }
        else if(response.error.message.includes('Username')) {
          this.errorType = 'Username';
          this.errorMessage = response.error.message;
        }
        return throwError(() => response);
      })
    ).subscribe(() => {
      let config = new MatSnackBarConfig();
      config.duration = 3000;
      this.snackBar.open('You successfully registered and logged in', 'Close', config);
      this.router.navigate(['']);
    });
  }

  private initializeForm(): void {
    this.registrationForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(24)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(32)]],
      confirmPassword: ['', [Validators.required, this.matchConfirmPasswordValue('password')]]
    });

    this.registrationForm.controls['email'].valueChanges.subscribe(() => {
      if(this.errorType === 'Email') {
        this.errorType = null;
        this.errorMessage = null;
      }
    });

    this.registrationForm.controls['username'].valueChanges.subscribe(() => {
      if(this.errorType === 'Username') {
        this.errorType = null;
        this.errorMessage = null;
      }
    });
  }

  private matchConfirmPasswordValue(passwordFormControl: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[passwordFormControl].value ? null : { isMatching: true };
    }
  }
}

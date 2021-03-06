import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { map, Observable, ReplaySubject } from 'rxjs';
import { Account } from '../models/account';
import { RegisterRequest } from '../models/requests/register-request';
import { LoginEmailRequest } from '../models/requests/login-email-request';
import { LoginUsernameRequest } from '../models/requests/login-username-request';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private baseUrl: string = environment.apiUrl;
  private currentUserSource: ReplaySubject<Account> = new ReplaySubject<Account>(1);
  public currentUser$: Observable<Account> = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  public register(body: RegisterRequest): Observable<void> {
    return this.http.post<Account>(this.baseUrl + 'account/register', body).pipe(
      map((user: Account) => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  public loginByEmail(body: LoginEmailRequest): Observable<void> {
    return this.http.post<Account>(this.baseUrl + 'account/login-email', body).pipe(
      map((user: Account) => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  public loginByUsername(body: LoginUsernameRequest): Observable<void> {
    return this.http.post<Account>(this.baseUrl + 'account/login-username', body).pipe(
      map((user: Account) => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  public setCurrentUser(user: Account): void {
    user.role = JSON.parse(atob(user.accessToken.split('.')[1]))['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
  
  public getCurrentUser(): Account | null {
    return JSON.parse(localStorage.getItem('user')) || null;
  }

  public deleteCurrentUser(): void {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}

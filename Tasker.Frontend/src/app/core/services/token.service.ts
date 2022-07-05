import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account';
import { RefreshTokenRequest } from '../models/requests/refresh-token-request';
import { RevokeTokenRequest } from '../models/requests/revoke-token-request';
import { RefreshTokenResponse } from '../models/responses/refresh-token-response';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient, private accountService: AccountService) { }

  public refreshToken(body: RefreshTokenRequest): Observable<void> {
    return this.http.post<RefreshTokenResponse>(this.baseUrl + 'token/refresh', body).pipe(
      map((response: RefreshTokenResponse) => {
        if(response) {
          this.updateTokens(response);
        }
      })
    );
  }

  public revokeToken(body: RevokeTokenRequest): Observable<void> {
    return this.http.post(this.baseUrl + 'token/revoke', body).pipe(
      map(() => {
        this.accountService.deleteCurrentUser();
      })
    );
  }

  private updateTokens(tokens: RefreshTokenResponse): void {
    let user: Account = JSON.parse(localStorage.getItem('user') || '');
    if(user) {
      user.accessToken = tokens.accessToken;
      user.refreshToken = tokens.refreshToken;
    }
    this.accountService.setCurrentUser(user);
  }
}

import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../services/account.service';
import { TokenService } from '../services/token.service';
import { Account } from '../models/account';
import { RefreshTokenRequest } from '../models/requests/refresh-token-request';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService, private tokenService: TokenService) { }

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let currentUser: Account;
    this.accountService.currentUser$.pipe(take(1)).subscribe((user: Account) => currentUser = user);

    if(currentUser) {
      if(currentUser.accessTokenExpiryTime < new Date()) {
        let refreshTokenRequest: RefreshTokenRequest = {
          accessToken: currentUser.accessToken,
          refreshToken: currentUser.refreshToken
        };
        
        this.tokenService.refreshToken(refreshTokenRequest).subscribe(() => {
          this.accountService.currentUser$.pipe(take(1)).subscribe((user: Account) => currentUser = user);
        });
      }

      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${ currentUser.accessToken }`
        }
      });
    }

    return next.handle(request);
  }
}

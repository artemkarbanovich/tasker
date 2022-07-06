import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpInterceptor, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';
import { AccountService } from '../services/account.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  private isRefreshing: boolean = false;
  private refreshTokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(private accountService: AccountService, private tokenService: TokenService) { }

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    request = this.addAccessTokenToHeader(request);
    return next.handle(request).pipe(catchError(error => {
      if (error instanceof HttpErrorResponse && error.status === 401 && error.error === null) {
        return this.handleRefreshTokenError(request, next);
      }
      return throwError(error);
    }));
  }
  
  private addAccessTokenToHeader(request: HttpRequest<unknown>): HttpRequest<unknown> {
    let currentUser = this.accountService.getCurrentUser();
    if (currentUser) {
      return request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.accessToken}`
        }
      });
    }
    return request;
  }

  private handleRefreshTokenError(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      let currentUser = this.accountService.getCurrentUser();

      if (currentUser) {
        return this.tokenService.refreshToken().pipe(
          switchMap(() => {
            this.isRefreshing = false;
            currentUser = this.accountService.getCurrentUser();
            this.refreshTokenSubject.next(currentUser.accessToken);
            return next.handle(this.addAccessTokenToHeader(request));
          }),
          catchError(error => {
            this.isRefreshing = false;
            this.accountService.deleteCurrentUser();
            return throwError(error);
          })
        )
      }
    }
    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(() => next.handle(this.addAccessTokenToHeader(request)))
    );
  }
}

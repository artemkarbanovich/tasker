import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate } from '@angular/router';
import { map, Observable, take, tap } from 'rxjs';
import { AccountService } from '../services/account.service';
import { Account } from '../models/account';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private accountService: AccountService) { }

  public canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    let role = route.data.role as string;
    
    return this.accountService.currentUser$.pipe(
      map((user: Account) => {
        if(user.role === role) {
          return true;
        } else {
          return false;
        }
      })
    );
  }
}

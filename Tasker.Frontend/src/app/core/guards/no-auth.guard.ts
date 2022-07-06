import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private router: Router) { }
  
  public canActivate(): boolean {
    let canActivate: boolean = this.accountService.getCurrentUser() === null;

    if(canActivate) {
      return true;
    }
    else {
      this.router.navigate(['']);
      return false;
    }
  }
}

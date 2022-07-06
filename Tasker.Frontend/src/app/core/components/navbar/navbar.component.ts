import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../../services/account.service';
import { TokenService } from '../../services/token.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  constructor(public accountService: AccountService, private tokenService: TokenService,
    private router: Router) { }
    
  public logout(): void {
    this.tokenService.revokeToken().subscribe(() => {
      this.accountService.deleteCurrentUser();
      this.router.navigate(['']);
    });
  }
}

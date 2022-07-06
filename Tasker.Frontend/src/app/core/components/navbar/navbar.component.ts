import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ConfirmationComponent } from 'src/app/shared/components/confirmation/confirmation.component';
import { AccountService } from '../../services/account.service';
import { TokenService } from '../../services/token.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {
  constructor(public accountService: AccountService, private tokenService: TokenService,
    private router: Router, private dialog: MatDialog) { }

  public logout(): void {
    this.dialog.open(ConfirmationComponent, { disableClose: true, autoFocus: false, data: { confirmationMessage: 'Confirm logout, please' } })
      .afterClosed().subscribe((result: boolean) => {
        if (!result) {
          return;
        }
        this.tokenService.revokeToken().subscribe(() => {
          this.accountService.deleteCurrentUser();
          this.router.navigate(['']);
        });
      });
  }
}

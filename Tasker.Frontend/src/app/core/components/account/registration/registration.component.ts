import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/core/services/account.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {

  constructor(private accountService: AccountService) { }

  public ngOnInit(): void {

  }

}

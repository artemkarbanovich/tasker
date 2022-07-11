import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { CreateObjectiveRequest } from 'src/app/core/models/requests/create-objective-request';
import { GetFreeApisResponse } from 'src/app/core/models/responses/get-free-apis-response';
import { FreeApiService } from 'src/app/core/services/free-api.service';
import { ObjectiveService } from 'src/app/core/services/objective.service';

@Component({
  selector: 'app-objective-create',
  templateUrl: './objective-create.component.html',
  styleUrls: ['./objective-create.component.scss']
})
export class ObjectiveCreateComponent implements OnInit {
  public freeApis: GetFreeApisResponse[] = [];
  public selectedFreeApi: GetFreeApisResponse = null;
  public objectiveCreateForm: FormGroup = null;
  public startAtDefaultTime: number[] = [new Date().getHours(), new Date().getMinutes() + 2, 0];
  public startAtMinDate: Date = new Date(new Date().getTime() + 2 * 60000);

  constructor(private objectiveService: ObjectiveService, private freeApiService: FreeApiService,
    private formBuilder: FormBuilder, private router: Router, private snackBar: MatSnackBar) { }

  public ngOnInit(): void {
    this.getFreeApis();
  }

  public createObjective(): void {
    let objective: CreateObjectiveRequest = {
      name: this.objectiveCreateForm.controls['name'].value,
      description: this.objectiveCreateForm.controls['description'].value,
      startAt: new Date(this.objectiveCreateForm.controls['startAt'].value),
      periodInMinutes: this.objectiveCreateForm.controls['period'].value,
      freeApiId: this.selectedFreeApi.id,
      query: this.objectiveCreateForm.controls['query'].value || null
    };
    this.objectiveService.createObjective(objective).subscribe(() => {
      this.router.navigate(['objectives']);
      let config = new MatSnackBarConfig();
      config.duration = 3000;
      this.snackBar.open('You successfully create objective', 'Close', config);
    });
  }

  public resetStartAtDate(): void {
    this.objectiveCreateForm.controls['startAt'].reset();
  }

  public freeApiChanges(): void {
    this.freeApis.forEach((api: GetFreeApisResponse) => {
      if (api.id === this.objectiveCreateForm.controls['freeApi'].value) {
        this.selectedFreeApi = api;
      }
    });
  }

  public getSubmitButtonDisabler(): boolean {
    if (this.objectiveCreateForm.controls['name'].valid && this.objectiveCreateForm.controls['description'].valid
      && this.objectiveCreateForm.controls['startAt'].valid && this.objectiveCreateForm.controls['period'].valid
      && this.objectiveCreateForm.controls['freeApi'].valid && !this.selectedFreeApi.isQueryRequired) {
      return false;
    } else if (this.objectiveCreateForm.controls['name'].valid && this.objectiveCreateForm.controls['description'].valid
      && this.objectiveCreateForm.controls['startAt'].valid && this.objectiveCreateForm.controls['period'].valid
      && this.objectiveCreateForm.controls['freeApi'].valid && this.selectedFreeApi.isQueryRequired
      && this.objectiveCreateForm.controls['query'].valid) {
      return false;
    } else {
      return true;
    }
  }

  private getFreeApis(): void {
    this.freeApiService.getFreeApis().subscribe((freeApis: GetFreeApisResponse[]) => {
      this.freeApis = freeApis;
      this.initializeForm();
    });
  }

  private initializeForm(): void {
    this.objectiveCreateForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(35)]],
      description: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(150)]],
      startAt: ['', [Validators.required]],
      period: ['', [Validators.required, Validators.min(1), Validators.max(10080)]],
      freeApi: ['', [Validators.required]],
      query: ['', [Validators.maxLength(40), Validators.minLength(3), Validators.required]]
    });
  }
}

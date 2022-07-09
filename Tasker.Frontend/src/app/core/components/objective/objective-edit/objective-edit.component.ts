import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { UpdateObjectiveRequest } from 'src/app/core/models/requests/update-objective-request';
import { GetFreeApisResponse } from 'src/app/core/models/responses/get-free-apis-response';
import { GetObjectiveByIdResponse } from 'src/app/core/models/responses/get-objective-by-id-response';
import { FreeApiService } from 'src/app/core/services/free-api.service';
import { ObjectiveService } from 'src/app/core/services/objective.service';

@Component({
  selector: 'app-objective-edit',
  templateUrl: './objective-edit.component.html',
  styleUrls: ['./objective-edit.component.scss']
})
export class ObjectiveEditComponent implements OnInit {
  public freeApis: GetFreeApisResponse[] = [];
  public objective: GetObjectiveByIdResponse = null;
  public selectedFreeApi: GetFreeApisResponse = null;
  public objectiveEditForm: FormGroup;
  public startAtDefaultTime: number[] = [new Date().getHours(), new Date().getMinutes() + 2, 0];
  public startAtMinDate: Date = new Date(new Date().getTime() + 2 * 60000);

  constructor(private objectiveService: ObjectiveService, private freeApiService: FreeApiService, 
    private router: Router, private route: ActivatedRoute, private formBuilder: FormBuilder,
    private snackBar: MatSnackBar) { }

  public ngOnInit(): void {
    this.getObjectiveById();
  }

  public saveObjective(): void {
    let objective: UpdateObjectiveRequest = {
      id: this.objective.id,
      name: this.objectiveEditForm.controls['name'].value,
      description: this.objectiveEditForm.controls['description'].value,
      startAt: new Date(this.objectiveEditForm.controls['startAt'].value),
      periodInMinutes: this.objectiveEditForm.controls['period'].value,
      freeApiId: this.selectedFreeApi.id,
      query: this.objectiveEditForm.controls['query'].value || null
    };
    this.objectiveService.updateObjective(objective).subscribe(() => {
      this.objectiveEditForm.markAsPristine();
      let config = new MatSnackBarConfig();
      config.duration = 3000;
      this.snackBar.open('You successfully update objective', 'Close', config);
    });
  }

  public resetStartAtDate(): void {
    this.objectiveEditForm.controls['startAt'].reset();
  }

  public freeApiChanges(): void {
    this.objectiveEditForm.controls['query'].setValue('');
    this.freeApis.forEach((api: GetFreeApisResponse) => {
      if (api.id === this.objectiveEditForm.controls['freeApi'].value) {
        this.selectedFreeApi = api;
      }
    });
  }

  public getSubmitButtonDisabler(): boolean {
    if (this.objectiveEditForm.controls['name'].valid && this.objectiveEditForm.controls['description'].valid
      && this.objectiveEditForm.controls['startAt'].valid && this.objectiveEditForm.controls['period'].valid
      && this.objectiveEditForm.controls['freeApi'].valid && !this.selectedFreeApi.isQueryRequired) {
      return false;
    } else if (this.objectiveEditForm.controls['name'].valid && this.objectiveEditForm.controls['description'].valid
      && this.objectiveEditForm.controls['startAt'].valid && this.objectiveEditForm.controls['period'].valid
      && this.objectiveEditForm.controls['freeApi'].valid && this.selectedFreeApi.isQueryRequired
      && this.objectiveEditForm.controls['query'].valid) {
      return false;
    } else {
      return true;
    }
  }

  private getObjectiveById(): void {
    this.route.params.subscribe((params: Params) => {
      this.objectiveService.getObjectiveById(params['id']).pipe(
        catchError(error => {
          this.router.navigate(['objectives']);
          return throwError(() => error);
        })
      ).subscribe((objective: GetObjectiveByIdResponse) => {
        this.objective = objective;
        this.getFreeApis();
      })
    });
  }

  private getFreeApis(): void {
    this.freeApiService.getFreeApis().subscribe((freeApis: GetFreeApisResponse[]) => {
      this.freeApis = freeApis;
      this.freeApis.forEach((api: GetFreeApisResponse) => {
        if(api.id === this.objective.freeApiId) {
          this.selectedFreeApi = api;
        }
      });
      this.initializeForm();
    });
  }

  private initializeForm(): void {
    this.objectiveEditForm = this.formBuilder.group({
      name: [this.objective.name, [Validators.required, Validators.minLength(3), Validators.maxLength(35)]],
      description: [this.objective.description, [Validators.required, Validators.minLength(5), Validators.maxLength(150)]],
      startAt: [this.objective.startAt, [Validators.required]],
      period: [this.objective.periodInMinutes, [Validators.required, Validators.min(5), Validators.max(10080)]],
      freeApi: [this.objective.freeApiId, [Validators.required]],
      query: [this.objective.query || '', [Validators.maxLength(40), Validators.minLength(3), Validators.required]]
    });
    this.objectiveEditForm.controls['startAt'].markAllAsTouched();
  }
}

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { GetObjectivesResponse } from 'src/app/core/models/responses/get-objectives-response';
import { ObjectiveService } from 'src/app/core/services/objective.service';
import { ConfirmationComponent } from 'src/app/shared/components/confirmation/confirmation.component';

@Component({
  selector: 'app-objective-list',
  templateUrl: './objective-list.component.html',
  styleUrls: ['./objective-list.component.scss']
})
export class ObjectiveListComponent implements OnInit {
  public objectives: GetObjectivesResponse[] = [];
  public tableColumns: string[] = ['name', 'description', 'executedLastTime', 'edit', 'delete'];

  constructor(private objectiveService: ObjectiveService, private router: Router, private dialog: MatDialog,
    private snackBar: MatSnackBar) { }

  public ngOnInit(): void {
    this.getObjectives();
  }

  private getObjectives(): void {
    this.objectiveService.getObjectives().subscribe((objectives: GetObjectivesResponse[]) => {
      this.objectives = objectives;
    });
  }
  
  public deleteObjective(id: string): void {
    this.dialog.open(ConfirmationComponent, { disableClose: true, autoFocus: false, data: { confirmationMessage: 'Confirm deletion, please' } })
    .afterClosed().subscribe((result: boolean) => {
      if (!result) {
        return;
      }
      this.objectiveService.deleteObjective(id).subscribe(() => {
        this.getObjectives();
        let config = new MatSnackBarConfig();
        config.duration = 3000;
        this.snackBar.open('You successfully delete objective', 'Close', config);
      });
    });
  }
}

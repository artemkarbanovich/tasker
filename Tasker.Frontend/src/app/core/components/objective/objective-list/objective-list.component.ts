import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { GetObjectivesItems } from 'src/app/core/models/responses/get-objectives-items';
import { GetObjectivesResponse } from 'src/app/core/models/responses/get-objectives-response';
import { ObjectiveService } from 'src/app/core/services/objective.service';
import { ConfirmationComponent } from 'src/app/shared/components/confirmation/confirmation.component';

@Component({
  selector: 'app-objective-list',
  templateUrl: './objective-list.component.html',
  styleUrls: ['./objective-list.component.scss']
})
export class ObjectiveListComponent implements OnInit {
  public objectives: GetObjectivesItems[] = [];
  public tableColumns: string[] = ['name', 'description', 'executedLastTime', 'edit', 'delete'];
  public pageNumber: number = 0;
  public pageSize: number = 5;
  public totalItems: number = 0;
  
  constructor(private objectiveService: ObjectiveService, private router: Router, private dialog: MatDialog,
    private snackBar: MatSnackBar) { }

  public ngOnInit(): void {
    this.getObjectives();
  }
  
  private getObjectives(): void {
    this.objectiveService.getObjectives(this.pageNumber, this.pageSize).subscribe((response: GetObjectivesResponse) => {
      this.objectives = response.objectives;
      this.totalItems = response.totalItems;
      this.pageNumber = response.pageNumber;
      this.pageSize = response.pageSize;
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

  public handlePage(event?: PageEvent): PageEvent {
    this.pageNumber = event.pageIndex;
    this.pageSize = event.pageSize;
    this.getObjectives();
    return event;
  }
}

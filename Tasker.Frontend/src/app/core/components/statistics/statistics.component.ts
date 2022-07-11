import { Component, OnInit } from '@angular/core';
import { GetStatisticsItemResponse } from '../../models/responses/get-statistics-item-response';
import { StatisticsService } from '../../services/statistics.service';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent implements OnInit {
  public statistics: GetStatisticsItemResponse[] = [];
  public tableColumns: string[] = ['userEmail', 'objectivesTotalExecutedCount', 'objectiveExecutedLastTime',
    'totalObjectivesCount', 'objectivesDeletedCount'];
  
  constructor(private statisticsService: StatisticsService) { }

  public ngOnInit(): void {
    this.getStatistics();
  }

  private getStatistics() {
    this.statisticsService.getStatistics().subscribe((statistics: GetStatisticsItemResponse[]) => {
      this.statistics = statistics;
    });
  }
}

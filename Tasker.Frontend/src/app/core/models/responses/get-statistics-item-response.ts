export interface GetStatisticsItemResponse {
    userEmail: string;
    objectivesTotalExecutedCount: number,
    objectiveExecutedLastTime: Date | null;
    totalObjectivesCount: number;
    objectivesDeletedCount: number;
}

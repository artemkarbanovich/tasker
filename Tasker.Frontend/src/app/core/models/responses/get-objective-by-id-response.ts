export interface GetObjectiveByIdResponse {
    id: string;
    name: string;
    description: string;
    creationTime: Date;
    lastestUpdateTime: Date;
    startAt: Date;
    periodInMinutes: number;
    freeApiId: string;
    query: string | null;
    executedCount: number;
    executedLastTime: Date;
}

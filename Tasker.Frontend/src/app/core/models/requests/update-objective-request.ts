export interface UpdateObjectiveRequest {
    id: string;
    name: string;
    description: string;
    startAt: Date;
    periodInMinutes: number;
    freeApiId: string;
    query: string | null;
}

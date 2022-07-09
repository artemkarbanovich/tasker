import { GetObjectivesItems } from "./get-objectives-items";

export interface GetObjectivesResponse {
    pageNumber: number;
    pageSize: number;
    totalItems: number;
    objectives: GetObjectivesItems[];
}

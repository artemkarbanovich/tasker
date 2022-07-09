export interface GetFreeApisResponse {
    id: string;
    name: string;
    apiDescription: string;
    apiIconUrl: string | null;
    isQueryRequired: string;
    queryDescription: string;
}

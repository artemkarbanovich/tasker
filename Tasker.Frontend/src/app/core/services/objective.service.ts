import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CreateObjectiveRequest } from '../models/requests/create-objective-request';
import { UpdateObjectiveRequest } from '../models/requests/update-objective-request';
import { GetObjectiveByIdResponse } from '../models/responses/get-objective-by-id-response';
import { GetObjectivesResponse } from '../models/responses/get-objectives-response';

@Injectable({
  providedIn: 'root'
})
export class ObjectiveService {
  private baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }
  
  public updateObjective(objective: UpdateObjectiveRequest): Observable<void> {
    return this.http.put<void>(this.baseUrl + 'objectives', objective);
  }

  public getObjectiveById(id: string): Observable<GetObjectiveByIdResponse> {
    return this.http.get<GetObjectiveByIdResponse>(this.baseUrl + 'objectives/' + id);
  }
  
  public getObjectives(): Observable<GetObjectivesResponse[]> {
    return this.http.get<GetObjectivesResponse[]>(this.baseUrl + 'objectives');
  }
  
  public deleteObjective(id: string): Observable<void> {
    return this.http.delete<void>(this.baseUrl + 'objectives/' + id);
  }

  public createObjective(objective: CreateObjectiveRequest): Observable<void> {
    return this.http.post<void>(this.baseUrl + 'objectives', objective);
  }
}

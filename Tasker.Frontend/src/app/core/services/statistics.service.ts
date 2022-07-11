import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GetStatisticsItemResponse } from '../models/responses/get-statistics-item-response';

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {
  private baseUrl: string = environment.apiUrl;
  
  constructor(private http: HttpClient) { }
  
  public getStatistics(): Observable<GetStatisticsItemResponse[]> {
    return this.http.get<GetStatisticsItemResponse[]>(this.baseUrl + 'statistics');
  }
}

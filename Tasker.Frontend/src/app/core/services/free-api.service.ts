import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GetFreeApisResponse } from '../models/responses/get-free-apis-response';

@Injectable({
  providedIn: 'root'
})
export class FreeApiService {
  private baseUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) { }
  
  public getFreeApis(): Observable<GetFreeApisResponse[]> {
    return this.http.get<GetFreeApisResponse[]>(this.baseUrl + 'free-apis');
  }
}

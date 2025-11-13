import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { NPSResult } from '../models/nps-result.model';

@Injectable({
  providedIn: 'root'
})
export class NPSService {
  private apiUrl = `${environment.apiUrl}/NPS`;

  constructor(private http: HttpClient) {}

  getResults(): Observable<NPSResult> {
    return this.http.get<NPSResult>(`${this.apiUrl}/results`);
  }
}

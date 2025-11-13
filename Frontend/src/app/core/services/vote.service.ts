import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CheckVoteResponse, VoteRequest, VoteResponse } from '../models/vote.model';

@Injectable({
  providedIn: 'root'
})
export class VoteService {
  private apiUrl = `${environment.apiUrl}/Votes`;

  constructor(private http: HttpClient) {}

  createVote(score: number): Observable<VoteResponse> {
    const request: VoteRequest = { score };
    return this.http.post<VoteResponse>(this.apiUrl, request);
  }

  checkVote(): Observable<CheckVoteResponse> {
    return this.http.get<CheckVoteResponse>(`${this.apiUrl}/check`);
  }
}

export interface VoteRequest {
  score: number;
}

export interface VoteResponse {
  message: string;
  voteId: number;
}

export interface CheckVoteResponse {
  hasVoted: boolean;
}

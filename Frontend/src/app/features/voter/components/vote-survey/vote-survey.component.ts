import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { VoteService } from '../../../../core/services/vote.service';

@Component({
  selector: 'app-vote-survey',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vote-survey.component.html',
  styleUrls: ['./vote-survey.component.scss']
})
export class VoteSurveyComponent implements OnInit {
  selectedScore: number | null = null;
  isLoading = false;
  error: string | null = null;
  hasVoted = false;

  // Array para generar los botones de puntuación (0-10)
  scores = Array.from({ length: 11 }, (_, i) => i);

  constructor(
    private voteService: VoteService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Verificar si el usuario ya ha votado
    this.checkIfUserHasVoted();
  }

  checkIfUserHasVoted(): void {
    this.isLoading = true;
    
    this.voteService.checkVote().subscribe({
      next: (response) => {
        this.isLoading = false;
        this.hasVoted = response.hasVoted;
        
        // Si ya votó, redirigir a la página de éxito
        if (this.hasVoted) {
          this.router.navigate(['/voter/success']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.error = err.message || 'Error al verificar el estado del voto';
      }
    });
  }

  selectScore(score: number): void {
    this.selectedScore = score;
  }

  submitVote(): void {
    if (this.selectedScore === null) {
      return;
    }

    this.isLoading = true;
    this.error = null;

    this.voteService.createVote(this.selectedScore).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/voter/success']);
      },
      error: (err) => {
        this.isLoading = false;
        this.error = err.message || 'Error al enviar el voto';
      }
    });
  }
}

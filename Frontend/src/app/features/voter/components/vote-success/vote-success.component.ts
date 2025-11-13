import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-vote-success',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './vote-success.component.html',
  styleUrls: ['./vote-success.component.scss']
})
export class VoteSuccessComponent {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: () => {
        // Incluso si hay error, redirigir al login
        this.router.navigate(['/auth/login']);
      }
    });
  }
}

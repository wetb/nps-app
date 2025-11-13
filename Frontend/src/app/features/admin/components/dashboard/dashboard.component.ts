import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NPSService } from '../../../../core/services/nps.service';
import { AuthService } from '../../../../core/services/auth.service';
import { NPSResult } from '../../../../core/models/nps-result.model';
import { NpsChartComponent } from '../nps-chart/nps-chart.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, NpsChartComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  npsData: NPSResult | null = null;
  isLoading = false;
  error: string | null = null;
  username: string = '';

  constructor(
    private npsService: NPSService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadNPSData();
    const user = this.authService.getUserRole();
    if (user) {
      this.username = user;
    }
  }

  loadNPSData(): void {
    this.isLoading = true;
    this.error = null;

    this.npsService.getResults().subscribe({
      next: (data) => {
        this.npsData = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = err.message || 'Error al cargar los datos de NPS';
        this.isLoading = false;
      }
    });
  }

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

  refresh(): void {
    this.loadNPSData();
  }
}

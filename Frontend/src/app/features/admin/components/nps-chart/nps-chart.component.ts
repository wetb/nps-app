import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, registerables } from 'chart.js';
import { NPSResult } from '../../../../core/models/nps-result.model';

// Registrar los componentes necesarios de Chart.js
Chart.register(...registerables);

@Component({
  selector: 'app-nps-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './nps-chart.component.html',
  styleUrls: ['./nps-chart.component.scss']
})
export class NpsChartComponent implements OnChanges {
  @Input() npsData!: NPSResult;
  
  private chart: Chart | null = null;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['npsData'] && this.npsData) {
      this.renderChart();
    }
  }

  private renderChart(): void {
    const ctx = document.getElementById('npsChart') as HTMLCanvasElement;
    
    if (!ctx) {
      console.error('Canvas element not found');
      return;
    }
    
    // Destruir el gráfico anterior si existe
    if (this.chart) {
      this.chart.destroy();
    }
    
    // Crear el nuevo gráfico
    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: ['Promotores', 'Neutrales', 'Detractores'],
        datasets: [{
          label: 'Distribución NPS',
          data: [
            this.npsData.promotersPercentage,
            this.npsData.neutralsPercentage,
            this.npsData.detractorsPercentage
          ],
          backgroundColor: [
            'rgba(40, 167, 69, 0.7)',  // Verde para promotores
            'rgba(255, 193, 7, 0.7)',  // Amarillo para neutrales
            'rgba(220, 53, 69, 0.7)'   // Rojo para detractores
          ],
          borderColor: [
            'rgba(40, 167, 69, 1)',
            'rgba(255, 193, 7, 1)',
            'rgba(220, 53, 69, 1)'
          ],
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            beginAtZero: true,
            max: 100,
            ticks: {
              callback: function(value) {
                return value + '%';
              }
            }
          }
        },
        plugins: {
          tooltip: {
            callbacks: {
              label: function(context) {
                const value = context.parsed.y;
                return context.dataset.label + ': ' + (value ? value.toFixed(1) : '0') + '%';
              }
            }
          },
          legend: {
            display: false
          }
        }
      }
    });
  }
}

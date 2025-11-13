import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { NoAuthGuard } from './core/guards/no-auth.guard';
import { RoleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // Ruta por defecto
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },
  
  // Rutas de autenticación
  {
    path: 'auth',
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', loadComponent: () => import('./features/auth/components/login/login.component').then(m => m.LoginComponent) }
    ],
    canActivate: [NoAuthGuard]
  },
  
  // Rutas de votante
  {
    path: 'voter',
    children: [
      { path: '', redirectTo: 'survey', pathMatch: 'full' },
      { path: 'survey', loadComponent: () => import('./features/voter/components/vote-survey/vote-survey.component').then(m => m.VoteSurveyComponent) },
      { path: 'success', loadComponent: () => import('./features/voter/components/vote-success/vote-success.component').then(m => m.VoteSuccessComponent) }
    ],
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Voter'] }
  },
  
  // Rutas de administrador
  {
    path: 'admin',
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./features/admin/components/dashboard/dashboard.component').then(m => m.DashboardComponent) }
    ],
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] }
  },
  
  // Ruta para cualquier otra URL no definida
  { path: '**', redirectTo: '/auth/login' }
];

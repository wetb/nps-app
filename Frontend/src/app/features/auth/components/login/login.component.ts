import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoading = false;
  error: string | null = null;
  showTimeout = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });

    // Verificar si viene de un timeout de sesión
    this.route.queryParams.subscribe(params => {
      if (params['timeout']) {
        this.showTimeout = true;
        this.error = 'Su sesión ha expirado por inactividad. Por favor inicie sesión nuevamente.';
      } else if (params['unauthorized']) {
        this.error = 'Sesión no válida. Por favor inicie sesión nuevamente.';
      } else if (params['forbidden']) {
        this.error = 'No tiene permisos para acceder a ese recurso.';
      }
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.error = null;

    const { username, password } = this.loginForm.value;

    this.authService.login(username, password).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.role === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        } else {
          this.router.navigate(['/voter/survey']);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.error = err.message || 'Error al iniciar sesión';
      }
    });
  }
}

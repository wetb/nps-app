export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api', // Usando localhost para acceder desde el navegador
  tokenRefreshTimeoutMinutes: 4 // Refrescar 1 minuto antes de que expire (token expira en 5 min)
};

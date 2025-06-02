import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient,HttpClientModule  } from '@angular/common/http';
import { CommonModule } from '@angular/common'; 
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-jugador-form',
  standalone: true,
  templateUrl: './jugador-form.component.html',
  styleUrls: ['./jugador-form.component.css'],
  imports: [FormsModule, CommonModule,HttpClientModule]
    
})
export class JugadorFormComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';  


  constructor(private http: HttpClient, private router: Router) {}

  onSubmit() {
   const loginData = {
    username: this.username,
    password: this.password
  };
     console.log('Login data enviada:', loginData);

      
  this.http.post<any>(`${environment.apiUrl}/api/user/autenticar`, loginData).subscribe(
  response => {
    console.log('Respuesta del backend:', response);
    localStorage.setItem('jwt', response.token); // ✅ Guarda el token
    //localStorage.setItem('jwt', response.IdUser);
    //localStorage.setItem('jwt', response.Username);
    //localStorage.setItem('jwt', response.token);
    localStorage.setItem('idUser', response.IdUser);
    localStorage.setItem('username', response.Username);

    this.router.navigate(['/grafico']);
  },
  error => {
    console.error('Error en el login:', error);
    if (error.status === 401) {
      this.errorMessage = 'Credenciales inválidas. Por favor verifica usuario y contraseña.';
    } else {
      this.errorMessage = 'Error inesperado. Intenta más tarde.';
    }
  }
                                                                                        );
  } 
}

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import {FormsModule} from '@angular/forms';
@Component({
  selector: 'app-jugador-form',
  standalone: true,
  templateUrl: './jugador-form.component.html',
  styleUrls: ['./jugador-form.component.css'],
  imports: [FormsModule]
})
export class JugadorFormComponent {
  playerName: string = '';

  constructor(private router: Router) {}

  onSubmit() {
    // Aquí se va a enviar el nombre a C#
    console.log('Nombre del jugador enviado:', this.playerName);

    // Redirigir a la página de código
    this.router.navigate(['/codigo'],{ queryParams: { name: this.playerName } });
  }
}

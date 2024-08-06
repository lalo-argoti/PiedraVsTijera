import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiServiceTsService } from '../api.service.ts.service'; // Ajusta la ruta si es necesario

@Component({
  selector: 'app-juego',
  standalone: true,
  templateUrl: './juego.component.html',
  styleUrls: ['./juego.component.css']
})
export class JuegoComponent implements OnInit {
  code: string | null = null;
  playerName: string | null = null;
  movimiento: number = 0; // Asigna un valor por defecto o actualízalo según sea necesario

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiServiceTsService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.code = params['code'] || null;
      this.playerName = params['playerName'] || null;

      console.log('Código recibido:', this.code);
      console.log('Nombre del jugador recibido:', this.playerName);

      if (this.code && this.playerName) {
        // Asume que `code` es `partida`, asigna un valor para `movimiento` según sea necesario
        this.apiService.getPartidoDetalles(this.playerName, this.code, this.movimiento).subscribe(data => {
          console.log('Datos del partido:', data);
        });
      }
    });
  }

  // Método para manejar el clic en el SVG
  onSvgClick(movimiento: number) {
    if (this.playerName && this.code) {
      const url = `http://51.222.141.101:8000/partido?user=${this.playerName}&partida=${this.code}&movimiento=${movimiento}`;
      this.apiService.getPartidoDetalles(this.playerName, this.code, movimiento).subscribe(data => {
        console.log('Datos del partido:', data);
      });
    }
  }
}

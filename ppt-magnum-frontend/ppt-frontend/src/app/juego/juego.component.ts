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

      if (this.code) {
        this.apiService.getPartidoDetalles(this.code).subscribe(data => {
          console.log('Datos del partido:', data);
        });
      }
    });
  }
}

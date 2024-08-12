import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiServiceTsService } from '../api.service.ts.service'; // Ajusta la ruta si es necesario
import { Subscription, interval, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-juego',
  standalone: true,
  templateUrl: './juego.component.html',
  styleUrls: ['./juego.component.css'],
  imports: [CommonModule]
})
export class JuegoComponent implements OnInit, OnDestroy {
  code: string | null = null;
  playerName: string | null = null;
  movimiento: number = 0; // Asigna un valor por defecto o actualízalo según sea necesario
  partidoDetalles: string[] = []; // Arreglo para almacenar los datos del partido
  datosFiltrados: string[] = []; // Lista para los datos filtrados
  partidoDetalle: string | null = null;
  partidoDetalle1: string  | null = null;
  partidoDetalle2: string  | null = null;
  private pollingSubscription: Subscription | null = null;

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
        this.startPolling(); // Inicia el polling
      }
    });
  }

  onSvgClick(movimiento: number) {
    if (this.playerName && this.code) {
      const url = `http://51.222.141.101:8000/partido?user=${this.playerName}&partida=${this.code}&movimiento=${movimiento}`;
      this.apiService.getPartidoDetalles(this.playerName, this.code, movimiento).subscribe(data => {
        console.log('Datos del partido:', data);
      });
    }
  }

  private startPolling() {
  // Verifica si playerName y code están definidos
  if (this.playerName && this.code) {
    // Configura el polling cada 3 segundos (3000 ms)
    this.pollingSubscription = interval(3000).pipe(
      switchMap(() => {
        // Verifica nuevamente si playerName y code están definidos
        if (this.playerName && this.code) {
          return this.apiService.getPartidoDetalles(this.playerName, this.code, this.movimiento);
        } else {
          // Manejo de error si falta playerName o code
          console.error('playerName o code son null');
          return of(null); // Devuelve un Observable vacío en caso de error
        }
      })
    ).subscribe({
      next: (data) => {
        if (data && data.ServeSay) {
          console.log('Datos del partido (actualización periódica):', data);

          // Actualiza las variables según el contenido de ServeSay
           //Movimientos         
            this.partidoDetalle = data.ServeSay;
          
            this.partidoDetalle1 = data.Movimientos;
          
            this.partidoDetalle2 = data.Puntos;
          
        }
       },
       error: (err) => {
        // Maneja los errores que ocurran durante el polling
        console.error('Error durante el polling:', err);
       }
      });
     } else {
       // Manejo de caso cuando playerName o code son null
       console.error('playerName o code no están definidos');
     }
   }





  private filtrarDatos(): void {
    // Filtra los datos que cumplen con la condición y los actualiza
    this.datosFiltrados = this.partidoDetalles.filter(dato => dato.includes('->'));
  }

  ngOnDestroy() {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }
}

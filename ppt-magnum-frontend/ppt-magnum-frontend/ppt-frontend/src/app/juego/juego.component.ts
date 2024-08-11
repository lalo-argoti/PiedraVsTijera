import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiServiceTsService } from '../api.service.ts.service'; // Ajusta la ruta si es necesario
import { Subscription, interval, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-juego',
  standalone: true,
  templateUrl: './juego.component.html',
  styleUrls: ['./juego.component.css']
})
export class JuegoComponent implements OnInit  , OnDestroy{
  code: string | null = null;
  playerName: string | null = null;
  movimiento: number = 0; // Asigna un valor por defecto o actualízalo según sea necesario

  private pollingSubscription: Subscription | undefined;

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
       this.startPolling(); /// Añadido para iniciar el polling
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

  private startPolling() { /// Añadido para método de polling
  if (this.playerName && this.code) {
    // Configura el polling cada 3 segundos (3000 ms)
    this.pollingSubscription = interval(3000).pipe(
      switchMap(() => {
        // Asegúrate de que playerName y code no sean null
        if (this.playerName && this.code) {
          return this.apiService.getPartidoDetalles(this.playerName, this.code, this.movimiento);
        } else {
          // Manejo de error si falta un parámetro
          console.error('playerName o code son null');
          return of(null); // Usa un Observable vacío en caso de error
        }
      })
    ).subscribe(data => {
      if (data) {
        console.log('Datos del partido (actualización periódica):', data);
      }
    }, error => {
      console.error('Error durante el polling:', error);
    });
  } else {
    // Manejo de caso cuando playerName o code son null
    console.error('playerName o code no están definidos');
  }
}


  ngOnDestroy() { /// Añadido para limpiar la suscripción
    // Limpia la suscripción cuando el componente se desmonte
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }

}

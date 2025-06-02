import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://172.20.10.3:8000';

  constructor(private http: HttpClient) { }


  getPartidoDetalles(user: string, partida: string, movimiento: number): Observable<any> {
  const url = `${this.apiUrl}/ping`;
  return this.http.get<any>(url);
     }

  getEstadoJuego(codigo: string): Observable<any> {
  const url = `${this.apiUrl}/ping`;
  return this.http.get<any>(url);
  }

  


}

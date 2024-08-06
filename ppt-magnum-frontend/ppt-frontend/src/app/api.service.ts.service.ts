import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiServiceTsService {
  private apiUrl = 'http://51.222.141.101:8000/partido';

  constructor(private http: HttpClient) { }


  getPartidoDetalles(user: string, partida: string, movimiento: number): Observable<any> {
  const url = `${this.apiUrl}?user=${user}&partida=${partida}&movimiento=${movimiento}`;
  return this.http.get<any>(url);

  }
}

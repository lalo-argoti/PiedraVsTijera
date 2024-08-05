import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiServiceTsService {
  private apiUrl = 'https://mdn.github.io/learning-area/javascript/oojs/json/superheroes.json';

  constructor(private http: HttpClient) { }

  getPartidoDetalles(codigo: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?codigo=${codigo}`);
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-tipos-gasto',
  standalone: true,
  templateUrl: './tipos-gasto.component.html',
  styleUrls: ['./tipos-gasto.component.css'],
  imports: [CommonModule, FormsModule],
})
export class TiposGastoComponent implements OnInit {
  tiposGasto: {
    id: number;
    nombre: string;
    presupuestocol: number;
    presupuestousd: number;
    codigo?: string;
  }[] = [];

  @Input() codigoGenerado: string = '';

  nombre: string = '';
  presupuestocol: number = 0;
  presupuestousd: number = 0;
  editandoCodigo: string | null = null;

  private apiUrl = `${environment.apiUrl}/api/GastoTipo`;

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit() {
    this.cargarTiposGasto();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  cargarTiposGasto() {
    this.http.get<any[]>(this.apiUrl, { headers: this.getAuthHeaders() }).subscribe({
      next: (data) => {
        this.tiposGasto = data.map((tipo) => ({
          ...tipo,
          codigo: `TG-${tipo.id.toString().padStart(4, '0')}`,
        }));
        this.codigoGenerado = this.generarCodigo();
      },
      error: (err) => {
        console.error('Error al obtener tipos de gasto', err);
      }
    });
  }

  generarCodigo(): string {
    const maxId = this.tiposGasto.length > 0 ? Math.max(...this.tiposGasto.map(t => t.id)) : 0;
    return `TG-${(maxId + 1).toString().padStart(4, '0')}`;
  }

  onSubmit() {
    const tipo = {
      nombre: this.nombre,
      presupuestocol: this.presupuestocol,
      presupuestousd: this.presupuestousd,
    };

    if (this.editandoCodigo) {
      this.http.put(`${this.apiUrl}/${this.editandoCodigo}`, tipo, { headers: this.getAuthHeaders() }).subscribe({
        next: () => {
          alert('Actualizado correctamente');
          this.cargarTiposGasto();
          this.cancelarEdicion();
        },
        error: err => console.error('Error al actualizar', err)
      });
    } else {
      this.http.post(this.apiUrl, tipo, { headers: this.getAuthHeaders() }).subscribe({
        next: () => {
          alert('Guardado correctamente');
          this.cargarTiposGasto();
          this.limpiarFormulario();
        },
        error: err => console.error('Error al guardar', err)
      });
    }
  }

  editarTipo(tipo: any) {
    this.editandoCodigo = tipo.id.toString();
    this.nombre = tipo.nombre;
    this.presupuestocol = tipo.presupuestocol;
    this.presupuestousd = tipo.presupuestousd;
  }

  eliminarTipo(id: number) {
    this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() }).subscribe({
      next: () => {
        alert('Eliminado correctamente');
        this.cargarTiposGasto();
      },
      error: err => console.error('Error al eliminar', err)
    });
  }

  cancelarEdicion() {
    this.editandoCodigo = null;
    this.limpiarFormulario();
  }

  limpiarFormulario() {
    this.nombre = '';
    this.presupuestocol = 0;
    this.presupuestousd = 0;
    this.codigoGenerado = this.generarCodigo();
  }
}

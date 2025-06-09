import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface FondoMonetario {
  id: number;
  nombre: string;
  tipo: 'Caja' | 'Bancaria';
  capitalCOP: number;
  capitalUSD: number;
  codigo?: string;
}

@Component({
  selector: 'app-fondo-monetario',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fondo-monetario.component.html',
  styleUrls: ['./fondo-monetario.component.css']
})
export class FondoMonetarioComponent implements OnInit {
  fondos: FondoMonetario[] = [];

  nombre: string = '';
  tipo: 'Caja' | 'Bancaria' = 'Caja';
  capitalCOP: number = 0;
  capitalUSD: number = 0;

  codigoGenerado: string = '';
  editandoId: number | null = null;

  private apiUrl = `${environment.apiUrl}/api/FondoMonetario`;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.cargarFondos();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  cargarFondos() {
    this.http.get<FondoMonetario[]>(this.apiUrl, { headers: this.getAuthHeaders() }).subscribe({
      next: (data) => {
        this.fondos = data.map(fondo => ({
          ...fondo,
          codigo: `FM-${fondo.id.toString().padStart(4, '0')}`
        }));
        this.codigoGenerado = this.generarCodigo();
      },
      error: (err) => {
        console.error('Error al cargar fondos', err);
      }
    });
  }

  generarCodigo(): string {
    const maxId = this.fondos.length > 0 ? Math.max(...this.fondos.map(f => f.id)) : 0;
    return `FM-${(maxId + 1).toString().padStart(4, '0')}`;
  }

  onSubmit() {
    const fondo = {
      nombre: this.nombre,
      tipo: this.tipo,
      capitalCOP: this.capitalCOP,
      capitalUSD: this.capitalUSD
    };
    //console.log("76: ",fondo);
    
    if (this.editandoId !== null) {
      this.http.put(`${this.apiUrl}/${this.editandoId}`, fondo, { headers: this.getAuthHeaders() }).subscribe({
        next: () => {
          alert('Fondo actualizado correctamente');
          this.cargarFondos();
          this.cancelarEdicion();
        },
        error: err => console.error('Error al actualizar', err)
      });
    } else {
      this.http.post(this.apiUrl, fondo, { headers: this.getAuthHeaders() }).subscribe({
        next: () => {
          alert('Fondo creado correctamente');
          this.cargarFondos();
          this.limpiarFormulario();
        },
        error: err => console.error('Error al crear', err)
      });
    }
  }

  editar(fondo: FondoMonetario) {
    this.editandoId = fondo.id;
    this.nombre = fondo.nombre;
    this.tipo = fondo.tipo;
    this.capitalCOP = fondo.capitalCOP;
    this.capitalUSD = fondo.capitalUSD;
    this.codigoGenerado = `FM-${fondo.id.toString().padStart(4, '0')}`;
  }

  eliminar(id: number) {
    if (confirm('¿Está seguro de eliminar este fondo?')) {
      this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() }).subscribe({
        next: () => {
          alert('Fondo eliminado correctamente');
          this.cargarFondos();
        },
        error: err => console.error('Error al eliminar', err)
      });
    }
  }

  cancelarEdicion() {
    this.editandoId = null;
    this.limpiarFormulario();
  }

  limpiarFormulario() {
    this.nombre = '';
    this.tipo = 'Caja';
    this.capitalCOP = 0;
    this.capitalUSD = 0;
    this.codigoGenerado = this.generarCodigo();
  }
}

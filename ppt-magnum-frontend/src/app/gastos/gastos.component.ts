import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-gastos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gastos.component.html',
  styleUrls: ['./gastos.component.css']
})
export class GastosComponent implements OnInit {
  encabezado = {
    fecha: new Date().toISOString().split('T')[0],
    observaciones: '',
    criterio: ''
  };

  detalle = {
    gastoTipoId: '',
    fondoId: '',
    monto: 0
  };

  detalles: any[] = [];
  tiposGasto: any[] = [];
  fondos: any[] = [];

  total = 0;
  editandoDetalleId: number | null = null;

  private apiUrl = 'http://172.20.10.3:8000/api/gastoregistro';
  propietarioId = 3; // Temporal o extraído del login

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.cargarDatosIniciales();
  }

  cargarDatosIniciales() {
    this.http.get<any[]>(`http://172.20.10.3:8000/api/GastosTipo/${this.propietarioId}`).subscribe({
      next: (data) => this.tiposGasto = data,
      error: (err) => console.error('Error al cargar tipos de gasto', err)
    });

    this.http.get<any[]>(`http://172.20.10.3:8000/api/Fondos/${this.propietarioId}`).subscribe({
      next: (data) => this.fondos = data,
      error: (err) => console.error('Error al cargar fondos', err)
    });
  }

  agregarDetalle() {
    if (this.editandoDetalleId !== null) {
      this.detalles[this.editandoDetalleId] = { ...this.detalle };
      this.editandoDetalleId = null;
    } else {
      this.detalles.push({ ...this.detalle });
    }

    this.calcularTotal();
    this.limpiarFormularioDetalle();
  }

  editarDetalle(index: number) {
    this.editandoDetalleId = index;
    this.detalle = { ...this.detalles[index] };
  }

  eliminarDetalle(index: number) {
    this.detalles.splice(index, 1);
    this.calcularTotal();
  }

  cancelarEdicionDetalle() {
    this.editandoDetalleId = null;
    this.limpiarFormularioDetalle();
  }

  limpiarFormularioDetalle() {
    this.detalle = {
      gastoTipoId: '',
      fondoId: '',
      monto: 0
    };
  }

  calcularTotal() {
    this.total = this.detalles.reduce((sum, item) => sum + (item.monto || 0), 0);
  }

  onSubmit() {
    if (this.detalles.length === 0) {
      alert('Debe agregar al menos un detalle');
      return;
    }

    const propietario = '3'; // Puedes obtenerlo del token si usas autenticación

    const payload = {
      fecha: this.encabezado.fecha,
      observaciones: this.encabezado.observaciones,
      criterio: this.encabezado.criterio,
      propietario: propietario,
      fondoId: this.detalles[0]?.fondoId || 1,
      detalles: this.detalles.map(d => ({
        gastoTipoId: d.gastoTipoId,
        monto: d.monto
      }))
    };

    this.http.post(this.apiUrl, payload).subscribe({
      next: () => {
        alert('Transacción guardada correctamente');
        this.resetearFormulario();
      },
      error: (err) => console.error('Error al guardar transacción', err)
    });
  }

  resetearFormulario() {
    this.encabezado = {
      fecha: new Date().toISOString().split('T')[0],
      observaciones: '',
      criterio: ''
    };
    this.detalles = [];
    this.limpiarFormularioDetalle();
    this.calcularTotal();
  }

  getNombreTipoGasto(id: number): string {
    const tipo = this.tiposGasto.find(t => t.id === id);
    return tipo ? tipo.nombre : 'Desconocido';
  }

  getNombreFondo(id: number): string {
    const fondo = this.fondos.find(f => f.id === id);
    return fondo ? fondo.nombre : 'Desconocido';
  }
}

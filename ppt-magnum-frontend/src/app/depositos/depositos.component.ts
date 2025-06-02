import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-depositos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './depositos.component.html',
  styleUrls: ['./depositos.component.css']
})
export class DepositosComponent implements OnInit {
  // Encabezado del depósito
  encabezado = {
    fecha: new Date().toISOString().split('T')[0],
    descripcion: '',
    mes: new Date().getMonth() + 1,
    anio: new Date().getFullYear(),
    referencia: ''
  };

  // Detalle del depósito
  detalle = {
    fondoId: '',
    montoCOP: 0,
    montoUSD: 0,
    metodoPago: 'transferencia',
    referenciaPago: ''
  };

  // Lista de detalles
  detalles: any[] = [];

  // Datos para selects
  meses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, 
    { id: 3, nombre: 'Marzo' }, { id: 4, nombre: 'Abril' },
    { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' },
    { id: 9, nombre: 'Septiembre' }, { id: 10, nombre: 'Octubre' },
    { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
  ];

  anios = Array.from({length: 10}, (_, i) => new Date().getFullYear() - i);
  fondos: any[] = [];
  metodosPago = [
    { id: 'transferencia', nombre: 'Transferencia' },
    { id: 'efectivo', nombre: 'Efectivo' },
    { id: 'cheque', nombre: 'Cheque' },
    { id: 'tarjeta', nombre: 'Tarjeta' }
  ];

  // Totales
  totalCOP = 0;
  totalUSD = 0;

  // Control de edición
  editandoDetalleId: number | null = null;

  private apiUrl = `${environment.apiUrl}/api/depositos`;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.cargarDatosIniciales();
  }

  cargarDatosIniciales() {
    this.http.get<any[]>(`${environment.apiUrl}/api/fondos`).subscribe({
      next: (data) => this.fondos = data,
      error: (err) => console.error('Error al cargar fondos', err)
    });
  }

  agregarDetalle() {
    if (this.editandoDetalleId !== null) {
      // Actualizar detalle existente
      this.detalles[this.editandoDetalleId] = {...this.detalle};
      this.editandoDetalleId = null;
    } else {
      // Agregar nuevo detalle
      this.detalles.push({...this.detalle});
    }
    
    this.calcularTotales();
    this.limpiarFormularioDetalle();
  }

  editarDetalle(index: number) {
    this.editandoDetalleId = index;
    this.detalle = {...this.detalles[index]};
  }

  eliminarDetalle(index: number) {
    this.detalles.splice(index, 1);
    this.calcularTotales();
  }

  cancelarEdicionDetalle() {
    this.editandoDetalleId = null;
    this.limpiarFormularioDetalle();
  }

  limpiarFormularioDetalle() {
    this.detalle = {
      fondoId: '',
      montoCOP: 0,
      montoUSD: 0,
      metodoPago: 'transferencia',
      referenciaPago: ''
    };
  }

  calcularTotales() {
    this.totalCOP = this.detalles.reduce((sum, item) => sum + (item.montoCOP || 0), 0);
    this.totalUSD = this.detalles.reduce((sum, item) => sum + (item.montoUSD || 0), 0);
  }

  onSubmit() {
    if (this.detalles.length === 0) {
      alert('Debe agregar al menos un detalle');
      return;
    }

    const transaccion = {
      encabezado: this.encabezado,
      detalles: this.detalles
    };

    this.http.post(this.apiUrl, transaccion).subscribe({
      next: () => {
        alert('Depósito registrado correctamente');
        this.resetearFormulario();
      },
      error: (err) => {
        console.error('Error al guardar depósito', err);
        alert('Error al guardar el depósito');
      }
    });
  }

  resetearFormulario() {
    this.encabezado = {
      fecha: new Date().toISOString().split('T')[0],
      descripcion: '',
      mes: new Date().getMonth() + 1,
      anio: new Date().getFullYear(),
      referencia: ''
    };
    this.detalles = [];
    this.limpiarFormularioDetalle();
    this.calcularTotales();
  }

  getNombreFondo(id: number): string {
    const fondo = this.fondos.find(f => f.id === id);
    return fondo ? fondo.nombre : 'Desconocido';
  }

  getNombreMetodoPago(id: string): string {
    const metodo = this.metodosPago.find(m => m.id === id);
    return metodo ? metodo.nombre : 'Desconocido';
  }
}

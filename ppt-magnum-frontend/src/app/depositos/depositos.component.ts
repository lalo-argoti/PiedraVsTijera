import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface DepositoEncabezado {
  fecha: string;
  descripcion: string;
  mes: number;
  anio: number;
  referencia: string;
}

interface DepositoDetalle {
  fondoId: number;
  montoCOP: number;
  montoUSD: number;
  metodoPago: string;
  referenciaPago: string;
}

interface DepositoTransaccion {
  encabezado: DepositoEncabezado;
  detalles: DepositoDetalle[];
}

@Component({
  selector: 'app-depositos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './depositos.component.html',
  styleUrls: ['./depositos.component.css']
})
export class DepositosComponent implements OnInit {
  encabezado: DepositoEncabezado = {
    fecha: new Date().toISOString().split('T')[0],
    descripcion: '',
    mes: new Date().getMonth() + 1,
    anio: new Date().getFullYear(),
    referencia: ''
  };

  detalle: DepositoDetalle = {
    fondoId: 0,
    montoCOP: 0,
    montoUSD: 0,
    metodoPago: 'transferencia',
    referenciaPago: ''
  };

  detalles: DepositoDetalle[] = [];

  meses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' },
    { id: 3, nombre: 'Marzo' }, { id: 4, nombre: 'Abril' },
    { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' },
    { id: 9, nombre: 'Septiembre' }, { id: 10, nombre: 'Octubre' },
    { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
  ];

  anios = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - i);
  fondos: any[] = [];
  metodosPago = [
    { id: 'transferencia', nombre: 'Transferencia' },
    { id: 'efectivo', nombre: 'Efectivo' },
    { id: 'cheque', nombre: 'Cheque' },
    { id: 'tarjeta', nombre: 'Tarjeta' }
  ];

  totalCOP = 0;
  totalUSD = 0;

  editandoDetalleId: number | null = null;

  private apiUrl = `${environment.apiUrl}/api/deposito`;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.cargarDatosIniciales();
  }

  cargarDatosIniciales() {
  const initUrl = `${environment.apiUrl}/api/presupuestomovimiento/init`;
  this.http.get<any>(initUrl).subscribe({
    next: (data) => {
      this.fondos = data.fondos;
      // Si en el futuro necesitas tiposGasto, también estarán disponibles aquí
    },
    error: (err) => console.error('Error al cargar datos iniciales', err)
  });
}


  agregarDetalle() {
    if (!this.detalle.fondoId || this.detalle.fondoId === 0) {
      alert('Debe seleccionar un fondo');
      return;
    }

    if (this.detalle.montoCOP <= 0 && this.detalle.montoUSD <= 0) {
      alert('Debe ingresar al menos un monto en COP o USD');
      return;
    }

    if (this.editandoDetalleId !== null) {
      this.detalles[this.editandoDetalleId] = { ...this.detalle };
      this.editandoDetalleId = null;
    } else {
      this.detalles.push({ ...this.detalle });
    }

    this.calcularTotales();
    this.limpiarFormularioDetalle();
  }

  editarDetalle(index: number) {
    this.editandoDetalleId = index;
    this.detalle = { ...this.detalles[index] };
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
      fondoId: 0,
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

    const transaccion: DepositoTransaccion = {
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

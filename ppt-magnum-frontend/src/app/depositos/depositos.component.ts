import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface DepositoEncabezado {
  id?: number;
  fecha: string;
  remitente: string;
  propietario?: string;
}

interface DepositoDetalle {
  id?: number;
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
    remitente: ''
  };

  detalle: DepositoDetalle = {
    fondoId: 0,
    montoCOP: 0,
    montoUSD: 0,
    metodoPago: 'transferencia',
    referenciaPago: ''
  };

  detalles: DepositoDetalle[] = [];

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
  editandoDepositoId: number | null = null;

  private apiUrl = `${environment.apiUrl}/api/deposito`;
  private initUrl = `${environment.apiUrl}/api/presupuestomovimiento/init`;
  meses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' }, { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' }, { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' }, { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
  ];
  anios = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - i);

  mesSeleccionado = new Date().getMonth() + 1;
  anioSeleccionado = new Date().getFullYear();

  depositosFiltrados: DepositoTransaccion[] = [];

  constructor(private http: HttpClient) {}

tiposGasto: any[] = [];

ngOnInit() {
  this.cargarDatosIniciales().then(() => {
    this.cargarDepositosPorMes();
  });
}


  
  cargarDatosIniciales(): Promise<void> {
  console.log("Se vana a cargar desde:", this.initUrl);
  return new Promise((resolve, reject) => {
    this.http.get<any>(this.initUrl).subscribe({
      next: (data) => {
        this.fondos = data.fondos;
        this.tiposGasto = data.tiposGasto;
        resolve();
      },
      error: (err) => {
        console.error('Error al cargar datos iniciales', err);
        reject(err);
      }
      
    });
  });
}

  cargarDepositosPorMes() {
      
    const params = new HttpParams()
      .set('mes', this.mesSeleccionado.toString())
      .set('anio', this.anioSeleccionado.toString());

    this.http.get<DepositoTransaccion[]>(this.apiUrl, { params }).subscribe({
      next: data => {
        console.log('Depósitos obtenidos:', data);
        this.depositosFiltrados = data;
      },
      error: err => {
        console.error('Error cargando depósitos por mes', err);
      }
      
    });
  }

  cambiarMes() {
    this.cargarDepositosPorMes();
  }

cargarDepositoExistente(id: number) {
  this.http.get<DepositoTransaccion>(`${this.apiUrl}/${id}`).subscribe({
    next: (data) => {
      this.encabezado = {
        id: data.encabezado.id,
        fecha: data.encabezado.fecha?.split('T')[0] || new Date().toISOString().split('T')[0],
        remitente: data.encabezado.remitente,
        propietario: data.encabezado.propietario
      };
      this.editandoDepositoId = data.encabezado.id || null;

      this.detalles = data.detalles.map(d => ({
        id: d.id,
        fondoId: d.fondoId,
        montoCOP: d.montoCOP,
        montoUSD: d.montoUSD,
        metodoPago: d.metodoPago || 'transferencia',
        referenciaPago: d.referenciaPago || ''
      }));

      this.calcularTotales();
    },
    error: err => {
      console.error('Error cargando depósito', err);
    }
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
      console.log("174:  " , this.detalle)
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
    console.log(transaccion)

    if (this.editandoDepositoId !== null) {
      this.http.put(`${this.apiUrl}/${this.editandoDepositoId}`, transaccion).subscribe({
        next: () => {
          alert('Depósito actualizado correctamente');
          console.log(transaccion);
          this.resetearFormulario();
          this.cargarDepositosPorMes(); 
        },
        error: err => {
          console.error('Error al actualizar depósito', err);
          alert('Error al actualizar el depósito');
        }
      });
    } else {
      this.http.post(this.apiUrl, transaccion).subscribe({
        next: () => {
          console.log(transaccion);
          alert('Depósito registrado correctamente');
          
          this.resetearFormulario();
          this.cargarDepositosPorMes(); 
        },
        error: err => {
          console.error('Error al guardar depósito', err);
          alert('Error al guardar el depósito');
        }
      });
    }
  }


  
  resetearFormulario() {
    this.encabezado = {
      fecha: new Date().toISOString().split('T')[0],
      remitente: ''
    };
    this.detalles = [];
    this.limpiarFormularioDetalle();
    this.calcularTotales();
    this.editandoDepositoId = null;
    this.editandoDetalleId = null;
  }

  getNombreFondo(id: number): string {
    const fondo = this.fondos.find(f => f.id === id);
    return fondo ? fondo.nombre : 'Desconocido';
  }
get nombreMesSeleccionado(): string {
  const mes = this.meses.find(m => m.id === this.mesSeleccionado);
  return mes ? mes.nombre : '';
}




get textoBotonDetalle(): string {
  return this.editandoDetalleId !== null ? 'Actualizar' : 'Agregar';
}
  getNombreMetodoPago(id: string): string {
    const metodo = this.metodosPago.find(m => m.id === id);
    return metodo ? metodo.nombre : 'Desconocido';
  }
}

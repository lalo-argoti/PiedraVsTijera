import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

interface FondoMonetarioDto {
  id: number;
  nombre: string;
}

interface GastoTipoDto {
  id: number;
  nombre: string;
}

@Component({
  selector: 'app-gastos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gastos.component.html',
  styleUrls: ['./gastos.component.css']
})
export class GastosComponent implements OnInit {
  fondos: FondoMonetarioDto[] = [];
  tiposGasto: GastoTipoDto[] = [];
  gastos: any[] = [];
  editandoDetalleId: number | null = null;

  meses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' }, { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' }, { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' }, { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
  ];

  anios = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - i);
  mesSeleccionado = new Date().getMonth() + 1;
  anioSeleccionado = new Date().getFullYear();

  nuevoGasto = {
    fecha: new Date().toISOString().split('T')[0],
    tipoGastoId: 0,
    fondoId: 0,
    montoCOP: 0,
    montoUSD: 0,
    descripcion: '',
    detalleId: null,           // 🆕 nuevo
    gastoRegistroId: null      // 🆕 nuevo
  };
  totalGastadoCOP = 0;
  totalGastadoUSD = 0;
  editandoId: number | null = null;

  private apiUrl = `${environment.apiUrl}/api/gastoregistro`;
  private initUrl = `${environment.apiUrl}/api/presupuestomovimiento/init`;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.cargarDatosIniciales();
    this.cargarGastos();
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

  cargarGastos() {
    const params = {
      mes: this.mesSeleccionado.toString(),
      anio: this.anioSeleccionado.toString()
    };

this.http.get<any[]>(`${this.apiUrl}`, { params }).subscribe({
  next: (data) => {
    this.gastos = data;
    this.calcularTotales();
      console.log('📡 Llamado a:', `${this.apiUrl}`, { params });  // 👈 Aquí se muestra la URL en consola

    console.log(data)
    
  },
  
  error: (err) => console.error('Error al cargar gastos', err)
});

  }

  calcularTotales() {
    this.totalGastadoCOP = this.gastos.reduce((sum, gasto) => sum + (gasto.montoCOP || 0), 0);
    this.totalGastadoUSD = this.gastos.reduce((sum, gasto) => sum + (gasto.montoUSD || 0), 0);
  }

  onSubmit() {
    const gasto: any = {
      fecha: this.nuevoGasto.fecha,
      fondoId: Number(this.nuevoGasto.fondoId),
      observaciones: this.nuevoGasto.descripcion || '',
      detalles: [
        {
          gastoTipoId: Number(this.nuevoGasto.tipoGastoId),
          montoCOP: Number(this.nuevoGasto.montoCOP),
          montoUSD: Number(this.nuevoGasto.montoUSD)
        }
      ]
    };

    if (this.editandoId) {
      this.actualizarGasto(gasto);
    } else {
      this.crearGasto(gasto);
    }
  }

  private crearGasto(gastoData: any) {
    const payload = {
      fecha: gastoData.fecha,
      observaciones: gastoData.observaciones || '',
      fondoId: Number(gastoData.fondoId),
      detalles: [
        {
           id: this.editandoDetalleId, //  
          gastoTipoId: Number(gastoData.detalles[0].gastoTipoId),
          montoCOP: Number(gastoData.detalles[0].montoCOP),
          montoUSD: Number(gastoData.detalles[0].montoUSD)
        }
      ]
    };
    console.log("enviando a la url" , this.apiUrl);
    console.log('📤 Payload enviado al backend:', payload);

    this.http.post(this.apiUrl, payload).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      
      },
      error: (err) => {
        console.error('❌ Error al crear gasto', err, payload);
        alert(err.error?.mensaje || 'Error desconocido al guardar');
      }
    });
  }

actualizarGasto(_: any) {
  const gastoRegistroDto = {
    id: this.nuevoGasto.gastoRegistroId,
    fecha: this.nuevoGasto.fecha,
    observaciones: this.nuevoGasto.descripcion || '',
    fondoId: Number(this.nuevoGasto.fondoId),
    criterio: 'manual',
    detalles: [
      {
        id: this.nuevoGasto.detalleId,
        gastoRegistroId: this.nuevoGasto.gastoRegistroId,
        gastoTipoId: Number(this.nuevoGasto.tipoGastoId),
        montoCOP: Number(this.nuevoGasto.montoCOP),
        montoUSD: Number(this.nuevoGasto.montoUSD)
      }
    ]
  };
    console.log('➡️ Payload PUT:', gastoRegistroDto);
          alert('Actualizado correctamente');

    this.http.put(`${this.apiUrl}/${this.editandoId}`, gastoRegistroDto).subscribe({
        next: () => {
        alert('Actualizado correctamente');
        this.editandoId = null;
        this.editandoDetalleId = null;  // Limpias el id del detalle también
        this.cargarGastos();
        this.limpiarFormulario();
        },
        error: (err) => {
        console.error('Error al actualizar gasto', err);
        alert(err.error?.mensaje || 'Error al actualizar');
        }
    });
    }
editarGasto(gasto: any) {
  const detalle = gasto.detalles?.[0];
  console.log('Junio72025:',detalle);
  
  /*Si ya estan aqui ¿porque no se actualizan?*/
  /* es decir, ya estan en manos del forntend, */
  
  this.editandoId = gasto.id;
  this.editandoDetalleId = detalle.id || null;

  this.nuevoGasto = {
    fecha: gasto.fecha?.split('T')[0] || '',
    tipoGastoId: detalle?.gastoTipoId || 0,
    fondoId: gasto.fondoId || 0,
    montoCOP: detalle?.montoCOP || 0,
    montoUSD: detalle?.montoUSD || 0,
    descripcion: gasto.observaciones || '',
    detalleId: detalle?.id || null,           // 🆕
    gastoRegistroId: gasto.id                 // 🆕
  };

  console.log('Detalle a editar:', this.editandoDetalleId);
}

eliminarGasto(id: number) {
  if (confirm('¿Estás seguro de eliminar este gasto?')) {
    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => this.cargarGastos(),
      error: (err) => console.error('Error al eliminar gasto', err)
    });
  }
}

  cancelarEdicion() {
    this.editandoId = null;
    this.limpiarFormulario();
  }

limpiarFormulario() {
  this.nuevoGasto = {
    fecha: new Date().toISOString().split('T')[0],
    tipoGastoId: 0,
    fondoId: 0,
    montoCOP: 0,
    montoUSD: 0,
    descripcion: '',
    detalleId: null,
    gastoRegistroId: null
  };
}
  cambiarMes() {
    this.cargarGastos();
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

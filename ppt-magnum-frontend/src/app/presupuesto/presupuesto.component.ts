import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

// Interfaces
interface GastoTipoDto {
  id: number;
  nombre: string;
}

interface PresupuestoMovimiento {
  id?: number;
  gastoTipoId: number;
  montoPresupuestadoUSD: number;
  montoPresupuestadoCOP: number;
  mes: number;
  anio: number;
}

@Component({
  selector: 'app-presupuesto',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './presupuesto.component.html',
  styleUrls: ['./presupuesto.component.css']
})
export class PresupuestoComponent implements OnInit {
  tiposGasto: GastoTipoDto[] = [];
  gastos: PresupuestoMovimiento[] = [];

  meses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' }, { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' }, { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' }, { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
  ];

  anios = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - i);
  mesSeleccionado = new Date().getMonth() + 1;
  anioSeleccionado = new Date().getFullYear();

  nuevoGasto: PresupuestoMovimiento = {
    gastoTipoId: 0,
    montoPresupuestadoCOP: 0,
    montoPresupuestadoUSD: 0,
    mes: this.mesSeleccionado,
    anio: this.anioSeleccionado
  };

  totalGastadoCOP = 0;
  totalGastadoUSD = 0;
  editandoId: number | null = null;

  private apiUrl = `${environment.apiUrl}/api/gastos`;
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

    this.http.get<PresupuestoMovimiento[]>(this.apiUrl, { params }).subscribe({
      next: (data) => {
        this.gastos = data;
        this.calcularTotales();
      },
      error: (err) => console.error('Error al cargar gastos', err)
    });
  }

  calcularTotales() {
    this.totalGastadoCOP = this.gastos.reduce((sum, g) => sum + (g.montoPresupuestadoCOP || 0), 0);
    this.totalGastadoUSD = this.gastos.reduce((sum, g) => sum + (g.montoPresupuestadoUSD || 0), 0);
  }

  onSubmit() {
    this.nuevoGasto.mes = this.mesSeleccionado;
    this.nuevoGasto.anio = this.anioSeleccionado;

    if (this.editandoId) {
      this.actualizarGasto();
    } else {
      this.crearGasto();
    }
  }

  crearGasto() {
    console.log("119: ",this.nuevoGasto);

    this.http.post(this.apiUrl, this.nuevoGasto).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      },
      error: (err) => console.error('Error al crear gasto', err)
    });
  }

  actualizarGasto() {
    if (!this.editandoId) return;

    const gastoActualizado: PresupuestoMovimiento = {
      ...this.nuevoGasto,
      id: this.editandoId
    };

    this.http.put(`${this.apiUrl}/${this.editandoId}`, gastoActualizado).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      },
      error: (err) => console.error('Error al actualizar gasto', err)
    });
  }

  editarGasto(gasto: PresupuestoMovimiento) {
    this.editandoId = gasto.id!;
    this.nuevoGasto = {
      gastoTipoId: gasto.gastoTipoId,
      montoPresupuestadoCOP: gasto.montoPresupuestadoCOP,
      montoPresupuestadoUSD: gasto.montoPresupuestadoUSD,
      mes: gasto.mes,
      anio: gasto.anio
    };
  }

  eliminarGasto(id: number) {
    if (confirm('¿Estás seguro de eliminar este presupuesto?')) {
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
      gastoTipoId: 0,
      montoPresupuestadoCOP: 0,
      montoPresupuestadoUSD: 0,
      mes: this.mesSeleccionado,
      anio: this.anioSeleccionado
    };
  }

  cambiarMes() {
    this.cargarGastos();
  }

  getNombreTipoGasto(id: number): string {
    const tipo = this.tiposGasto.find(t => t.id === id);
    return tipo ? tipo.nombre : 'Desconocido';
  }
}

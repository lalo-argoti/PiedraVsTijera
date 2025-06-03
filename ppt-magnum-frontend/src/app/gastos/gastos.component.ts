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
    tipoGastoId: '',
    fondoId: '',
    montoCOP: 0,
    montoUSD: 0,
    descripcion: ''
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

  cargarDatosIniciales() {
    this.http.get<any>(this.initUrl).subscribe({
      next: (data) => {
        this.fondos = data.fondos;
        this.tiposGasto = data.tiposGasto;
      },
      error: (err) => console.error('Error al cargar datos iniciales', err)
    });
  }

  cargarGastos() {
    const params = {
      mes: this.mesSeleccionado.toString(),
      anio: this.anioSeleccionado.toString()
    };

    this.http.get<any[]>(this.apiUrl, { params }).subscribe({
      next: (data) => {
        this.gastos = data;
        this.calcularTotales();
      },
      error: (err) => console.error('Error al cargar gastos', err)
    });
  }

  calcularTotales() {
    this.totalGastadoCOP = this.gastos.reduce((sum, gasto) => sum + (gasto.montoCOP || 0), 0);
    this.totalGastadoUSD = this.gastos.reduce((sum, gasto) => sum + (gasto.montoUSD || 0), 0);
  }

  onSubmit() {
    const gastoData = {
      ...this.nuevoGasto,
      mes: this.mesSeleccionado,
      anio: this.anioSeleccionado
    };

    if (this.editandoId !== null) {
      this.actualizarGasto(gastoData);
    } else {
      this.crearGasto(gastoData);
    }
  }

  private crearGasto(gastoData: any) {
    this.http.post(this.apiUrl, gastoData).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      },
      error: (err) => console.error('Error al crear gasto', err)
    });
  }

  private actualizarGasto(gastoData: any) {
    this.http.put(`${this.apiUrl}/${this.editandoId}`, gastoData).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      },
      error: (err) => console.error('Error al actualizar gasto', err)
    });
  }

  editarGasto(gasto: any) {
    this.editandoId = gasto.id;
    this.nuevoGasto = {
      fecha: gasto.fecha.split('T')[0],
      tipoGastoId: gasto.tipoGastoId,
      fondoId: gasto.fondoId,
      montoCOP: gasto.montoCOP,
      montoUSD: gasto.montoUSD,
      descripcion: gasto.descripcion
    };
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
      tipoGastoId: '',
      fondoId: '',
      montoCOP: 0,
      montoUSD: 0,
      descripcion: ''
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

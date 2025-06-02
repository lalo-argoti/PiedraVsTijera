import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-presupuesto',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './presupuesto.component.html',
  styleUrls: ['./presupuesto.component.css']
})
export class PresupuestoComponent implements OnInit {
  // Lista de meses para el selector
  meses = [
    { id: 1, nombre: 'Enero' },
    { id: 2, nombre: 'Febrero' },
    { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' },
    { id: 5, nombre: 'Mayo' },
    { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' },
    { id: 8, nombre: 'Agosto' },
    { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' },
    { id: 11, nombre: 'Noviembre' },
    { id: 12, nombre: 'Diciembre' }
  ];

  // Genera los últimos 10 años para el selector
  anios = Array.from({length: 10}, (_, i) => new Date().getFullYear() - i);

  // Datos del componente
  gastos: any[] = [];
  tiposGasto: any[] = [];
  fondos: any[] = [];

  // Valores seleccionados
  mesSeleccionado: number = new Date().getMonth() + 1;
  anioSeleccionado: number = new Date().getFullYear();

  // Modelo para nuevo gasto
  nuevoGasto = {
    fecha: new Date().toISOString().split('T')[0],
    tipoGastoId: '',
    fondoId: '',
    montoCOP: 0,
    montoUSD: 0,
    descripcion: ''
  };

  // Totales calculados
  totalGastadoCOP = 0;
  totalGastadoUSD = 0;

  // Control de edición
  editandoId: number | null = null;

  // URL de la API (debes reemplazarla con tu endpoint real)
  private apiUrl = '${environment.apiUrl}/api/gastos';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.cargarDatosIniciales();
    this.cargarGastos();
  }

  // Carga los tipos de gasto y fondos disponibles
  cargarDatosIniciales() {
    this.http.get<any[]>('${environment.apiUrl}/api/tipos-gasto').subscribe({
      next: (data) => this.tiposGasto = data,
      error: (err) => console.error('Error al cargar tipos de gasto', err)
    });

    this.http.get<any[]>('${environment.apiUrl}/api/fondos').subscribe({
      next: (data) => this.fondos = data,
      error: (err) => console.error('Error al cargar fondos', err)
    });
  }

  // Carga los gastos del mes/año seleccionado
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

  // Calcula los totales gastados
  calcularTotales() {
    this.totalGastadoCOP = this.gastos.reduce((sum, gasto) => sum + (gasto.montoCOP || 0), 0);
    this.totalGastadoUSD = this.gastos.reduce((sum, gasto) => sum + (gasto.montoUSD || 0), 0);
  }

  // Maneja el envío del formulario
  onSubmit() {
    const gastoData = {
      ...this.nuevoGasto,
      mes: this.mesSeleccionado,
      anio: this.anioSeleccionado
    };

    if (this.editandoId) {
      this.actualizarGasto(gastoData);
    } else {
      this.crearGasto(gastoData);
    }
  }

  // Crea un nuevo gasto
  private crearGasto(gastoData: any) {
    this.http.post(this.apiUrl, gastoData).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      },
      error: (err) => console.error('Error al crear gasto', err)
    });
  }

  // Actualiza un gasto existente
  private actualizarGasto(gastoData: any) {
    this.http.put(`${this.apiUrl}/${this.editandoId}`, gastoData).subscribe({
      next: () => {
        this.cargarGastos();
        this.limpiarFormulario();
      },
      error: (err) => console.error('Error al actualizar gasto', err)
    });
  }

  // Prepara el formulario para edición
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

  // Elimina un gasto
  eliminarGasto(id: number) {
    if (confirm('¿Estás seguro de eliminar este gasto?')) {
      this.http.delete(`${this.apiUrl}/${id}`).subscribe({
        next: () => this.cargarGastos(),
        error: (err) => console.error('Error al eliminar gasto', err)
      });
    }
  }

  // Cancela la edición
  cancelarEdicion() {
    this.editandoId = null;
    this.limpiarFormulario();
  }

  // Limpia el formulario
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

  // Maneja el cambio de mes/año
  cambiarMes() {
    this.cargarGastos();
  }

  // Helper para obtener nombre de tipo de gasto
  getNombreTipoGasto(id: number): string {
    const tipo = this.tiposGasto.find(t => t.id === id);
    return tipo ? tipo.nombre : 'Desconocido';
  }

  // Helper para obtener nombre de fondo
  getNombreFondo(id: number): string {
    const fondo = this.fondos.find(f => f.id === id);
    return fondo ? fondo.nombre : 'Desconocido';
  }
}

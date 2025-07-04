import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration, ChartType } from 'chart.js';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-grafico',
  standalone: true,
  imports: [CommonModule, FormsModule, NgChartsModule],
  templateUrl: './grafico.component.html',
  styleUrls: ['./grafico.component.css']
})
export class GraficoComponent implements OnInit {
  // Opciones gráfico COP
  public barChartOptionsCOP: ChartConfiguration['options'] = {
    responsive: true,
    scales: {
      x: { stacked: false },
      y: {
        stacked: false,
        beginAtZero: true,
        title: { display: true, text: 'Montos (COP)' },
        ticks: {
          callback: (value: string | number) => {
            if (typeof value === 'number') {
              return new Intl.NumberFormat('es-CO', {
                style: 'currency',
                currency: 'COP',
                minimumFractionDigits: 0
              }).format(value);
            }
            return value;
          }
        }
      }
    },
    plugins: {
      title: {
        display: true,
        text: 'Presupuesto vs. Ejecución por Tipo de Gasto (COP)',
        font: { size: 16 }
      },
      legend: { position: 'top' },
      tooltip: {
        callbacks: {
          label: (context: any) => {
            let label = context.dataset.label || '';
            if (label) label += ': ';
            if (context.raw !== null) {
              label += new Intl.NumberFormat('es-CO', {
                style: 'currency',
                currency: 'COP',
                minimumFractionDigits: 0
              }).format(Number(context.raw));
            }
            return label;
          }
        }
      }
    }
  };

  // Opciones gráfico USD
  public barChartOptionsUSD: ChartConfiguration['options'] = {
    responsive: true,
    scales: {
      x: { stacked: false },
      y: {
        stacked: false,
        beginAtZero: true,
        title: { display: true, text: 'Montos (USD)' },
        ticks: {
          callback: (value: string | number) => {
            if (typeof value === 'number') {
              return new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD',
                minimumFractionDigits: 0
              }).format(value);
            }
            return value;
          }
        }
      }
    },
    plugins: {
      title: {
        display: true,
        text: 'Presupuesto vs. Ejecución por Tipo de Gasto (USD)',
        font: { size: 16 }
      },
      legend: { position: 'top' },
      tooltip: {
        callbacks: {
          label: (context: any) => {
            let label = context.dataset.label || '';
            if (label) label += ': ';
            if (context.raw !== null) {
              label += new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD',
                minimumFractionDigits: 0
              }).format(Number(context.raw));
            }
            return label;
          }
        }
      }
    }
  };

  // Variables para el gráfico COP
  public barChartLabels: string[] = [];
  public barChartType: ChartType = 'bar';
  public barChartLegend = true;
  public barChartData: ChartConfiguration['data'] = { labels: [], datasets: [] };

  // Variables para el gráfico USD
  public barChartUSDLabels: string[] = [];
  public barChartUSDData: ChartConfiguration['data'] = { labels: [], datasets: [] };

  // Filtros para el formulario
  meses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' }, { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' }, { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' }, { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
  ];
  anios = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - i);

  mesSeleccionado: number = new Date().getMonth() + 1;
  anioSeleccionado: number = new Date().getFullYear();
  tipoGastoIdSeleccionado: number | null = null;

  tiposGasto: any[] = [];
  cargando = false;
  error = '';
  private initUrl = `${environment.apiUrl}/api/presupuestomovimiento/init`;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.cargarDatosIniciales()
      .then(() => {
        // Puedes llamar a consultarDatos() aquí para cargar los gráficos iniciales
      })
      .catch(err => {
        this.error = 'Error al cargar datos iniciales';
        console.error(err);
      });
  }

  cargarDatosIniciales(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.http.get<any>(this.initUrl).subscribe({
        next: (data) => {
          this.tiposGasto = data.tiposGasto || [];
          resolve();
        },
        error: (err) => {
          console.error('Error al cargar datos iniciales', err);
          reject(err);
        }
      });
    });
  }

  consultarDatos() {
    if (!this.mesSeleccionado || !this.anioSeleccionado || !this.tipoGastoIdSeleccionado) {
      this.error = 'Debe seleccionar mes, año y tipo de gasto';
      return;
    }

    this.cargando = true;
    this.error = '';

    const params = {
      mes: this.mesSeleccionado.toString(),
      anio: this.anioSeleccionado.toString(),
      tipoGastoId: this.tipoGastoIdSeleccionado.toString()
    };

    this.http.get<any>(`${environment.apiUrl}/api/PresupuestoMovimiento/ejecucion`, { params }).subscribe({
      next: (data) => {
        this.procesarDatos(data);
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error al consultar datos:', err);
        this.error = 'Error al cargar los datos para el gráfico';
        this.cargando = false;
      }
    });
  }

  procesarDatos(data: any) {
    const tipo = this.tiposGasto.find(t => t.id === this.tipoGastoIdSeleccionado);
    const label = tipo ? tipo.nombre : 'Tipo de Gasto';

    // Datos COP
    const presupuestadoCOP = data.presupuestado?.montoCOP || 0;
    const ejecutadoCOP = data.ejecutado?.montoCOP || 0;
    const diferenciaCOP = presupuestadoCOP - ejecutadoCOP;

    // Datos USD
    const presupuestadoUSD = data.presupuestado?.montoUSD || 0;
    const ejecutadoUSD = data.ejecutado?.montoUSD || 0;
    const diferenciaUSD = presupuestadoUSD - ejecutadoUSD;

    // Labels para ambos gráficos (igual etiqueta)
    this.barChartLabels = [label];
    this.barChartUSDLabels = [label];

    // Asignar datos al gráfico COP
    this.barChartData = {
      labels: this.barChartLabels,
      datasets: [
        { label: 'Presupuestado', data: [presupuestadoCOP], backgroundColor: '#3b82f6' },
        { label: 'Ejecutado', data: [ejecutadoCOP], backgroundColor: '#10b981' },
        { label: 'Diferencia', data: [diferenciaCOP], backgroundColor: '#f59e0b' }
      ]
    };

    // Asignar datos al gráfico USD
    this.barChartUSDData = {
      labels: this.barChartUSDLabels,
      datasets: [
        { label: 'Presupuestado', data: [presupuestadoUSD], backgroundColor: '#3b82f6' },
        { label: 'Ejecutado', data: [ejecutadoUSD], backgroundColor: '#10b981' },
        { label: 'Diferencia', data: [diferenciaUSD], backgroundColor: '#f59e0b' }
      ]
    };
  }
}

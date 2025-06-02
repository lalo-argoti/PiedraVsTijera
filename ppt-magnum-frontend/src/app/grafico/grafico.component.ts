import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgChartsModule } from 'ng2-charts';
import { ChartConfiguration, ChartType } from 'chart.js';

@Component({
  selector: 'app-grafico',
  standalone: true,
  imports: [CommonModule, FormsModule, NgChartsModule],
  templateUrl: './grafico.component.html',
  styleUrls: ['./grafico.component.css']
})
export class GraficoComponent implements OnInit {
  // Configuración del gráfico con tipos correctos
  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    scales: {
      x: {
        stacked: true,
      },
      y: {
        stacked: false,
        beginAtZero: true,
        title: {
          display: true,
          text: 'Montos (COP)'
        },
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
        text: 'Presupuesto vs. Ejecución por Tipo de Gasto',
        font: {
          size: 16
        }
      },
      legend: {
        position: 'top',
      },
      tooltip: {
        callbacks: {
          label: (context: any) => {
            let label = context.dataset.label || '';
            if (label) {
              label += ': ';
            }
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

  public barChartLabels: string[] = [];
  public barChartType: ChartType = 'bar';
  public barChartLegend = true;
  public barChartData: ChartConfiguration['data'] = {
    labels: this.barChartLabels,
    datasets: []
  };

  // Filtros
  fechaInicio: string;
  fechaFin: string;
  cargando: boolean = false;
  error: string = '';

  // Datos
  tiposGasto: any[] = [];

  constructor(private http: HttpClient) {
    const hoy = new Date();
    const primerDiaMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
    
    this.fechaInicio = this.formatDate(primerDiaMes);
    this.fechaFin = this.formatDate(hoy);
  }

  ngOnInit(): void {
    this.cargarTiposGasto();
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  cargarTiposGasto() {
    this.http.get<any[]>('http://172.20.10.3:8000/api/tipos-gasto')
      .subscribe({
        next: (data) => {
          this.tiposGasto = data;
          this.consultarDatos();
        },
        error: (err) => {
          console.error('Error al cargar tipos de gasto:', err);
          this.error = 'Error al cargar los tipos de gasto';
        }
      });
  }

  consultarDatos() {
    if (!this.fechaInicio || !this.fechaFin) {
      this.error = 'Debe seleccionar ambas fechas';
      return;
    }

    this.cargando = true;
    this.error = '';
    
    const params = {
      fecha_inicio: this.fechaInicio,
      fecha_fin: this.fechaFin
    };

    this.http.get<any>('http://172.20.10.3:8000/api/presupuesto-ejecucion', { params })
      .subscribe({
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
    this.barChartLabels = this.tiposGasto.map(tipo => tipo.nombre);
    
    this.barChartData = {
      labels: this.barChartLabels,
      datasets: [
        {
          label: 'Presupuestado',
          data: this.tiposGasto.map(tipo => 
            data.presupuestado.find((item: any) => item.tipoGastoId === tipo.id)?.montoCOP || 0
          ),
          backgroundColor: '#3b82f6',
          hoverBackgroundColor: '#2563eb'
        },
        {
          label: 'Ejecutado',
          data: this.tiposGasto.map(tipo => 
            data.ejecutado.find((item: any) => item.tipoGastoId === tipo.id)?.montoCOP || 0
          ),
          backgroundColor: '#10b981',
          hoverBackgroundColor: '#059669'
        },
        {
          label: 'Diferencia',
          data: this.tiposGasto.map(tipo => {
            const presupuesto = data.presupuestado.find((item: any) => item.tipoGastoId === tipo.id)?.montoCOP || 0;
            const ejecutado = data.ejecutado.find((item: any) => item.tipoGastoId === tipo.id)?.montoCOP || 0;
            return presupuesto - ejecutado;
          }),
          backgroundColor: '#f59e0b',
          hoverBackgroundColor: '#d97706'
        }
      ]
    };
  }
}

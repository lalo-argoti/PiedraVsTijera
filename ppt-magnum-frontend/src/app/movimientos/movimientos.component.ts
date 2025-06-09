import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { environment } from '../../environments/environment';

interface Movimiento {
  tipo: 'deposito' | 'gasto';
  fecha: string;
  montoCOP: number;
  montoUSD: number;
  descripcion: string;
  referencia?: string; 
   tipoGasto?: string; 
   fondo?: number;
   metodoPago?: string;
}

@Component({
  selector: 'app-movimientos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './movimientos.component.html',
  styleUrls: ['./movimientos.component.css'],
  providers: [DatePipe]
})
export class MovimientosComponent implements OnInit {
  movimientos: Movimiento[] = [];
  fechaInicio: string;
  fechaFin: string;
  cargando: boolean = false;
  error: string = '';
  totalDepositosCOP: number = 0;
  totalDepositosUSD: number = 0;
  totalGastosCOP: number = 0;
  totalGastosUSD: number = 0;

  constructor(private http: HttpClient, private datePipe: DatePipe) {
    const hoy = new Date();
    const primerDiaMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
    
    this.fechaInicio = this.datePipe.transform(primerDiaMes, 'yyyy-MM-dd') || '';
    this.fechaFin = this.datePipe.transform(hoy, 'yyyy-MM-dd') || '';
  }

  ngOnInit(): void {
    this.consultarMovimientos();
  }

  consultarMovimientos() {
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
  
    this.http.get<Movimiento[]>(`${environment.apiUrl}/api/PresupuestoMovimiento/balance`, { params })
      .subscribe({
        next: (data) => {
          this.movimientos = data;
          this.calcularTotales();
          this.cargando = false;
        },
        error: (err) => {
          console.error('Error al consultar movimientos:', err);
          this.error = 'Error al cargar los movimientos';
          this.cargando = false;
        }
      });
  }

  calcularTotales() {
    this.totalDepositosCOP = 0;
    this.totalDepositosUSD = 0;
    this.totalGastosCOP = 0;
    this.totalGastosUSD = 0;

    this.movimientos.forEach(mov => {
      if (mov.tipo === 'deposito') {
        this.totalDepositosCOP += mov.montoCOP || 0;
        this.totalDepositosUSD += mov.montoUSD || 0;
      } else if (mov.tipo === 'gasto') {
        this.totalGastosCOP += mov.montoCOP || 0;
        this.totalGastosUSD += mov.montoUSD || 0;
      }
    });
  }

  getColorMovimiento(tipo: string) {
    return tipo === 'deposito' ? 'text-success' : 'text-danger';
  }

  getIconoMovimiento(tipo: string) {
    return tipo === 'deposito' ? '↑' : '↓';
  }
}

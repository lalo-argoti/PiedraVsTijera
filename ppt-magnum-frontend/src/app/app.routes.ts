import { Routes } from '@angular/router';
import { JugadorFormComponent } from './jugador-form/jugador-form.component';
import { CodigoFormComponent } from './codigo-form/codigo-form.component';
import { JuegoComponent } from './juego/juego.component';
import { TiposGastoComponent } from './tipos-gasto/tipos-gasto.component';
import { FondoMonetarioComponent } from './fondo-monetario/fondo-monetario.component';
import { PresupuestoComponent } from './presupuesto/presupuesto.component';
import { GastosComponent } from './gastos/gastos.component';
import { DepositosComponent } from './depositos/depositos.component';
import { MovimientosComponent } from './movimientos/movimientos.component';
import { GraficoComponent } from './grafico/grafico.component';

export const routes: Routes = [
  { path: '', redirectTo: '/jugador', pathMatch: 'full' },
  { path: 'jugador', component: JugadorFormComponent },
  { path: 'codigo', component: CodigoFormComponent },
  { path: 'juego', component: JuegoComponent },
  
  // Nuevas rutas para cada componente
  { path: 'tipos-gasto', component: TiposGastoComponent },
  { path: 'fondos', component: FondoMonetarioComponent },
  { path: 'presupuesto', component: PresupuestoComponent },
  { path: 'gastos', component: GastosComponent },
  { path: 'depositos', component: DepositosComponent },
  { path: 'movimientos', component: MovimientosComponent },
  { path: 'grafico', component: GraficoComponent }
];

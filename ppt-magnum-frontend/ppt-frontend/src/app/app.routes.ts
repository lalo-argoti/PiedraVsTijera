import { Routes } from '@angular/router';
import { JugadorFormComponent } from './jugador-form/jugador-form.component';
import { CodigoFormComponent } from './codigo-form/codigo-form.component';
import { JuegoComponent } from './juego/juego.component';

export const routes: Routes = [
  { path: '', redirectTo: '/jugador', pathMatch: 'full' },
  { path: 'jugador', component: JugadorFormComponent },
  { path: 'codigo', component: CodigoFormComponent },
  { path: 'juego', component: JuegoComponent },
];

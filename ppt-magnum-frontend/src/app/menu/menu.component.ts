import { Component } from '@angular/core';
import { RouterModule } from '@angular/router'; // si usas routerLink en plantilla
import { CommonModule } from '@angular/common'; // ✅ necesario para *ngFor y *ngIf
import { environment } from '../../environments/environment';


@Component({
  selector: 'app-menu',
  standalone: true,  
  imports: [CommonModule, RouterModule], // ✅ agrega CommonModule
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {
  menuItems = [
    {
      title: 'Mantenimientos',
      children: [
        { name: 'Tipos de Gasto', route: '/tipos-gasto' },
        { name: 'Fondo Monetario', route: '/fondos' }
      ],
      expanded: false
    },
    {
      title: 'Movimientos',
      children: [
        { name: 'Presupuesto por tipo de gasto', route: '/presupuesto' },
        { name: 'Registros de gastos', route: '/gastos' },
        { name: 'Depósitos', route: '/depositos' }
      ],
      expanded: false
    },
    {
      title: 'Consultas y Reportes',
      children: [
        { name: 'Consulta de movimientos', route: '/movimientos' },
        { name: 'Gráfico Comparativo de Presupuesto y Ejecución', route: '/grafico' }
      ],
      expanded: false
    }
  ];

  toggleSubmenu(item: any) {
    item.expanded = !item.expanded;
  }
}


import { Component, OnInit } from '@angular/core';
import { Router,ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-codigo-form',
  standalone: true,
  templateUrl: './codigo-form.component.html',
  styleUrls: ['./codigo-form.component.css'],
  imports: [CommonModule, FormsModule]
})
export class CodigoFormComponent implements OnInit {
  playerName: string ='';
  inputCode: string = '';  // Campo para el código ingresado
  generatedCode: string = '';  // Código generado por el sistema


  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit() {
    this.generatedCode = this.generateRandomCode(5);
    this.route.queryParams.subscribe(params => { this.playerName = params['name'] || null;});
  }

  generateRandomCode(length: number): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let code = '';
    for (let i = 0; i < length; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      code += chars[randomIndex];
    }
    return code;
  }

  onSubmit() {
    const codeToSubmit = this.inputCode || this.generatedCode;
    console.log('Código enviado:', codeToSubmit,'Jugador local:',  this.playerName);

    this.router.navigate(['/juego'], { queryParams: { code: codeToSubmit , playerName: this.playerName } });
  }

  useGeneratedCode() {
    console.log('Entrando con código generado:', this.generatedCode,'Jugador local:',  this.playerName );

    this.router.navigate(['/juego'], { queryParams: { code: "S"+this.generatedCode, playerName: this.playerName  }  });
  }

  clearInput() {
    this.inputCode = '';
  }
}

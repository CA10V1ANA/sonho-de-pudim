import { Component, HostListener, signal } from '@angular/core';
import { HeaderComponent } from './header.component';
import { RevealDirective } from './reveal.directive';
import { Highlight, Occasion } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HeaderComponent, RevealDirective],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  readonly year = new Date().getFullYear();
  readonly activeHighlight = signal(0);
  readonly pointerX = signal(0);
  readonly pointerY = signal(0);

  readonly highlights: readonly Highlight[] = [
    { number: '01', title: 'Textura delicada', text: 'Cremoso, leve e preparado para derreter na primeira colherada.' },
    { number: '02', title: 'Feito com carinho', text: 'Uma produção artesanal, cuidada em cada detalhe.' },
    { number: '03', title: 'Calda generosa', text: 'Brilho, sabor e a quantidade certa para completar o momento.' }
  ];

  readonly occasions: readonly Occasion[] = [
    { number: '01', title: 'Para celebrar', text: 'Um doce especial para aniversários, encontros e datas que merecem carinho.' },
    { number: '02', title: 'Para presentear', text: 'Um gesto bonito, delicado e cheio de afeto.', featured: true },
    { number: '03', title: 'Para revender', text: 'Leve o Sonho de Pudim ao seu negócio e converse com a gente sobre parceria.' }
  ];

  @HostListener('window:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (event.pointerType !== 'mouse') return;
    this.pointerX.set((event.clientX / innerWidth - 0.5) * 18);
    this.pointerY.set((event.clientY / innerHeight - 0.5) * 14);
  }

  selectHighlight(index: number): void { this.activeHighlight.set(index); }
}

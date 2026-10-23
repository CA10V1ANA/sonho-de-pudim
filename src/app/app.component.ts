import { Component, HostListener, inject, signal } from '@angular/core';
import { HeaderComponent } from './header.component';
import { RevealDirective } from './reveal.directive';
import { ContactModalComponent } from './contact-modal.component';
import { ContactService } from './contact.service';
import { IFOOD_URL } from './contact.config';
import { Highlight, Occasion, ProductSize, Review, StoreHour } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HeaderComponent, ContactModalComponent, RevealDirective],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  readonly contact = inject(ContactService);
  readonly ifoodUrl = IFOOD_URL;
  readonly year = new Date().getFullYear();
  readonly todayIndex = new Date().getDay();
  readonly pointerX = signal(0);
  readonly pointerY = signal(0);

  readonly sizes: readonly ProductSize[] = [
    {
      badge: 'Porção individual • 120 ml',
      title: 'Pudim Individual',
      serves: 'Para 1 pessoa • 120 ml',
      volume: '120 ml',
      options: ['Tradicional', 'Zero Lactose'],
      text: 'Na medida certa para matar a vontade de um doce artesanal a qualquer hora, com textura super cremosa e calda generosa.',
      image: 'assets/pudim-individual.png',
      alt: 'Pudim individual de 120 ml servido com calda de caramelo'
    },
    {
      badge: 'Para compartilhar • 500 ml',
      title: 'Pudim Tamanho Família',
      serves: 'Serve até 8 fatias • 500 ml',
      volume: '500 ml',
      options: ['Tradicional', 'Zero Lactose'],
      text: 'Perfeito para dividir com quem você ama no almoço em família, encontros especiais e comemorações.',
      image: 'assets/pudim-familia.png',
      alt: 'Pudim tamanho família de 500 ml para até 8 fatias na embalagem Sonho de Pudim'
    }
  ];

  readonly highlights: readonly Highlight[] = [
    { title: 'Textura delicada', text: 'Cremoso, lisinho e preparado para derreter na primeira colherada.' },
    { title: 'Zero lactose disponível', text: 'Todo o sabor e cremosidade na versão tradicional ou zero lactose.' },
    { title: 'Calda generosa', text: 'Brilho dourado, sabor marcante e a quantidade certa de caramelo.' }
  ];

  readonly occasions: readonly Occasion[] = [
    { title: 'Para celebrar', text: 'Um doce especial para aniversários, encontros e datas que merecem carinho.' },
    { title: 'Para presentear', text: 'Um gesto bonito, delicado e cheio de afeto.', featured: true },
    { title: 'Para revender', text: 'Leve o Sonho de Pudim ao seu negócio e converse com a gente sobre parceria.' }
  ];

  readonly reviews: readonly Review[] = [
    {
      quote: 'Simplesmente perfeito! Super lisinho, sem furinhos e com muita calda de caramelo. Todo mundo aqui em casa amou.',
      author: 'Cliente no iFood',
      tag: 'Pudim 500 ml • Tamanho Família'
    },
    {
      quote: 'A opção zero lactose é maravilhosa, não perde nada em cremosidade e sabor. Chegou super rápido e bem embalado!',
      author: 'Cliente em Horizonte - CE',
      tag: 'Pudim Zero Lactose'
    },
    {
      quote: 'Encomendei para o aniversário da minha mãe e foi o maior sucesso da festa. Apresentação impecável!',
      author: 'Encomenda de Evento',
      tag: 'Pudim 500 ml'
    }
  ];

  readonly storeHours: readonly StoreHour[] = [
    { day: 'Segunda-feira', hours: '08:00 às 17:00', dayIndex: 1 },
    { day: 'Terça-feira', hours: '08:00 às 17:00', dayIndex: 2 },
    { day: 'Quarta-feira', hours: '11:00 às 15:00', dayIndex: 3 },
    { day: 'Quinta-feira', hours: '08:00 às 17:00', dayIndex: 4 },
    { day: 'Sexta-feira', hours: '08:00 às 17:00', dayIndex: 5 },
    { day: 'Sábado', hours: '08:00 às 17:00', dayIndex: 6 },
    { day: 'Domingo', hours: '09:00 às 14:00', dayIndex: 0 }
  ];

  @HostListener('window:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (event.pointerType !== 'mouse') return;
    this.pointerX.set((event.clientX / innerWidth - 0.5) * 18);
    this.pointerY.set((event.clientY / innerHeight - 0.5) * 14);
  }
}

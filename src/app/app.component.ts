import { Component, HostListener, inject, signal } from '@angular/core';
import { HeaderComponent } from './header.component';
import { RevealDirective } from './reveal.directive';
import { ContactModalComponent } from './contact-modal.component';
import { ContactService } from './contact.service';
import { IFOOD_URL } from './contact.config';
import { Highlight, Occasion, ProductSize, Review } from './models';

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
  readonly pointerX = signal(0);
  readonly pointerY = signal(0);

  readonly sizes: readonly ProductSize[] = [
    {
      badge: 'Dose individual',
      title: 'Pudim Individual',
      serves: 'Para 1 pessoa',
      text: 'Na medida certa para matar a vontade de um doce artesanal a qualquer hora, com textura super cremosa e calda generosa.',
      image: 'assets/pudim-individual.png',
      alt: 'Pudim individual artesanal servido com calda de caramelo'
    },
    {
      badge: 'Para compartilhar',
      title: 'Pudim Tamanho Família',
      serves: 'Serve até 8 pessoas (8 fatias)',
      text: 'Perfeito para dividir com quem você ama no almoço em família, encontros especiais e comemorações.',
      image: 'assets/pudim-familia.png',
      alt: 'Pudim tamanho família para até 8 fatias na embalagem Sonho de Pudim'
    }
  ];

  readonly highlights: readonly Highlight[] = [
    { number: '01', title: 'Textura delicada', text: 'Cremoso, lisinho e preparado para derreter na primeira colherada.' },
    { number: '02', title: 'Feito com carinho', text: 'Uma produção 100% artesanal, cuidada de perto em cada detalhe.' },
    { number: '03', title: 'Calda generosa', text: 'Brilho dourado, sabor marcante e a quantidade certa de caramelo.' }
  ];

  readonly occasions: readonly Occasion[] = [
    { number: '01', title: 'Para celebrar', text: 'Um doce especial para aniversários, encontros e datas que merecem carinho.' },
    { number: '02', title: 'Para presentear', text: 'Um gesto bonito, delicado e cheio de afeto.', featured: true },
    { number: '03', title: 'Para revender', text: 'Leve o Sonho de Pudim ao seu negócio e converse com a gente sobre parceria.' }
  ];

  readonly reviews: readonly Review[] = [
    {
      quote: 'Simplesmente perfeito! Super lisinho, sem furinhos e com muita calda de caramelo. Todo mundo aqui em casa amou.',
      author: 'Cliente no iFood',
      tag: 'Pudim Tamanho Família'
    },
    {
      quote: 'Pedi o individual pra provar e já virou vício. Chegou super rápido e bem embalado, dá pra sentir o capricho!',
      author: 'Cliente em Horizonte - CE',
      tag: 'Pudim Individual'
    },
    {
      quote: 'Encomendei para o aniversário da minha mãe e foi o maior sucesso da festa. Apresentação impecável!',
      author: 'Encomenda de Evento',
      tag: 'Tamanho Família'
    }
  ];

  @HostListener('window:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (event.pointerType !== 'mouse') return;
    this.pointerX.set((event.clientX / innerWidth - 0.5) * 18);
    this.pointerY.set((event.clientY / innerHeight - 0.5) * 14);
  }
}

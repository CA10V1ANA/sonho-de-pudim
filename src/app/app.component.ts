import { Component, HostListener, inject, signal } from '@angular/core';
import { HeaderComponent } from './header.component';
import { RevealDirective } from './reveal.directive';
import { ContactModalComponent } from './contact-modal.component';
import { ContactService } from './contact.service';
import { IFOOD_URL } from './contact.config';
import { Occasion, ProductOption, ProductSize, Review } from './models';

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

  private readonly productOptions: readonly ProductOption[] = [
    { label: 'Tradicional', icon: '🍮' },
    { label: 'Zero Lactose', icon: '✦', variant: 'zero' },
    { label: 'Café', icon: '☕', variant: 'coffee' }
  ];

  readonly sizes: readonly ProductSize[] = [
    {
      badge: 'Porção individual • 120 ml',
      title: 'Pudim Individual',
      serves: 'Para 1 pessoa • 120 ml',
      volume: '120 ml',
      options: this.productOptions,
      text: 'Na medida certa para matar a vontade de um doce artesanal a qualquer hora, com textura super cremosa e calda generosa.',
      image: 'assets/pudim-individual.png',
      alt: 'Pudim individual de 120 ml servido com calda de caramelo'
    },
    {
      badge: 'Para compartilhar • 500 ml',
      title: 'Pudim Tamanho Família',
      serves: 'Serve até 8 fatias • 500 ml',
      volume: '500 ml',
      options: this.productOptions,
      text: 'Perfeito para dividir com quem você ama no almoço em família, encontros especiais e comemorações.',
      image: 'assets/pudim-familia.png',
      alt: 'Pudim tamanho família de 500 ml para até 8 fatias na embalagem Sonho de Pudim'
    }
  ];

  readonly occasions: readonly Occasion[] = [
    { title: 'Para celebrar', text: 'Um doce especial para aniversários, encontros e datas que merecem carinho.', step: 'evento' },
    { title: 'Para presentear', text: 'Um gesto bonito, delicado e cheio de afeto.', step: 'menu', featured: true },
    { title: 'Para revender', text: 'Leve o Sonho de Pudim ao seu negócio e converse com a gente sobre parceria.', step: 'revenda' }
  ];

  readonly reviews: readonly Review[] = [
    {
      quote: 'Simplesmente perfeito! Super lisinho, sem furinhos e com muita calda de caramelo. Todo mundo aqui em casa amou.',
      author: 'Cliente no iFood',
      tag: 'Pudim Tamanho Família • 500 ml'
    },
    {
      quote: 'A opção zero lactose é maravilhosa, não perde nada em cremosidade e sabor. Chegou super rápido e bem embalado!',
      author: 'Cliente em Horizonte - CE',
      tag: 'Pudim Zero Lactose • 120 ml'
    },
    {
      quote: 'Encomendei para o aniversário da minha mãe e foi o maior sucesso da festa. Apresentação impecável!',
      author: 'Encomenda para Evento',
      tag: 'Pudim Tamanho Família • 500 ml'
    }
  ];

  @HostListener('window:pointermove', ['$event'])
  onPointerMove(event: PointerEvent): void {
    if (event.pointerType !== 'mouse') return;
    this.pointerX.set((event.clientX / innerWidth - 0.5) * 18);
    this.pointerY.set((event.clientY / innerHeight - 0.5) * 14);
  }
}

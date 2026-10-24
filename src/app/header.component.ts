import { Component, inject, signal } from '@angular/core';

import { ContactService } from './contact.service';

@Component({
  selector: 'app-header',
  standalone: true,
  styleUrl: './header.component.scss',
  template: `
    <div class="caramel-ribbon" aria-hidden="true"><i></i><i></i><i></i><i></i></div>
    <header class="nav-shell">
      <nav class="nav" aria-label="Navegação principal">
        <a class="brand" href="#inicio" aria-label="Sonho de Pudim — início" (click)="close()">
          <img src="assets/logo.png" alt=""><span>Sonho de Pudim</span>
        </a>
        <button class="menu" type="button" aria-label="Abrir menu" [attr.aria-expanded]="menuOpen()" (click)="toggle()">
          <span></span><span></span>
        </button>
        <div class="links" [class.open]="menuOpen()">
          <a href="#pudins" (click)="close()">O nosso pudim</a>
          <a href="#experiencia" (click)="close()">Momentos</a>
          <button class="nav-link" type="button" (click)="close(); contact.open('horarios')">Horários</button>
          <button class="button button--small" type="button" (click)="close(); contact.open()">Fazer encomenda</button>
        </div>
      </nav>
    </header>
  `
})
export class HeaderComponent {
  readonly contact = inject(ContactService);
  readonly menuOpen = signal(false);
  toggle(): void { this.menuOpen.update(value => !value); }
  close(): void { this.menuOpen.set(false); }
}

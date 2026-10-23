import { Component, HostListener, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ContactService } from './contact.service';
import { IFOOD_URL, whatsappLink } from './contact.config';

type Step = 'menu' | 'evento' | 'revenda';

@Component({
  selector: 'app-contact-modal',
  standalone: true,
  imports: [FormsModule],
  styleUrl: './contact-modal.component.scss',
  template: `
    @if (contact.isOpen()) {
      <div class="backdrop" (click)="contact.close()">
        <div class="modal" role="dialog" aria-modal="true" aria-labelledby="modal-title" (click)="$event.stopPropagation()">
          <button class="close" type="button" aria-label="Fechar" (click)="contact.close()">×</button>

          @switch (step()) {
            @case ('menu') {
              <span class="eyebrow">Fale com a gente</span>
              <h2 id="modal-title">Como podemos <em>ajudar?</em></h2>
              <p class="lead">Escolha o motivo do seu contato.</p>
              <div class="options">
                @if (ifoodUrl) {
                  <a class="option" [href]="ifoodUrl" target="_blank" rel="noreferrer">
                    <b>🛵</b><span><strong>Pedir pelo iFood</strong><small>Entrega na sua casa</small></span><i>↗</i>
                  </a>
                }
                <button class="option" type="button" (click)="step.set('evento')">
                  <b>🎂</b><span><strong>Eventos</strong><small>Aniversários, casamentos e mais</small></span><i>→</i>
                </button>
                <button class="option" type="button" (click)="step.set('revenda')">
                  <b>🏪</b><span><strong>Quero revender</strong><small>Parceria para o seu negócio</small></span><i>→</i>
                </button>
                <a class="option" [href]="whatsapp()" target="_blank" rel="noreferrer">
                  <b>💬</b><span><strong>Falar com a gente</strong><small>Conversa direta no WhatsApp</small></span><i>↗</i>
                </a>
              </div>
            }
            @case ('evento') {
              <button class="back" type="button" (click)="step.set('menu')">← Voltar</button>
              <span class="eyebrow">Eventos</span>
              <h2 id="modal-title">Vamos adoçar o seu <em>evento.</em></h2>
              <form (ngSubmit)="sendEvento()" #f="ngForm">
                <label>Seu nome<input name="nome" [(ngModel)]="evento.nome" required autocomplete="name"></label>
                <label>Tipo de evento
                  <select name="tipo" [(ngModel)]="evento.tipo" required>
                    <option value="" disabled>Selecione</option>
                    <option>Aniversário</option><option>Casamento</option><option>Outro</option>
                  </select>
                </label>
                <div class="row">
                  <label>Data<input name="data" type="date" [(ngModel)]="evento.data"></label>
                  <label>Nº de pessoas<input name="qtd" type="number" min="1" inputmode="numeric" [(ngModel)]="evento.qtd"></label>
                </div>
                <label>Cidade / bairro<input name="local" [(ngModel)]="evento.local"></label>
                <button class="submit" type="submit" [disabled]="f.invalid">Enviar pelo WhatsApp ↗</button>
              </form>
            }
            @case ('revenda') {
              <button class="back" type="button" (click)="step.set('menu')">← Voltar</button>
              <span class="eyebrow">Revenda</span>
              <h2 id="modal-title">Leve o Sonho de Pudim ao seu <em>negócio.</em></h2>
              <form (ngSubmit)="sendRevenda()" #g="ngForm">
                <label>Seu nome<input name="nome" [(ngModel)]="revenda.nome" required autocomplete="name"></label>
                <label>Nome do negócio<input name="negocio" [(ngModel)]="revenda.negocio" required></label>
                <label>Cidade / bairro<input name="local" [(ngModel)]="revenda.local"></label>
                <label>Volume semanal estimado<input name="volume" [(ngModel)]="revenda.volume" placeholder="Ex.: 40 unidades"></label>
                <button class="submit" type="submit" [disabled]="g.invalid">Enviar pelo WhatsApp ↗</button>
              </form>
            }
          }
        </div>
      </div>
    }
  `
})
export class ContactModalComponent {
  readonly contact = inject(ContactService);
  readonly step = signal<Step>('menu');
  readonly ifoodUrl = IFOOD_URL;

  evento = { nome: '', tipo: '', data: '', qtd: null as number | null, local: '' };
  revenda = { nome: '', negocio: '', local: '', volume: '' };

  constructor() {
    effect(() => {
      const open = this.contact.isOpen();
      document.body.style.overflow = open ? 'hidden' : '';
      if (open) this.step.set('menu');
    }, { allowSignalWrites: true });
  }

  whatsapp(): string { return whatsappLink('Olá! Gostaria de falar com vocês.'); }

  @HostListener('document:keydown.escape')
  onEscape(): void { this.contact.close(); }

  sendEvento(): void {
    const e = this.evento;
    const parts = [`Olá! Me chamo ${e.nome} e gostaria de um orçamento para evento (${e.tipo}).`];
    if (e.data) parts.push(`Data: ${e.data.split('-').reverse().join('/')}.`);
    if (e.qtd) parts.push(`Pessoas: ${e.qtd}.`);
    if (e.local) parts.push(`Local: ${e.local}.`);
    this.go(parts.join(' '));
  }

  sendRevenda(): void {
    const r = this.revenda;
    const parts = [`Olá! Me chamo ${r.nome} e tenho interesse em revender o Sonho de Pudim no(a) ${r.negocio}.`];
    if (r.local) parts.push(`Local: ${r.local}.`);
    if (r.volume) parts.push(`Volume estimado: ${r.volume}.`);
    this.go(parts.join(' '));
  }

  private go(message: string): void {
    window.open(whatsappLink(message), '_blank', 'noopener');
    this.contact.close();
  }
}

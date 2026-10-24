import { Component, HostListener, computed, effect, inject, signal } from '@angular/core';
import { ContactService } from './contact.service';
import { IFOOD_URL, whatsappLink } from './contact.config';
import { ModalStep } from './models';

@Component({
  selector: 'app-contact-modal',
  standalone: true,
  styleUrl: './contact-modal.component.scss',
  template: `
    @if (contact.isOpen()) {
      <div class="backdrop" (click)="contact.close()">
        <div class="modal" role="dialog" aria-modal="true" aria-labelledby="modal-title" (click)="$event.stopPropagation()">
          <button class="close" type="button" aria-label="Fechar" (click)="contact.close()">×</button>

          @switch (step()) {
            @case ('menu') {
              <span class="eyebrow"><img class="modal__icon" src="favicon.png" alt=""> Fale com a gente</span>
              <h2 id="modal-title">Como podemos <em>ajudar?</em></h2>
              <p class="lead">Escolha o motivo do seu contato.</p>

              <button
                class="status-banner"
                [class.is-open]="contact.status().isOpen"
                [class.is-closed]="!contact.status().isOpen"
                type="button"
                (click)="openHorariosFromMenu()">
                <i class="status-dot" aria-hidden="true"></i>
                <span class="status-banner__copy">
                  <strong>{{ contact.status().badgeLabel }}</strong>
                  <small>{{ contact.status().detailLabel }}</small>
                </span>
                <span class="status-banner__action">Ver horários</span>
              </button>

              <div class="options">
                @if (ifoodUrl) {
                  <a class="option" [href]="ifoodUrl" target="_blank" rel="noreferrer">
                    <b>🛵</b><span><strong>Pedir pelo iFood</strong><small>Entrega na sua casa</small></span>
                  </a>
                }
                <button class="option" type="button" (click)="step.set('evento')">
                  <b>🎂</b><span><strong>Eventos</strong><small>Aniversários, casamentos e mais</small></span>
                </button>
                <button class="option" type="button" (click)="step.set('revenda')">
                  <b>🏪</b><span><strong>Quero revender</strong><small>Parceria para o seu negócio</small></span>
                </button>
                <a class="option" [href]="whatsapp()" target="_blank" rel="noreferrer">
                  <b>💬</b><span><strong>Falar com a gente</strong><small>Conversa direta no WhatsApp</small></span>
                </a>
              </div>
            }
            @case ('horarios') {
              @if (fromMenu()) {
                <div class="modal__top">
                  <button class="back" type="button" (click)="step.set('menu')">Voltar</button>
                  <img class="modal__icon" src="favicon.png" alt="">
                </div>
              } @else {
                <span class="eyebrow"><img class="modal__icon" src="favicon.png" alt=""> Centro • Horizonte - CE</span>
              }
              <h2 id="modal-title">Horários da <em>nossa loja.</em></h2>

              <div
                class="hours-status"
                [class.is-open]="contact.status().isOpen"
                [class.is-closed]="!contact.status().isOpen">
                <i class="status-dot" aria-hidden="true"></i>
                <strong>{{ contact.status().badgeLabel }}</strong>
                <span>• {{ contact.status().detailLabel }}</span>
              </div>

              <ul class="hours-list">
                @for (item of contact.storeHours; track item.day) {
                  @let isToday = item.dayIndex === contact.status().todayIndex;
                  <li
                    [class.is-today]="isToday"
                    [class.is-open]="isToday && contact.status().isOpen"
                    [class.is-closed]="isToday && !contact.status().isOpen">
                    <span class="hours-list__day">
                      @if (isToday) {
                        <i class="status-dot" aria-hidden="true"></i>
                      }
                      {{ item.day }}
                      @if (isToday) {
                        <b class="day-badge">{{ contact.status().isOpen ? 'Aberto' : 'Fechado' }}</b>
                      }
                    </span>
                    <span class="hours-list__time">{{ item.open }} às {{ item.close }}</span>
                  </li>
                }
              </ul>

              <button class="submit hours-cta" type="button" (click)="step.set('menu')">Fazer encomenda</button>
            }
            @case ('evento') {
              <div class="modal__top">
                <button class="back" type="button" (click)="step.set('menu')">← Voltar</button>
              </div>
              <div class="modal__hero">
                <div>
                  <span class="eyebrow">Orçamento para festas</span>
                  <h2 id="modal-title">Vamos adoçar o seu <em>evento.</em></h2>
                </div>
                <div class="modal__mascot-badge">
                  <img src="assets/mascote-eventos.png" alt="Mascote de pudim comemorando evento">
                </div>
              </div>
              <form (submit)="submit($event, 'evento')">
                <label>Seu nome<input name="nome" [value]="evento().nome" (input)="setevento('nome', $event)" required autocomplete="name"></label>
                <label>Tipo de evento
                  <select name="tipo" (change)="setevento('tipo', $event)" required>
                    <option value="" disabled>Selecione</option>
                    <option>Aniversário</option><option>Casamento</option><option>Outro</option>
                  </select>
                </label>
                <div class="row">
                  <label>Data<input name="data" type="date" [value]="evento().data" (input)="setevento('data', $event)"></label>
                  <label>Nº de pessoas<input name="qtd" type="number" min="1" inputmode="numeric" [value]="evento().qtd" (input)="setevento('qtd', $event)"></label>
                </div>
                <label>Cidade / bairro<input name="local" [value]="evento().local" (input)="setevento('local', $event)"></label>
                <button class="submit" type="submit" [disabled]="!eventoValid()">Enviar pelo WhatsApp</button>
              </form>
            }
            @case ('revenda') {
              <div class="modal__top">
                <button class="back" type="button" (click)="step.set('menu')">← Voltar</button>
              </div>
              <div class="modal__hero">
                <div>
                  <span class="eyebrow">Parceria comercial</span>
                  <h2 id="modal-title">Leve o Sonho de Pudim ao seu <em>negócio.</em></h2>
                </div>
                <div class="modal__mascot-badge">
                  <img src="assets/mascote-revenda.png" alt="Mascote de pudim parceiro de revenda">
                </div>
              </div>
              <form (submit)="submit($event, 'revenda')">
                <label>Seu nome<input name="nome" [value]="revenda().nome" (input)="setrevenda('nome', $event)" required autocomplete="name"></label>
                <label>Nome do negócio<input name="negocio" [value]="revenda().negocio" (input)="setrevenda('negocio', $event)" required></label>
                <label>Cidade / bairro<input name="local" [value]="revenda().local" (input)="setrevenda('local', $event)"></label>
                <label>Volume semanal estimado<input name="volume" [value]="revenda().volume" (input)="setrevenda('volume', $event)" placeholder="Ex.: 40 unidades"></label>
                <button class="submit" type="submit" [disabled]="!revendaValid()">Enviar pelo WhatsApp</button>
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
  readonly step = signal<ModalStep>('menu');
  readonly fromMenu = signal(false);
  readonly ifoodUrl = IFOOD_URL;

  readonly evento = signal({ nome: '', tipo: '', data: '', qtd: '', local: '' });
  readonly revenda = signal({ nome: '', negocio: '', local: '', volume: '' });
  readonly eventoValid = computed(() => !!this.evento().nome.trim() && !!this.evento().tipo);
  readonly revendaValid = computed(() => !!this.revenda().nome.trim() && !!this.revenda().negocio.trim());

  setevento(key: string, e: Event): void { const v = (e.target as HTMLInputElement).value; this.evento.update(s => ({ ...s, [key]: v })); }
  setrevenda(key: string, e: Event): void { const v = (e.target as HTMLInputElement).value; this.revenda.update(s => ({ ...s, [key]: v })); }
  submit(e: Event, kind: 'evento' | 'revenda'): void { e.preventDefault(); if (kind === 'evento' ? this.eventoValid() : this.revendaValid()) { kind === 'evento' ? this.sendEvento() : this.sendRevenda(); } }

  constructor() {
    effect(() => {
      const open = this.contact.isOpen();
      document.body.style.overflow = open ? 'hidden' : '';
      if (open) {
        this.fromMenu.set(false);
        this.step.set(this.contact.initialStep());
        this.evento.set({ nome: '', tipo: '', data: '', qtd: '', local: '' });
        this.revenda.set({ nome: '', negocio: '', local: '', volume: '' });
      }
    });
  }

  openHorariosFromMenu(): void {
    this.fromMenu.set(true);
    this.step.set('horarios');
  }

  whatsapp(): string { return whatsappLink('Olá! Gostaria de falar com vocês.'); }

  @HostListener('document:keydown.escape')
  onEscape(): void { this.contact.close(); }

  sendEvento(): void {
    const e = this.evento();
    const parts = [`Olá! Me chamo ${e.nome} e gostaria de um orçamento para evento (${e.tipo}).`];
    if (e.data) parts.push(`Data: ${e.data.split('-').reverse().join('/')}.`);
    if (e.qtd) parts.push(`Pessoas: ${e.qtd}.`);
    if (e.local) parts.push(`Local: ${e.local}.`);
    this.go(parts.join(' '));
  }

  sendRevenda(): void {
    const r = this.revenda();
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

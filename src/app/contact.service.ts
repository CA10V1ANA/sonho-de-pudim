import { DestroyRef, Injectable, inject, signal } from '@angular/core';
import { ModalStep, StoreHour, StoreStatus } from './models';

const WEEKDAY_MAP: Record<string, number> = {
  Sun: 0,
  Mon: 1,
  Tue: 2,
  Wed: 3,
  Thu: 4,
  Fri: 5,
  Sat: 6
};

export const STORE_HOURS: readonly StoreHour[] = [
  { day: 'Segunda-feira', open: '08:00', close: '17:00', dayIndex: 1 },
  { day: 'Terça-feira', open: '08:00', close: '17:00', dayIndex: 2 },
  { day: 'Quarta-feira', open: '11:00', close: '15:00', dayIndex: 3 },
  { day: 'Quinta-feira', open: '08:00', close: '17:00', dayIndex: 4 },
  { day: 'Sexta-feira', open: '08:00', close: '17:00', dayIndex: 5 },
  { day: 'Sábado', open: '08:00', close: '17:00', dayIndex: 6 },
  { day: 'Domingo', open: '09:00', close: '14:00', dayIndex: 0 }
];

function toMinutes(hhmm: string): number {
  const [hours, minutes] = hhmm.split(':').map(Number);
  return hours * 60 + minutes;
}

function getFortalezaTime(now: Date): { dayIndex: number; minutes: number } {
  try {
    const parts = new Intl.DateTimeFormat('en-US', {
      timeZone: 'America/Fortaleza',
      weekday: 'short',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false
    }).formatToParts(now);

    const weekday = parts.find(p => p.type === 'weekday')?.value ?? '';
    const hour = Number(parts.find(p => p.type === 'hour')?.value ?? 0) % 24;
    const minute = Number(parts.find(p => p.type === 'minute')?.value ?? 0);
    return {
      dayIndex: WEEKDAY_MAP[weekday] ?? now.getDay(),
      minutes: hour * 60 + minute
    };
  } catch {
    return { dayIndex: now.getDay(), minutes: now.getHours() * 60 + now.getMinutes() };
  }
}

function computeStoreStatus(now = new Date()): StoreStatus {
  const { dayIndex, minutes } = getFortalezaTime(now);
  const todayRule = STORE_HOURS.find(item => item.dayIndex === dayIndex) ?? STORE_HOURS[0];
  const openMin = toMinutes(todayRule.open);
  const closeMin = toMinutes(todayRule.close);
  const isOpen = minutes >= openMin && minutes < closeMin;

  if (isOpen) {
    return {
      isOpen: true,
      todayIndex: dayIndex,
      badgeLabel: 'Aberto agora',
      detailLabel: `Hoje até às ${todayRule.close}`
    };
  }

  if (minutes < openMin) {
    return {
      isOpen: false,
      todayIndex: dayIndex,
      badgeLabel: 'Fechado agora',
      detailLabel: `Abre hoje às ${todayRule.open}`
    };
  }

  const tomorrowIndex = (dayIndex + 1) % 7;
  const tomorrowRule = STORE_HOURS.find(item => item.dayIndex === tomorrowIndex) ?? STORE_HOURS[0];
  return {
    isOpen: false,
    todayIndex: dayIndex,
    badgeLabel: 'Fechado agora',
    detailLabel: `Abre amanhã às ${tomorrowRule.open}`
  };
}

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly destroyRef = inject(DestroyRef);

  readonly storeHours = STORE_HOURS;
  readonly isOpen = signal(false);
  readonly initialStep = signal<ModalStep>('menu');
  readonly status = signal<StoreStatus>(computeStoreStatus());

  constructor() {
    const timer = setInterval(() => this.status.set(computeStoreStatus()), 30_000);
    this.destroyRef.onDestroy(() => clearInterval(timer));
  }

  open(step: ModalStep = 'menu'): void {
    this.status.set(computeStoreStatus());
    this.initialStep.set(step);
    this.isOpen.set(true);
  }

  close(): void {
    this.isOpen.set(false);
  }
}

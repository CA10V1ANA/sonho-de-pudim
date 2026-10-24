export interface Occasion {
  readonly title: string;
  readonly text: string;
  readonly step: ModalStep;
  readonly featured?: boolean;
}

export interface ProductOption {
  readonly label: string;
  readonly icon: string;
  readonly variant?: 'zero' | 'coffee';
}

export interface ProductSize {
  readonly badge: string;
  readonly title: string;
  readonly serves: string;
  readonly volume: string;
  readonly options: readonly ProductOption[];
  readonly text: string;
  readonly image: string;
  readonly alt: string;
}

export interface Review {
  readonly quote: string;
  readonly author: string;
  readonly tag: string;
}

export type ModalStep = 'menu' | 'evento' | 'revenda' | 'horarios';

export interface StoreHour {
  readonly day: string;
  readonly open: string;
  readonly close: string;
  readonly dayIndex: number;
}

export interface StoreStatus {
  readonly isOpen: boolean;
  readonly todayIndex: number;
  readonly badgeLabel: string;
  readonly detailLabel: string;
}

export interface Highlight {
  readonly title: string;
  readonly text: string;
}

export interface Occasion {
  readonly title: string;
  readonly text: string;
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

export interface StoreHour {
  readonly day: string;
  readonly hours: string;
  readonly dayIndex: number;
}

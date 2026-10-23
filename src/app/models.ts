export interface Highlight {
  readonly number: string;
  readonly title: string;
  readonly text: string;
}

export interface Occasion {
  readonly number: string;
  readonly title: string;
  readonly text: string;
  readonly featured?: boolean;
}

export interface ProductSize {
  readonly badge: string;
  readonly title: string;
  readonly serves: string;
  readonly volume: string;
  readonly options: readonly string[];
  readonly text: string;
  readonly image: string;
  readonly alt: string;
}

export interface Review {
  readonly quote: string;
  readonly author: string;
  readonly tag: string;
}

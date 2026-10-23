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

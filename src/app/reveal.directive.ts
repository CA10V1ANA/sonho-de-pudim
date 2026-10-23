import { AfterViewInit, Directive, ElementRef, OnDestroy, inject } from '@angular/core';

@Directive({ selector: '[appReveal]', standalone: true })
export class RevealDirective implements AfterViewInit, OnDestroy {
  private readonly element = inject(ElementRef<HTMLElement>);
  private observer?: IntersectionObserver;

  ngAfterViewInit(): void {
    if (matchMedia('(prefers-reduced-motion: reduce)').matches) {
      this.element.nativeElement.classList.add('is-visible');
      return;
    }

    this.observer = new IntersectionObserver(([entry]) => {
      if (entry.isIntersecting) {
        this.element.nativeElement.classList.add('is-visible');
        this.observer?.disconnect();
      }
    }, { threshold: 0.16, rootMargin: '0px 0px -5% 0px' });

    this.observer.observe(this.element.nativeElement);
  }

  ngOnDestroy(): void { this.observer?.disconnect(); }
}

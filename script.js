const menu = document.querySelector('.menu');
const links = document.querySelector('.links');
menu.addEventListener('click', () => {
  const open = links.classList.toggle('open');
  menu.setAttribute('aria-expanded', open);
});
links.querySelectorAll('a').forEach(a => a.addEventListener('click', () => links.classList.remove('open')));

const observer = new IntersectionObserver((entries) => {
  entries.forEach((entry, i) => {
    if (entry.isIntersecting) {
      setTimeout(() => entry.target.classList.add('visible'), i * 70);
      observer.unobserve(entry.target);
    }
  });
}, { threshold: .12 });
document.querySelectorAll('.reveal').forEach(el => observer.observe(el));

const toast = document.querySelector('.toast');
let toastTimer;
document.querySelectorAll('.flavor-tag').forEach(tag => tag.addEventListener('click', () => {
  toast.textContent = tag.dataset.note;
  toast.classList.add('show');
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => toast.classList.remove('show'), 2800);
}));

const art = document.querySelector('[data-parallax]');
if (window.matchMedia('(pointer:fine)').matches) {
  art.addEventListener('pointermove', e => {
    const r = art.getBoundingClientRect();
    const x = (e.clientX - r.left) / r.width - .5;
    const y = (e.clientY - r.top) / r.height - .5;
    art.querySelector('.hero-pudim').style.transform = `translate(${x * 14}px, ${y * 14}px) rotate(${-3 + x * 2}deg)`;
    art.querySelector('.orbit-note').style.transform = `translate(${x * -10}px, ${y * -10}px) rotate(9deg)`;
  });
}
document.getElementById('year').textContent = new Date().getFullYear();

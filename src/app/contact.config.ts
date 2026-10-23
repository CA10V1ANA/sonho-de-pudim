/** Número do WhatsApp com DDI+DDD, só dígitos. Ex.: '5511999999999'. Vazio = usa o link curto (sem mensagem pré-preenchida). */
export const WHATSAPP_NUMBER = '5585992001749';
export const WHATSAPP_FALLBACK = 'https://w.app/sonhodepudim';
/** Cole aqui o link da loja no iFood. */
export const IFOOD_URL = 'https://www.ifood.com.br'; // TODO: trocar pelo link da loja

export function whatsappLink(message?: string): string {
  if (!WHATSAPP_NUMBER) return WHATSAPP_FALLBACK;
  return `https://wa.me/${WHATSAPP_NUMBER}` + (message ? `?text=${encodeURIComponent(message)}` : '');
}

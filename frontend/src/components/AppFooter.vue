<template>
  <footer class="w-full mt-8">
    <div class="bg-black/20 backdrop-blur-sm border-t border-white/10 px-6 py-5">
      <div class="max-w-4xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
        <div class="flex items-center gap-6">
          <img
            :src="logoUCR"
            alt="UCR"
            class="h-7 object-contain brightness-0 invert opacity-90 cursor-pointer select-none"
            @click="abrirEasterEgg"
          />
          <div class="w-px h-8 bg-white/30" />
          <img :src="logoEIC" alt="EIC" class="h-10 object-contain brightness-0 invert opacity-90" />
        </div>
        <div class="text-center md:text-right">
          <p class="text-white/90 text-sm font-medium">© {{ anio }} Escuela de Ingeniería Civil</p>
          <p class="text-white/60 text-xs mt-1">Universidad de Costa Rica</p>
          <a href="mailto:soporte.eic@ucr.ac.cr" class="text-blue-200 hover:text-white text-xs transition-colors duration-200 mt-1 inline-block">
            soporte.eic@ucr.ac.cr
          </a>
        </div>
      </div>
    </div>
  </footer>

  <!-- Easter Egg -->
  <Teleport to="body">
    <Transition name="egg">
      <div v-if="easterEggVisible" class="egg-overlay">

        <!-- Confetti infinito -->
        <div v-for="n in 120" :key="n" class="confetti" :style="confettiStyle(n)" />

        <!-- Esquinas decorativas -->
        <div class="corner corner-tl">★</div>
        <div class="corner corner-tr">★</div>
        <div class="corner corner-bl">★</div>
        <div class="corner corner-br">★</div>

        <!-- Contenido -->
        <div class="egg-content">
          <div class="bad-border">

            <!-- Alerta parpadeante -->
            <p class="blink-text">⚠️ ATENCION ATENCION ⚠️</p>
            <p class="blink-text2">🚨 SITIO OFICIAL CERTIFICADO 🚨</p>

            <!-- Título arcoíris con wobble -->
            <div class="rainbow-title">
              <span v-for="(char, i) in 'diceñado x daniel'" :key="i"
                :style="{ color: RAINBOW[i % RAINBOW.length], animationDelay: (i * 0.08) + 's' }"
                class="wobble-char">{{ char }}</span>
            </div>

            <!-- Estrellitas flotantes -->
            <div class="floating-stars">
              <span class="star" style="animation-delay:0s">⭐</span>
              <span class="star" style="animation-delay:0.3s">🌟</span>
              <span class="star" style="animation-delay:0.6s">✨</span>
              <span class="star" style="animation-delay:0.9s">💫</span>
              <span class="star" style="animation-delay:1.2s">⭐</span>
            </div>

            <!-- Marquee doble -->
            <div class="marquee-wrap">
              <span class="marquee-text">
                🏆 MEJOR DISEÑADOR DEL MUNDO &nbsp;•&nbsp; 🎨 PIXEL PERFECT &nbsp;•&nbsp;
                💎 PREMIUM QUALITY &nbsp;•&nbsp; 🔥 HOT DESIGN &nbsp;•&nbsp;
                🏆 MEJOR DISEÑADOR DEL MUNDO &nbsp;•&nbsp; 🎨 PIXEL PERFECT
              </span>
            </div>
            <div class="marquee-wrap marquee-reverse">
              <span class="marquee-text2">
                👑 DANIEL STUDIOS™ &nbsp;•&nbsp; 🦄 UNICORN DESIGNS &nbsp;•&nbsp;
                💅 SUPER PROFESIONAL &nbsp;•&nbsp; 🚀 NEXT LEVEL &nbsp;•&nbsp;
                👑 DANIEL STUDIOS™ &nbsp;•&nbsp; 🦄 UNICORN DESIGNS
              </span>
            </div>

            <!-- Contador de visitas falso -->
            <div class="visit-counter">
              <span class="blink-text3">👁️ VISITAS: </span>
              <span class="counter-num">{{ visitas.toLocaleString() }}</span>
            </div>

            <!-- Badges -->
            <div class="badges">
              <span class="badge" style="background:#ff0080;transform:rotate(-5deg)">🥇 AWARD WINNING</span>
              <span class="badge" style="background:#00aa00;transform:rotate(4deg)">✅ CERTIFIED PRO</span>
              <span class="badge" style="background:#ff6600;transform:rotate(-2deg)">🔥 TOP DESIGNER</span>
              <span class="badge" style="background:#6600ff;transform:rotate(3deg)">💎 PREMIUM</span>
              <span class="badge" style="background:#0088ff;transform:rotate(-3deg)">🚀 NEXT LEVEL</span>
            </div>

            <!-- Separador fancy -->
            <p class="separator">— ✦ —— ✦ —— ✦ —— ✦ —— ✦ —</p>

            <!-- Testimonios falsos -->
            <div class="testimonials">
              <p class="testimonial">"el mejor diseñador que e conosido" — <em>mama de daniel</em></p>
              <p class="testimonial">"increible trabajo bro 10/10" — <em>daniel mismo</em></p>
              <p class="testimonial">"no save usar figma pero igual" — <em>fuente anonima</em></p>
            </div>

            <!-- Under construction -->
            <div class="under-construction">
              <span class="blink-text">🚧</span>
              <span> SITIO EN KONSTRUKCION </span>
              <span class="blink-text">🚧</span>
            </div>

            <!-- Créditos -->
            <p class="copy-text">© {{ anio }} daniel produccionz™ &nbsp;|&nbsp; all rightz reserved</p>
            <p class="made-with">hecho con ❤️ MS Paint, fe y mucho ctrl+z</p>
            <p class="made-with" style="color:#ff0099;">optimizado para Internet Explorer 6.0</p>

            <!-- Botón cerrar -->
            <button class="close-btn" @click="cerrarEasterEgg">
              [ cerrar xd ]
            </button>

          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { ref, onBeforeUnmount } from 'vue'
import logoUCR from '@/assets/UCR.png'
import logoEIC from '@/assets/EIC.png'

const anio = new Date().getFullYear()
const easterEggVisible = ref(false)
const visitas = ref(Math.floor(Math.random() * 900000) + 100000)

const RAINBOW = ['#ff0000','#ff6600','#ffcc00','#33cc00','#0099ff','#6600ff','#ff00cc']
const CONFETTI_COLORS = ['#ff0000','#ff6600','#ffdd00','#33cc00','#0088ff','#cc00ff','#ff0099','#00ffcc','#ff4444','#44ff44','#ffffff','#ffaaff']

let visitasInterval = null

function abrirEasterEgg() {
  visitas.value = Math.floor(Math.random() * 900000) + 100000
  easterEggVisible.value = true
  visitasInterval = setInterval(() => {
    visitas.value += Math.floor(Math.random() * 7) + 1
  }, 300)
}

function cerrarEasterEgg() {
  easterEggVisible.value = false
  clearInterval(visitasInterval)
}

onBeforeUnmount(() => clearInterval(visitasInterval))

function confettiStyle(n) {
  const seed = n * 137.508
  const left  = (seed * 1.3) % 100
  const delay = (seed * 0.11) % 5
  const dur   = 3 + (seed * 0.04) % 3
  const size  = 8 + (n % 9) * 2
  const color = CONFETTI_COLORS[n % CONFETTI_COLORS.length]
  const rot   = (seed * 3.7) % 360
  return {
    left:              `${left}%`,
    animationDelay:    `${delay}s`,
    animationDuration: `${dur}s`,
    width:             `${size}px`,
    height:            `${size}px`,
    background:        color,
    transform:         `rotate(${rot}deg)`,
    borderRadius:      n % 3 === 0 ? '50%' : n % 3 === 1 ? '0' : '3px',
  }
}
</script>

<style scoped>
/* ── Overlay ── */
.egg-overlay {
  position: fixed;
  inset: 0;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 99999;
  overflow: hidden;
}

/* ── Estrellas esquinas ── */
.corner {
  position: absolute;
  font-size: 2.5rem;
  animation: spin 3s linear infinite;
  color: gold;
  text-shadow: 0 0 10px orange;
}
.corner-tl { top: 1rem; left: 1rem; }
.corner-tr { top: 1rem; right: 1rem; animation-direction: reverse; }
.corner-bl { bottom: 1rem; left: 1rem; animation-direction: reverse; }
.corner-br { bottom: 1rem; right: 1rem; }
@keyframes spin { to { transform: rotate(360deg); } }

/* ── Contenido ── */
.egg-content {
  position: relative;
  z-index: 1;
  max-width: 780px;
  width: 94%;
  text-align: center;
  max-height: 90vh;
  overflow-y: auto;
}

/* ── Triple borde bad design ── */
.bad-border {
  font-family: 'Comic Sans MS', 'Chalkboard SE', cursive;
  border: 7px dashed #ff0099;
  outline: 5px solid #00cc00;
  box-shadow: 0 0 0 12px #ffdd00, 0 0 0 17px #ff6600, 0 0 0 22px #cc00ff;
  border-radius: 16px;
  padding: 2.5rem 3rem;
  background: linear-gradient(135deg, #fffde7 0%, #fff0f6 40%, #f0f8ff 70%, #f0fff0 100%);
}

/* ── Título arcoíris con wobble ── */
.rainbow-title {
  font-size: clamp(2.2rem, 6vw, 4rem);
  font-weight: 900;
  letter-spacing: 3px;
  text-shadow: 4px 4px 0 rgba(0,0,0,0.12), 7px 7px 0 rgba(255,0,150,0.15);
  line-height: 1.2;
  margin: 0.75rem 0 1rem;
}
.wobble-char {
  display: inline-block;
  animation: wobble 1.2s ease-in-out infinite alternate;
}
@keyframes wobble {
  0%   { transform: translateY(0) rotate(-3deg) scale(1); }
  50%  { transform: translateY(-8px) rotate(3deg) scale(1.1); }
  100% { transform: translateY(2px) rotate(-1deg) scale(0.95); }
}

/* ── Estrellas flotantes ── */
.floating-stars {
  display: flex;
  justify-content: center;
  gap: 0.75rem;
  margin: 0.5rem 0 0.75rem;
}
.star {
  font-size: 1.5rem;
  animation: floatStar 1.5s ease-in-out infinite alternate;
}
@keyframes floatStar {
  from { transform: translateY(0) scale(1); }
  to   { transform: translateY(-10px) scale(1.2); }
}

/* ── Blink ── */
.blink-text {
  font-size: 1.1rem;
  color: #ff0099;
  font-weight: bold;
  animation: blink 0.5s step-start infinite;
  margin: 0.1rem 0;
}
.blink-text2 {
  font-size: 0.9rem;
  color: #ff6600;
  font-weight: bold;
  animation: blink 0.8s step-start infinite;
  margin-bottom: 0.25rem;
}
.blink-text3 {
  font-size: 0.85rem;
  color: #cc0000;
  font-weight: bold;
  animation: blink 1s step-start infinite;
}
@keyframes blink {
  0%, 100% { opacity: 1; }
  50%       { opacity: 0; }
}

/* ── Marquee ── */
.marquee-wrap {
  overflow: hidden;
  white-space: nowrap;
  font-size: 0.88rem;
  color: #6600ff;
  font-weight: bold;
  margin: 0.4rem 0;
}
.marquee-reverse { color: #cc0066; }
.marquee-text {
  display: inline-block;
  animation: marquee 7s linear infinite;
}
.marquee-text2 {
  display: inline-block;
  animation: marquee2 9s linear infinite;
}
@keyframes marquee  { from { transform: translateX(100%); } to { transform: translateX(-100%); } }
@keyframes marquee2 { from { transform: translateX(-100%); } to { transform: translateX(100%); } }

/* ── Contador de visitas ── */
.visit-counter {
  background: #000080;
  color: #00ff00;
  font-size: 0.9rem;
  font-weight: bold;
  padding: 4px 14px;
  border-radius: 4px;
  display: inline-block;
  margin: 0.6rem 0;
  letter-spacing: 2px;
  border: 2px inset #0000aa;
}
.counter-num {
  color: #ffff00;
  font-size: 1.1rem;
}

/* ── Badges ── */
.badges {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 0.6rem;
  margin: 0.75rem 0;
}
.badge {
  font-size: 0.72rem;
  font-weight: 900;
  color: white;
  padding: 4px 12px;
  border-radius: 4px;
  display: inline-block;
  text-shadow: 1px 1px 0 rgba(0,0,0,0.5);
  letter-spacing: 1px;
  box-shadow: 2px 2px 0 rgba(0,0,0,0.3);
}

/* ── Separador ── */
.separator {
  color: #cc00ff;
  font-size: 0.9rem;
  margin: 0.5rem 0;
  letter-spacing: 3px;
}

/* ── Testimonios ── */
.testimonials {
  background: rgba(255,255,255,0.6);
  border: 2px dotted #aaa;
  border-radius: 8px;
  padding: 0.75rem 1rem;
  margin: 0.5rem 0;
  text-align: left;
}
.testimonial {
  font-size: 0.78rem;
  color: #555;
  margin: 0.25rem 0;
  font-style: italic;
}

/* ── Under construction ── */
.under-construction {
  font-size: 0.95rem;
  font-weight: bold;
  color: #ff6600;
  background: repeating-linear-gradient(
    45deg, #ffdd00, #ffdd00 10px, #000000 10px, #000000 20px
  );
  color: white;
  text-shadow: 1px 1px 0 #000;
  padding: 4px 12px;
  border-radius: 4px;
  display: inline-block;
  margin: 0.6rem 0;
  font-size: 0.8rem;
  letter-spacing: 2px;
}

/* ── Créditos ── */
.copy-text {
  font-size: 0.72rem;
  color: #888;
  margin-top: 0.75rem;
}
.made-with {
  font-size: 0.68rem;
  color: #aaa;
  margin-top: 0.15rem;
}

/* ── Botón cerrar ── */
.close-btn {
  font-family: 'Comic Sans MS', 'Chalkboard SE', cursive;
  font-size: 0.9rem;
  color: #0066cc;
  background: none;
  border: 2px dashed #0066cc;
  border-radius: 6px;
  cursor: pointer;
  margin-top: 1.2rem;
  padding: 6px 20px;
  transition: all 0.2s;
  display: inline-block;
}
.close-btn:hover {
  color: white;
  background: #ff0099;
  border-color: #ff0099;
  transform: scale(1.05) rotate(-2deg);
}

/* ── Confetti infinito ── */
.confetti {
  position: absolute;
  top: -30px;
  pointer-events: none;
  animation: fall linear infinite;
}
@keyframes fall {
  0%   { top: -30px; opacity: 1;   transform: rotate(0deg) translateX(0); }
  50%  { transform: rotate(360deg) translateX(30px); }
  100% { top: 110vh;  opacity: 0.3; transform: rotate(720deg) translateX(-20px); }
}

/* ── Transición entrada/salida ── */
.egg-enter-active { animation: zoomIn 0.4s cubic-bezier(.17,.67,.35,1.4) forwards; }
.egg-leave-active { animation: zoomOut 0.25s ease-in forwards; }
@keyframes zoomIn  { from { opacity:0; transform: scale(0.2) rotate(-5deg); } to { opacity:1; transform: scale(1) rotate(0); } }
@keyframes zoomOut { from { opacity:1; transform: scale(1); } to { opacity:0; transform: scale(0.2) rotate(5deg); } }
</style>

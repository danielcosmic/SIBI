<template>
  <footer class="w-full mt-8">
    <div class="bg-black/20 backdrop-blur-sm border-t border-white/10 px-6 py-5">
      <div class="max-w-4xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">

        <!-- Logos -->
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

        <!-- Info -->
        <div class="text-center md:text-right">
          <p class="text-white/90 text-sm font-medium">
            © {{ anio }} Escuela de Ingeniería Civil
          </p>
          <p class="text-white/60 text-xs mt-1">Universidad de Costa Rica</p>
          <a
            href="mailto:soporte.eic@ucr.ac.cr"
            class="text-blue-200 hover:text-white text-xs transition-colors duration-200 mt-1 inline-block"
          >
            soporte.eic@ucr.ac.cr
          </a>
        </div>

      </div>
    </div>
  </footer>

  <!-- Easter Egg -->
  <Teleport to="body">
    <Transition name="egg">
      <div
        v-if="easterEggVisible"
        class="egg-overlay"
      >
        <!-- Confetti -->
        <div
          v-for="n in 90"
          :key="n"
          class="confetti"
          :style="confettiStyle(n)"
        />

        <!-- Contenido principal -->
        <div class="egg-content" @click.stop>

          <!-- Borde tipo bad design -->
          <div class="bad-border">

            <p class="blink-text">✨✨✨ ATENCION ✨✨✨</p>

            <div class="rainbow-title">
              <span
                v-for="(char, i) in 'diceñado x daniel'"
                :key="i"
                :style="{ color: RAINBOW[i % RAINBOW.length] }"
              >{{ char }}</span>
            </div>

            <p class="marquee-wrap">
              <span class="marquee-text">🌈 &nbsp; diseñador profesional &nbsp; 🌈 &nbsp; diseñador profesional &nbsp; 🌈</span>
            </p>

            <div class="badges">
              <span class="badge" style="background:#ff0080; transform: rotate(-4deg)">AWARD WINNING</span>
              <span class="badge" style="background:#00cc00; transform: rotate(3deg)">CERTIFIED PRO</span>
              <span class="badge" style="background:#ff6600; transform: rotate(-2deg)">TOP DESIGNER</span>
            </div>

            <p class="copy-text">© {{ anio }} daniel produccionz™ &nbsp;|&nbsp; all rightz reserved</p>
            <p class="made-with">made with ❤️ &amp; MS Paint</p>

            <button class="close-btn" @click="cerrarEasterEgg">[ cerrar xd ]</button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { ref } from 'vue'
import logoUCR from '@/assets/UCR.png'
import logoEIC from '@/assets/EIC.png'

const anio = new Date().getFullYear()
const easterEggVisible = ref(false)

const RAINBOW = ['#ff0000','#ff6600','#ffcc00','#33cc00','#0099ff','#6600ff','#ff00cc']

function abrirEasterEgg() {
  easterEggVisible.value = true
}

function cerrarEasterEgg() {
  easterEggVisible.value = false
}

const CONFETTI_COLORS = ['#ff0000','#ff6600','#ffdd00','#33cc00','#0088ff','#cc00ff','#ff0099','#00ffcc','#ff4444','#44ff44']

function confettiStyle(n) {
  const seed = n * 137.508
  const left    = (seed * 1.3) % 100
  const delay   = (seed * 0.07) % 3
  const dur     = 2.5 + (seed * 0.031) % 2
  const size    = 8 + (n % 7) * 2
  const color   = CONFETTI_COLORS[n % CONFETTI_COLORS.length]
  const rot     = (seed * 3.7) % 360
  return {
    left:              `${left}%`,
    animationDelay:    `${delay}s`,
    animationDuration: `${dur}s`,
    width:             `${size}px`,
    height:            `${size}px`,
    background:        color,
    transform:         `rotate(${rot}deg)`,
    borderRadius:      n % 3 === 0 ? '50%' : n % 3 === 1 ? '0' : '2px',
  }
}
</script>

<style scoped>
/* ── Easter egg overlay ── */
.egg-overlay {
  position: fixed;
  inset: 0;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 99999;
  overflow: hidden;
  cursor: pointer;
}

.egg-content {
  position: relative;
  z-index: 1;
  cursor: default;
  max-width: 600px;
  width: 90%;
  text-align: center;
}

/* Borde estilo bad design — triple borde de colores */
.bad-border {
  font-family: 'Comic Sans MS', 'Chalkboard SE', cursive;
  border: 6px dashed #ff0099;
  outline: 4px solid #00cc00;
  box-shadow: 0 0 0 10px #ffdd00, 0 0 0 14px #ff6600, 8px 8px 0 14px #cc00ff;
  border-radius: 12px;
  padding: 2rem 2.5rem;
  background: linear-gradient(135deg, #fffde7 0%, #fff0f6 50%, #f0f8ff 100%);
}

/* Título arcoíris */
.rainbow-title {
  font-family: 'Comic Sans MS', cursive;
  font-size: clamp(1.8rem, 5vw, 3rem);
  font-weight: 900;
  letter-spacing: 2px;
  text-shadow: 3px 3px 0 rgba(0,0,0,0.15), 5px 5px 0 rgba(255,0,150,0.2);
  line-height: 1.2;
  margin: 0.5rem 0 1rem;
  word-break: break-word;
}

/* Texto parpadeante */
.blink-text {
  font-family: 'Comic Sans MS', cursive;
  font-size: 1rem;
  color: #ff0099;
  font-weight: bold;
  animation: blink 0.6s step-start infinite;
  margin-bottom: 0.25rem;
}

@keyframes blink {
  0%, 100% { opacity: 1; }
  50%       { opacity: 0; }
}

/* Marquee */
.marquee-wrap {
  overflow: hidden;
  white-space: nowrap;
  font-family: 'Comic Sans MS', cursive;
  font-size: 0.85rem;
  color: #6600ff;
  font-weight: bold;
  margin: 0.75rem 0;
}
.marquee-text {
  display: inline-block;
  animation: marquee 4s linear infinite;
}
@keyframes marquee {
  from { transform: translateX(60%); }
  to   { transform: translateX(-100%); }
}

/* Badges */
.badges {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin: 1rem 0 0.75rem;
}
.badge {
  font-family: 'Comic Sans MS', cursive;
  font-size: 0.7rem;
  font-weight: 900;
  color: white;
  padding: 3px 10px;
  border-radius: 4px;
  display: inline-block;
  text-shadow: 1px 1px 0 rgba(0,0,0,0.4);
  letter-spacing: 1px;
}

/* Textos inferiores */
.copy-text {
  font-family: 'Comic Sans MS', cursive;
  font-size: 0.7rem;
  color: #888;
  margin-top: 0.75rem;
}
.made-with {
  font-family: 'Comic Sans MS', cursive;
  font-size: 0.65rem;
  color: #aaa;
  margin-top: 0.2rem;
}

/* Botón cerrar */
.close-btn {
  font-family: 'Comic Sans MS', cursive;
  font-size: 0.8rem;
  color: #0066cc;
  background: none;
  border: none;
  cursor: pointer;
  margin-top: 1rem;
  text-decoration: underline;
  display: block;
  width: 100%;
}
.close-btn:hover { color: #ff0099; }

/* ── Confetti ── */
.confetti {
  position: absolute;
  top: -20px;
  animation: fall linear forwards;
  pointer-events: none;
}
@keyframes fall {
  0%   { top: -20px; opacity: 1; transform: rotate(0deg) scale(1); }
  80%  { opacity: 1; }
  100% { top: 110vh;  opacity: 0; transform: rotate(720deg) scale(0.5); }
}

/* ── Transición entrada/salida ── */
.egg-enter-active { animation: zoomIn 0.35s cubic-bezier(.17,.67,.35,1.4) forwards; }
.egg-leave-active { animation: zoomOut 0.25s ease-in forwards; }
@keyframes zoomIn  { from { opacity:0; transform: scale(0.3); } to { opacity:1; transform: scale(1); } }
@keyframes zoomOut { from { opacity:1; transform: scale(1); }   to { opacity:0; transform: scale(0.3); } }
</style>

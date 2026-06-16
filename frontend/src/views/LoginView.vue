<template>
  <div class="min-h-screen relative flex items-center justify-center px-4 pb-16 bg-gradient-to-br from-[#4da6ff] via-[#0066cc] to-[#003d7a]">
    <div class="w-full max-w-md">
      <div class="bg-white/95 backdrop-blur-sm rounded-2xl shadow-2xl p-10 border border-white/20">
        <!-- Logo -->
        <div class="flex flex-col items-center mb-7">
          <div class="w-32 h-32 mb-4">
            <img :src="sibiLogo" alt="SIBI" class="w-full h-full object-contain" />
          </div>
          <h1 class="text-2xl font-bold text-[#003d7a]">Inventario EIC</h1>
          <p class="text-gray-500 text-sm mt-1">Universidad de Costa Rica</p>
        </div>

        <hr class="border-gray-100 mb-6" />

        <!-- Error de credenciales -->
        <Transition name="fade-error">
          <div v-if="error" class="mb-5 p-3.5 bg-red-50 border border-red-200 rounded-lg flex items-start gap-2.5">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-red-500 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p class="text-sm text-red-700 font-medium">{{ error }}</p>
          </div>
        </Transition>

        <!-- Form -->
        <form @submit.prevent="handleSubmit" class="space-y-5">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Correo Institucional</label>
            <input
              v-model="correo"
              type="email"
              placeholder="usuario@ucr.ac.cr"
              required
              @input="errorCorreo = ''"
              class="w-full px-4 py-3 border rounded-lg focus:ring-2 outline-none transition bg-gray-50/50 text-sm"
              :class="errorCorreo
                ? 'border-amber-400 focus:ring-amber-300/30 focus:border-amber-400'
                : 'border-gray-200 focus:ring-[#0066cc]/30 focus:border-[#0066cc]/50'"
            />
            <Transition name="fade-error">
              <p v-if="errorCorreo" class="mt-1.5 text-xs text-amber-700 flex items-center gap-1.5">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                {{ errorCorreo }}
              </p>
            </Transition>
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Contraseña</label>
            <div class="relative">
              <input
                v-model="contrasena"
                :type="mostrarContrasena ? 'text' : 'password'"
                required
                class="w-full px-4 py-3 border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc]/30 focus:border-[#0066cc]/50 outline-none transition pr-11 bg-gray-50/50 text-sm"
              />
              <button
                type="button"
                @click="mostrarContrasena = !mostrarContrasena"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition"
              >
                <svg v-if="!mostrarContrasena" xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                </svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                </svg>
              </button>
            </div>

            <!-- Indicador de completitud -->
            <div v-if="contrasena.length > 0" class="mt-3 space-y-2">
              <div class="h-1.5 bg-gray-200 rounded-full overflow-hidden">
                <div
                  class="h-full rounded-full transition-all duration-300"
                  :class="requisitos.barColor"
                  :style="{ width: (requisitos.cumplidos / 4 * 100) + '%' }"
                />
              </div>
              <div class="grid grid-cols-2 gap-x-4 gap-y-1">
                <div v-for="r in requisitos.lista" :key="r.texto" class="flex items-center gap-1.5">
                  <svg v-if="r.ok" xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5 text-green-500 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
                  </svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5 text-gray-300 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                  <span class="text-xs" :class="r.ok ? 'text-green-600' : 'text-gray-400'">{{ r.texto }}</span>
                </div>
              </div>
            </div>
          </div>

          <button
            type="submit"
            :disabled="intentos >= 3 || loading"
            class="w-full bg-[#003d7a] text-white py-3 rounded-lg hover:bg-[#002d5a] transition disabled:bg-gray-400 disabled:cursor-not-allowed font-medium text-sm"
          >
            {{ loading ? 'Ingresando...' : 'Iniciar Sesión' }}
          </button>
        </form>

        <div class="mt-5 flex items-center justify-between text-xs text-gray-400">
          <span>Intentos: {{ intentos }}/3</span>
          <RouterLink to="/recuperar-contrasena" class="text-[#0066cc] hover:underline">
            ¿Olvidaste tu contraseña?
          </RouterLink>
        </div>
      </div>
    </div>

    <div class="absolute bottom-0 left-0 right-0">
      <AppFooter />
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import authService from '@/services/authService'
import AppFooter from '@/components/AppFooter.vue'
import sibiLogo from '@/assets/SIBI_logo_6000_transparente.png'

const router = useRouter()
const auth = useAuthStore()

const correo = ref('')
const contrasena = ref('')
const mostrarContrasena = ref(false)
const error = ref('')
const errorCorreo = ref('')
const intentos = ref(0)
const loading = ref(false)

const requisitos = computed(() => {
  const p = contrasena.value
  const lista = [
    { texto: 'Mínimo 6 caracteres', ok: p.length >= 6 },
    { texto: 'Una mayúscula (A-Z)',  ok: /[A-Z]/.test(p) },
    { texto: 'Una minúscula (a-z)',  ok: /[a-z]/.test(p) },
    { texto: 'Un número (0-9)',      ok: /[0-9]/.test(p) }
  ]
  const cumplidos = lista.filter(r => r.ok).length
  const barColor = cumplidos <= 1 ? 'bg-red-500' : cumplidos <= 2 ? 'bg-yellow-500' : cumplidos === 3 ? 'bg-blue-500' : 'bg-green-500'
  return { lista, cumplidos, barColor }
})

async function handleSubmit() {
  if (intentos.value >= 3) return

  if (!correo.value.endsWith('@ucr.ac.cr')) {
    errorCorreo.value = 'El correo debe ser institucional (@ucr.ac.cr)'
    intentos.value++
    return
  }

  loading.value = true
  try {
    const { data } = await authService.login(correo.value, contrasena.value)
    auth.setAuth(data.token, {
      correo: data.correo,
      nombre: data.nombre,
      permisos: data.permisos,
      esContrasenaTemporal: data.esContrasenaTemporal
    })
    router.push(data.esContrasenaTemporal ? '/cambiar-contrasena' : '/dashboard')
  } catch {
    error.value = 'Credenciales incorrectas.'
    intentos.value++
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.fade-error-enter-active,
.fade-error-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.fade-error-enter-from,
.fade-error-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>

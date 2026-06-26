<template>
  <div class="min-h-screen flex flex-col bg-gradient-to-br from-[#4da6ff] via-[#0066cc] to-[#003d7a]">
    <div class="flex-1 flex items-center justify-center px-4 py-8">
      <div class="w-full max-w-md">
        <div class="bg-white/95 backdrop-blur-sm rounded-2xl shadow-2xl p-8 border border-white/20">

          <!-- Encabezado -->
          <div class="text-center mb-8">
            <div class="w-20 h-20 bg-gradient-to-br from-[#003d7a] to-[#0066cc] rounded-full mx-auto mb-4 flex items-center justify-center shadow-lg">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-9 h-9 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
              </svg>
            </div>
            <h1 class="text-2xl font-bold text-[#003d7a] mb-2">Cambiar Contraseña</h1>
            <p class="text-gray-600 text-sm">
              Hola, <span class="font-medium text-[#003d7a]">{{ auth.nombre }}</span>.<br>
              Tu cuenta tiene una contraseña temporal. Debes establecer una nueva para continuar.
            </p>
          </div>

          <!-- Error global -->
          <div v-if="error" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg flex items-start gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-red-600 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <p class="text-sm text-red-700">{{ error }}</p>
          </div>

          <form @submit.prevent="handleSubmit" class="space-y-5">

            <!-- Nueva contraseña -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Nueva Contraseña</label>
              <div class="relative">
                <input
                  v-model="nueva"
                  :type="verNueva ? 'text' : 'password'"
                  required
                  placeholder="Mínimo 6 caracteres"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none transition pr-10"
                />
                <button type="button" @click="verNueva = !verNueva" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700">
                  <svg v-if="!verNueva" xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" /></svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" /></svg>
                </button>
              </div>

              <!-- Indicador de completitud -->
              <div v-if="nueva.length > 0" class="mt-3 space-y-2">
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

            <!-- Confirmar contraseña -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Confirmar Contraseña</label>
              <div class="relative">
                <input
                  v-model="confirmar"
                  :type="verConfirmar ? 'text' : 'password'"
                  required
                  placeholder="Repite la nueva contraseña"
                  class="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none transition pr-10"
                  :class="confirmar.length > 0 ? (coinciden ? 'border-green-400' : 'border-red-400') : 'border-gray-300'"
                />
                <button type="button" @click="verConfirmar = !verConfirmar" class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700">
                  <svg v-if="!verConfirmar" xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" /></svg>
                  <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" /></svg>
                </button>
              </div>
              <p v-if="confirmar.length > 0 && !coinciden" class="text-xs text-red-500 mt-1">Las contraseñas no coinciden.</p>
              <p v-if="confirmar.length > 0 && coinciden" class="text-xs text-green-600 mt-1 flex items-center gap-1">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" /></svg>
                Las contraseñas coinciden.
              </p>
            </div>

            <button
              type="submit"
              :disabled="!puedeEnviar || loading"
              class="w-full bg-[#003d7a] text-white py-2.5 rounded-lg hover:bg-[#002d5a] transition disabled:bg-gray-300 disabled:cursor-not-allowed font-medium mt-2"
            >
              {{ loading ? 'Guardando...' : 'Establecer Nueva Contraseña' }}
            </button>
          </form>

          <div class="mt-4 text-center">
            <button @click="cerrarSesion" class="text-sm text-gray-500 hover:text-gray-700 transition">
              Cerrar sesión
            </button>
          </div>
        </div>
      </div>
    </div>

    <AppFooter />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import authService from '@/services/authService'
import AppFooter from '@/components/AppFooter.vue'

const router = useRouter()
const auth = useAuthStore()

const nueva = ref('')
const confirmar = ref('')
const verNueva = ref(false)
const verConfirmar = ref(false)
const error = ref('')
const loading = ref(false)

const requisitos = computed(() => {
  const p = nueva.value
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

const coinciden = computed(() => nueva.value === confirmar.value && confirmar.value.length > 0)
const puedeEnviar = computed(() => requisitos.value.cumplidos === 4 && coinciden.value)

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    await authService.cambiarContrasena('', nueva.value)
    auth.clearTempFlag()
    router.push('/dashboard')
  } catch {
    error.value = 'No se pudo cambiar la contraseña. Verifica que cumpla con todos los requisitos.'
  } finally {
    loading.value = false
  }
}

function cerrarSesion() {
  auth.logout()
  router.push('/')
}
</script>

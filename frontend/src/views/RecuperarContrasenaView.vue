<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-[#4da6ff] via-[#0066cc] to-[#003d7a]">
    <div class="w-full max-w-md">
      <div class="bg-white/95 backdrop-blur-sm rounded-2xl shadow-2xl p-8 border border-white/20">
        <div class="text-center mb-8">
          <div class="w-20 h-20 bg-gradient-to-br from-[#003d7a] to-[#0066cc] rounded-full mx-auto mb-4 flex items-center justify-center">
            <span class="text-white text-2xl font-bold">UCR</span>
          </div>
          <h1 class="text-2xl font-bold text-[#003d7a] mb-2">Recuperar Contraseña</h1>
          <p class="text-gray-600 text-sm">Ingresa tu correo institucional para recibir una contraseña temporal</p>
        </div>

        <!-- Resultado exitoso -->
        <div v-if="contrasenaTemp" class="space-y-4">
          <div class="bg-green-50 border border-green-200 rounded-lg p-4">
            <p class="text-sm text-green-800 font-medium mb-2">Contraseña temporal generada:</p>
            <p class="text-2xl font-mono font-bold text-[#003d7a] tracking-wider text-center py-2">
              {{ contrasenaTemp }}
            </p>
          </div>
          <p class="text-sm text-gray-600 text-center">
            Usa esta contraseña para iniciar sesión. Deberás cambiarla al ingresar.
          </p>
          <button
            @click="() => { navigator.clipboard.writeText(contrasenaTemp) }"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium"
          >
            Copiar contraseña
          </button>
          <RouterLink
            to="/"
            class="block w-full text-center px-4 py-2.5 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium"
          >
            Ir al inicio de sesión
          </RouterLink>
        </div>

        <!-- Formulario -->
        <form v-else @submit.prevent="handleSubmit" class="space-y-4">
          <div v-if="error" class="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
            {{ error }}
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Correo Institucional</label>
            <input
              v-model="correo"
              type="email"
              placeholder="usuario@ucr.ac.cr"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none transition"
            />
          </div>
          <button
            type="submit"
            :disabled="loading"
            class="w-full bg-[#003d7a] text-white py-2.5 rounded-lg hover:bg-[#002d5a] transition disabled:bg-gray-400 font-medium"
          >
            {{ loading ? 'Procesando...' : 'Recuperar Contraseña' }}
          </button>
          <div class="text-center">
            <RouterLink to="/" class="text-sm text-[#0066cc] hover:underline">Volver al inicio de sesión</RouterLink>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import authService from '@/services/authService'

const correo = ref('')
const error = ref('')
const loading = ref(false)
const contrasenaTemp = ref('')

async function handleSubmit() {
  error.value = ''
  if (!correo.value.endsWith('@ucr.ac.cr')) {
    error.value = 'Solo se permiten correos con dominio @ucr.ac.cr.'
    return
  }
  loading.value = true
  try {
    const { data } = await authService.recuperar(correo.value)
    contrasenaTemp.value = data.contrasenaTemp
  } catch {
    error.value = 'No se encontró una cuenta activa con ese correo.'
  } finally {
    loading.value = false
  }
}
</script>

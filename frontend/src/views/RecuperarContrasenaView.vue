<template>
  <div class="min-h-screen flex flex-col bg-gradient-to-br from-[#4da6ff] via-[#0066cc] to-[#003d7a]">
    <div class="flex-1 flex items-center justify-center px-4 py-8">
      <div class="w-full max-w-md">
        <div class="bg-white/95 backdrop-blur-sm rounded-2xl shadow-2xl p-8 border border-white/20">
          <div class="text-center mb-8">
            <div class="w-20 h-20 bg-gradient-to-br from-[#003d7a] to-[#0066cc] rounded-full mx-auto mb-4 flex items-center justify-center shadow-lg">
              <span class="text-white text-2xl font-bold tracking-wide">SIBI</span>
            </div>
            <h1 class="text-2xl font-bold text-[#003d7a] mb-2">¿Olvidaste tu contraseña?</h1>
            <p class="text-gray-600 text-sm">Contacta a un administrador o al equipo de GTI para que te proporcionen una contraseña temporal.</p>
          </div>

          <div class="rounded-xl border border-blue-100 overflow-hidden mb-6">
            <div class="bg-gradient-to-r from-[#003d7a] to-[#0055a8] px-4 py-3 flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-white/80 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
              <p class="text-white text-sm font-medium">Correo de soporte</p>
            </div>
            <div class="bg-blue-50 px-4 py-4 text-center">
              <a href="mailto:soporte.eic@ucr.ac.cr" class="text-[#003d7a] font-semibold text-lg hover:underline">
                soporte.eic@ucr.ac.cr
              </a>
              <p class="text-xs text-gray-500 mt-1">Escríbenos indicando tu nombre completo y correo institucional.</p>
            </div>
          </div>

          <div class="bg-amber-50 border border-amber-200 rounded-lg px-4 py-3 mb-6">
            <p class="text-sm text-amber-800">
              Una vez que recibas la contraseña temporal, deberás cambiarla en tu primer ingreso al sistema.
            </p>
          </div>

          <RouterLink
            to="/"
            class="block w-full text-center px-4 py-2.5 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium"
          >
            Volver al inicio de sesión
          </RouterLink>
        </div>
      </div>
    </div>

    <AppFooter />
  </div>
</template>

<script setup>
import AppFooter from '@/components/AppFooter.vue'
</script>

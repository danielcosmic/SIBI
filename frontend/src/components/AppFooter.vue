<template>
  <footer class="w-full mt-8">
    <div class="bg-black/20 backdrop-blur-sm border-t border-white/10 px-6 py-5">
      <div class="max-w-4xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
        <div class="flex items-center gap-6">
          <img :src="logoUCR" alt="UCR" class="h-7 object-contain brightness-0 invert opacity-90" />
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
</template>

<script setup>
import logoUCR from '@/assets/UCR.png'
import logoEIC from '@/assets/EIC.png'

const anio = new Date().getFullYear()
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-[#003d7a]">Dashboard</h1>
      <p class="text-gray-600 mt-1">Resumen general del inventario institucional</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-blue-100/50 bg-gradient-to-br from-white to-blue-50/50 transition-all duration-300">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">Total de Activos</p>
            <p class="text-3xl font-bold text-[#003d7a] mt-2">{{ stats.totalActivos }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-[#4da6ff] to-[#0066cc] rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-red-100/50 bg-gradient-to-br from-white to-red-50/50 transition-all duration-300">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">En Bodega (Desecho)</p>
            <p class="text-3xl font-bold text-red-600 mt-2">{{ stats.enDesecho }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-red-400 to-red-600 rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-blue-100/50 bg-gradient-to-br from-white to-blue-50/50 transition-all duration-300">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">Categorías Activas</p>
            <p class="text-3xl font-bold text-[#0066cc] mt-2">{{ stats.porCategoria.length }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-[#4da6ff] to-[#0066cc] rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7a2 2 0 012-2h4l2 2h8a2 2 0 012 2v7a2 2 0 01-2 2H5a2 2 0 01-2-2V7z" />
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Categorías -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <h3 class="text-lg font-semibold text-[#003d7a] mb-4">Activos por Categoría</h3>
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
        <div
          v-for="cat in stats.porCategoria"
          :key="cat.nombre"
          class="border border-blue-100/50 rounded-xl p-4 hover:shadow-lg transition-all duration-300 cursor-pointer bg-gradient-to-br from-blue-50/30 to-white hover:scale-105"
        >
          <p class="text-2xl mb-2">{{ cat.icono || '📦' }}</p>
          <p class="font-semibold text-gray-800 text-sm">{{ cat.nombre }}</p>
          <p class="text-2xl font-bold text-[#003d7a]">{{ cat.cantidad }}</p>
        </div>
      </div>
    </div>

    <!-- Actividad Reciente -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <h3 class="text-lg font-semibold text-[#003d7a] mb-4">Actividad Reciente</h3>
      <div v-if="actividad.length === 0" class="text-center py-8 text-gray-400">
        No hay actividad reciente.
      </div>
      <div v-else class="space-y-3">
        <div
          v-for="item in actividad"
          :key="item.id"
          class="flex items-start gap-4 p-3 hover:bg-blue-50/30 rounded-xl transition-all duration-200"
        >
          <div class="w-2 h-2 bg-[#0066cc] rounded-full mt-2 flex-shrink-0" />
          <div class="flex-1">
            <p class="font-medium text-gray-800">{{ labelAccion(item.tipoAccion) }}</p>
            <p class="text-sm text-gray-600">Placa: {{ item.activoPlaca }}</p>
            <p class="text-xs text-gray-500 mt-1">
              {{ item.usuarioNombre }} • {{ formatFecha(item.fechaHora) }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import activoService from '@/services/activoService'
import historialService from '@/services/historialService'

const stats = ref({ totalActivos: 0, enDesecho: 0, solicitudesPendientes: 0, porCategoria: [] })
const actividad = ref([])

onMounted(async () => {
  const [statsRes, histRes] = await Promise.all([
    activoService.stats(),
    historialService.listar({ pagina: 1, tamano: 6 })
  ])
  stats.value = statsRes.data
  actividad.value = histRes.data.items
})

const LABELS = {
  Creacion: 'Nuevo activo agregado',
  CambioUbicacion: 'Cambio de ubicación',
  CambioEncargado: 'Cambio de encargado',
  CambioEstado: 'Cambio de estado',
  CambioPlaca: 'Cambio de placa',
  Eliminacion: 'Activo eliminado',
  Aprobacion: 'Eliminación aprobada',
  Rechazo: 'Eliminación rechazada'
}

function labelAccion(tipo) {
  return LABELS[tipo] || tipo
}

function formatFecha(iso) {
  return new Date(iso).toLocaleString('es-CR', { dateStyle: 'short', timeStyle: 'short' })
}
</script>

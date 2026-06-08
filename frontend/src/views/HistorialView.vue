<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-[#003d7a]">Historial de Cambios</h1>
      <p class="text-gray-600 mt-1">Auditoría completa de movimientos y modificaciones</p>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 bg-gradient-to-br from-blue-50/50 to-white border border-blue-100/50">
        <p class="text-sm text-gray-600">Total de Registros</p>
        <p class="text-3xl font-bold text-[#003d7a] mt-2">{{ total }}</p>
      </div>
      <div class="bg-white rounded-2xl shadow-lg p-6">
        <p class="text-sm text-gray-600">En el Filtro Actual</p>
        <p class="text-3xl font-bold text-green-600 mt-2">{{ registros.length }}</p>
      </div>
      <div class="bg-white rounded-2xl shadow-lg p-6">
        <p class="text-sm text-gray-600">Página</p>
        <p class="text-3xl font-bold text-yellow-600 mt-2">{{ pagina }} / {{ totalPaginas }}</p>
      </div>
    </div>

    <!-- Filtros -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Tipo de Acción</label>
          <select v-model="tipoAccion" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none">
            <option value="">Todos</option>
            <option value="Creacion">Creación</option>
            <option value="CambioUbicacion">Cambio de Ubicación</option>
            <option value="CambioEncargado">Cambio de Encargado</option>
            <option value="CambioEstado">Cambio de Estado</option>
            <option value="CambioPlaca">Cambio de Placa</option>
            <option value="Aprobacion">Aprobación</option>
            <option value="Eliminacion">Eliminación</option>
            <option value="Rechazo">Rechazo</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Usuario (correo)</label>
          <input v-model="usuarioCorreo" type="text" placeholder="usuario@ucr.ac.cr" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Desde</label>
          <input v-model="desde" type="date" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Hasta</label>
          <input v-model="hasta" type="date" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
        </div>
      </div>
    </div>

    <!-- Línea de tiempo -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <div v-if="loading" class="text-center py-10 text-gray-400">Cargando...</div>
      <div v-else-if="registros.length === 0" class="text-center py-10 text-gray-400">No hay registros con los filtros seleccionados.</div>
      <div v-else class="relative">
        <div class="absolute left-6 top-0 bottom-0 w-0.5 bg-gray-200" />
        <div class="space-y-6">
          <div v-for="reg in registros" :key="reg.id" class="relative flex gap-6">
            <div class="flex-shrink-0 w-12 h-12 bg-white border-4 border-gray-200 rounded-full flex items-center justify-center z-10">
              <span class="text-lg">{{ iconoAccion(reg.tipoAccion) }}</span>
            </div>
            <div class="flex-1 bg-gradient-to-br from-blue-50/30 to-white rounded-xl p-4 hover:shadow-lg transition-all duration-300 border border-blue-100/30">
              <div class="flex items-center gap-2 mb-2">
                <span class="px-3 py-1 rounded-full text-xs font-medium" :class="badgeAccion(reg.tipoAccion)">
                  {{ labelAccion(reg.tipoAccion) }}
                </span>
                <span class="text-xs text-gray-500">{{ formatFecha(reg.fechaHora) }}</span>
              </div>
              <p class="font-medium text-gray-900 mb-1">Placa: {{ reg.activoPlaca }}</p>
              <p v-if="reg.descripcion" class="text-sm text-gray-600">{{ reg.descripcion }}</p>
              <div class="flex items-center gap-2 mt-2 text-sm text-gray-500">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
                <span>{{ reg.usuarioNombre }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Paginación -->
      <div class="flex items-center justify-center gap-4 mt-6 pt-4 border-t border-gray-100">
        <button @click="pagina--" :disabled="pagina === 1" class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 transition">Anterior</button>
        <span class="text-sm text-gray-600">Página {{ pagina }} de {{ totalPaginas }}</span>
        <button @click="pagina++" :disabled="pagina >= totalPaginas" class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 transition">Siguiente</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import historialService from '@/services/historialService'

const registros = ref([])
const total = ref(0)
const pagina = ref(1)
const tamano = ref(20)
const tipoAccion = ref('')
const usuarioCorreo = ref('')
const desde = ref('')
const hasta = ref('')
const loading = ref(false)

const totalPaginas = computed(() => Math.max(1, Math.ceil(total.value / tamano.value)))

async function cargar() {
  loading.value = true
  try {
    const params = { pagina: pagina.value, tamano: tamano.value }
    if (tipoAccion.value) params.tipoAccion = tipoAccion.value
    if (usuarioCorreo.value) params.usuarioCorreo = usuarioCorreo.value
    if (desde.value) params.desde = desde.value
    if (hasta.value) params.hasta = hasta.value
    const { data } = await historialService.listar(params)
    registros.value = data.items
    total.value = data.total
  } finally {
    loading.value = false
  }
}

onMounted(cargar)
watch([pagina], cargar)
watch([tipoAccion, usuarioCorreo, desde, hasta], () => { pagina.value = 1; cargar() })

const ICONOS = { Creacion: '✨', CambioUbicacion: '📍', CambioEncargado: '👤', CambioEstado: '🔄', CambioPlaca: '🏷️', Eliminacion: '🗑️', Aprobacion: '✅', Rechazo: '❌' }
const LABELS = { Creacion: 'Creación', CambioUbicacion: 'Cambio de Ubicación', CambioEncargado: 'Cambio de Encargado', CambioEstado: 'Cambio de Estado', CambioPlaca: 'Cambio de Placa', Eliminacion: 'Eliminación', Aprobacion: 'Aprobación', Rechazo: 'Rechazo' }
const BADGES = { Creacion: 'bg-indigo-100 text-indigo-800', CambioUbicacion: 'bg-blue-100 text-blue-800', CambioEncargado: 'bg-green-100 text-green-800', CambioEstado: 'bg-yellow-100 text-yellow-800', CambioPlaca: 'bg-purple-100 text-purple-800', Eliminacion: 'bg-red-100 text-red-800', Aprobacion: 'bg-teal-100 text-teal-800', Rechazo: 'bg-orange-100 text-orange-800' }

function iconoAccion(tipo) { return ICONOS[tipo] || '📋' }
function labelAccion(tipo) { return LABELS[tipo] || tipo }
function badgeAccion(tipo) { return BADGES[tipo] || 'bg-gray-100 text-gray-800' }
function formatFecha(iso) { return new Date(iso).toLocaleString('es-CR', { dateStyle: 'short', timeStyle: 'short' }) }
</script>

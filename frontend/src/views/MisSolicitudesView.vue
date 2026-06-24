<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-[#003d7a]">Mis Solicitudes de Cambio</h1>
      <p class="text-gray-600 mt-1">Seguimiento de los cambios propuestos a activos</p>
    </div>

    <!-- Tabs -->
    <div class="flex gap-1 bg-white/90 backdrop-blur-sm rounded-xl p-1 border border-blue-100/50 shadow-sm w-fit">
      <button
        v-for="tab in tabs"
        :key="tab.value"
        @click="estadoFiltro = tab.value"
        :class="estadoFiltro === tab.value
          ? 'bg-[#003d7a] text-white shadow'
          : 'text-gray-600 hover:bg-gray-100'"
        class="px-5 py-2 rounded-lg text-sm font-medium transition flex items-center gap-2"
      >
        {{ tab.label }}
        <span v-if="tab.value === 'Pendiente' && pendienteCount > 0"
          class="bg-amber-400 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center">
          {{ pendienteCount > 9 ? '9+' : pendienteCount }}
        </span>
      </button>
    </div>

    <!-- Estado vacío -->
    <div v-if="cargando" class="text-center py-16 text-gray-400">Cargando...</div>
    <div v-else-if="solicitudes.length === 0"
      class="bg-white/90 rounded-2xl shadow-lg border border-blue-100/50 px-8 py-16 text-center">
      <svg xmlns="http://www.w3.org/2000/svg" class="w-12 h-12 text-gray-300 mx-auto mb-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
      </svg>
      <p class="text-gray-400">No tienes solicitudes {{ tab(estadoFiltro) }}.</p>
      <button v-if="estadoFiltro !== 'Pendiente'" @click="estadoFiltro = 'Pendiente'"
        class="mt-3 text-sm text-[#0066cc] hover:underline">Ver pendientes</button>
    </div>

    <!-- Lista -->
    <div v-else class="space-y-4">
      <div v-for="s in solicitudes" :key="s.id"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg border overflow-hidden"
        :class="estadoBorde(s.estado)">

        <!-- Cabecera -->
        <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100">
          <div class="flex items-center gap-3">
            <div>
              <span class="font-bold text-[#003d7a] font-mono">{{ s.activoPlaca }}</span>
              <span class="mx-2 text-gray-300">·</span>
              <span class="text-gray-700 font-medium">{{ s.articuloActual }}</span>
            </div>
            <span :class="estadoBadge(s.estado)" class="px-3 py-0.5 rounded-full text-xs font-semibold">
              {{ s.estado }}
            </span>
          </div>
          <span class="text-xs text-gray-400">{{ formatFecha(s.fechaSolicitud) }}</span>
        </div>

        <!-- Cambios propuestos -->
        <div class="px-6 py-4 space-y-2">
          <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Cambios propuestos</p>
          <div class="grid grid-cols-2 gap-x-8 gap-y-1.5">
            <template v-for="campo in calcularCambios(s)" :key="campo.label">
              <div class="flex items-baseline gap-2 col-span-1">
                <span class="text-xs text-gray-400 w-20 flex-shrink-0">{{ campo.label }}</span>
                <span v-if="campo.cambia" class="text-sm">
                  <span class="line-through text-gray-400">{{ campo.actual }}</span>
                  <span class="italic font-medium text-amber-700 ml-1">→ {{ campo.propuesto }}</span>
                </span>
                <span v-else class="text-sm text-gray-500 italic">sin cambio</span>
              </div>
            </template>
          </div>
        </div>

        <!-- Resolución -->
        <div v-if="s.estado !== 'Pendiente'"
          class="px-6 py-3 border-t border-gray-100 flex items-center gap-3"
          :class="s.estado === 'Aprobada' ? 'bg-green-50/60' : 'bg-red-50/60'">
          <svg v-if="s.estado === 'Aprobada'" xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-green-600 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
          <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-red-500 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
          <p class="text-sm" :class="s.estado === 'Aprobada' ? 'text-green-700' : 'text-red-600'">
            <span class="font-medium">{{ s.estado === 'Aprobada' ? 'Aprobado' : 'Rechazado' }}</span>
            por {{ s.revisorNombre }} · {{ formatFecha(s.fechaResolucion) }}
            <span v-if="s.comentario" class="italic ml-1">"{{ s.comentario }}"</span>
          </p>
        </div>

        <!-- Pendiente -->
        <div v-else class="px-6 py-3 border-t border-amber-100 bg-amber-50/40 flex items-center gap-2">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-amber-500 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-sm text-amber-700 italic">En espera de revisión por Administradora o GTI</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import solicitudService from '@/services/solicitudService'

const tabs = [
  { label: 'Pendientes', value: 'Pendiente' },
  { label: 'Aprobadas',  value: 'Aprobada' },
  { label: 'Rechazadas', value: 'Rechazada' }
]

const estadoFiltro = ref('Pendiente')
const solicitudes = ref([])
const cargando = ref(false)

const pendienteCount = computed(() =>
  estadoFiltro.value === 'Pendiente' ? solicitudes.value.length : 0
)

async function cargar() {
  cargando.value = true
  try {
    const { data } = await solicitudService.listarMias(estadoFiltro.value)
    solicitudes.value = data
  } finally {
    cargando.value = false
  }
}

onMounted(cargar)
watch(estadoFiltro, cargar)

function calcularCambios(s) {
  const d = s.datosNuevos
  const a = s
  return [
    { label: 'Artículo',  actual: a.articuloActual,  propuesto: d.articulo,        cambia: d.articulo !== a.articuloActual },
    { label: 'Marca',     actual: a.marcaActual,     propuesto: d.marca,           cambia: d.marca !== a.marcaActual },
    { label: 'Modelo',    actual: a.modeloActual,    propuesto: d.modelo,          cambia: d.modelo !== a.modeloActual },
    { label: 'Ubicación', actual: a.ubicacionActual, propuesto: d.ubicacionActual, cambia: d.ubicacionActual !== a.ubicacionActual },
    { label: 'Encargado', actual: a.encargadoActual, propuesto: d.encargadoNombre, cambia: d.encargadoNombre !== a.encargadoActual },
  ].filter(c => c.cambia || true)
}

function tab(v) {
  return { Pendiente: 'pendientes', Aprobada: 'aprobadas', Rechazada: 'rechazadas' }[v] || ''
}

function formatFecha(iso) {
  if (!iso) return '—'
  return new Date(iso).toLocaleDateString('es-CR', { day: '2-digit', month: 'short', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

const estadoBadges = {
  Pendiente: 'bg-amber-100 text-amber-700',
  Aprobada:  'bg-green-100 text-green-700',
  Rechazada: 'bg-red-100 text-red-700'
}
const estadoBordes = {
  Pendiente: 'border-amber-200/60',
  Aprobada:  'border-green-200/60',
  Rechazada: 'border-red-200/60'
}
function estadoBadge(e) { return estadoBadges[e] || 'bg-gray-100 text-gray-600' }
function estadoBorde(e) { return estadoBordes[e] || 'border-blue-100/50' }
</script>

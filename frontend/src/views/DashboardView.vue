<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-[#003d7a]">Dashboard</h1>
      <p class="text-gray-600 mt-1">Resumen general del inventario institucional</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-6">
      <!-- Activos -->
      <div @click="router.push('/inventario')"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-blue-100/50 bg-gradient-to-br from-white to-blue-50/50 transition-all duration-300 cursor-pointer hover:scale-105 hover:border-[#0066cc]/30">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500 font-medium">Activos</p>
            <p class="text-3xl font-bold text-[#003d7a] mt-2">{{ stats.totalActivos }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-[#4da6ff] to-[#0066cc] rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
          </div>
        </div>
        <p class="text-xs text-[#0066cc] mt-3 font-medium">Ver inventario →</p>
      </div>

      <!-- Desecho -->
      <div @click="router.push('/desecho')"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-red-100/50 bg-gradient-to-br from-white to-red-50/50 transition-all duration-300 cursor-pointer hover:scale-105 hover:border-red-300/50">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500 font-medium">Desecho</p>
            <p class="text-3xl font-bold text-red-600 mt-2">{{ stats.enDesecho }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-red-400 to-red-600 rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </div>
        </div>
        <p class="text-xs text-red-500 mt-3 font-medium">Ver desecho →</p>
      </div>

      <!-- Categorías -->
      <div @click="router.push('/categorias')"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-purple-100/50 bg-gradient-to-br from-white to-purple-50/50 transition-all duration-300 cursor-pointer hover:scale-105 hover:border-purple-300/50">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500 font-medium">Categorías</p>
            <p class="text-3xl font-bold text-purple-700 mt-2">{{ stats.porCategoria.length }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-purple-400 to-purple-600 rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7a2 2 0 012-2h4l2 2h8a2 2 0 012 2v7a2 2 0 01-2 2H5a2 2 0 01-2-2V7z" />
            </svg>
          </div>
        </div>
        <p class="text-xs text-purple-500 mt-3 font-medium">Ver categorías →</p>
      </div>

      <!-- Encargados -->
      <div @click="router.push('/encargados')"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg hover:shadow-xl p-6 border border-green-100/50 bg-gradient-to-br from-white to-green-50/50 transition-all duration-300 cursor-pointer hover:scale-105 hover:border-green-300/50">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-500 font-medium">Encargados</p>
            <p class="text-3xl font-bold text-green-700 mt-2">{{ totalEncargados }}</p>
          </div>
          <div class="w-12 h-12 bg-gradient-to-br from-green-400 to-green-600 rounded-full flex items-center justify-center shadow-lg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </div>
        </div>
        <p class="text-xs text-green-600 mt-3 font-medium">Ver encargados →</p>
      </div>
    </div>

    <!-- Categorías -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <h3 class="text-lg font-semibold text-[#003d7a] mb-4">Activos por Categoría</h3>
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-5 gap-4">
        <div
          v-for="cat in stats.porCategoria"
          :key="cat.id"
          @click="seleccionarCategoria(cat)"
          class="border rounded-xl p-4 hover:shadow-lg transition-all duration-300 cursor-pointer bg-gradient-to-br hover:scale-105"
          :class="categoriaSeleccionada?.id === cat.id
            ? 'border-[#0066cc] bg-gradient-to-br from-blue-100 to-blue-50 shadow-md ring-2 ring-[#0066cc]/30'
            : 'border-blue-100/50 from-blue-50/30 to-white'"
        >
          <p class="text-2xl mb-2 emoji-font">{{ cat.icono || defaultEmoji }}</p>
          <p class="font-semibold text-gray-800 text-sm">{{ cat.nombre }}</p>
          <p class="text-2xl font-bold text-[#003d7a]">{{ cat.cantidad }}</p>
        </div>
      </div>

      <!-- Panel de activos recientes por categoría -->
      <Transition
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-200 ease-in"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2"
      >
        <div v-if="categoriaSeleccionada" class="mt-6 border-t border-blue-100 pt-5">
          <div class="flex items-center justify-between mb-3">
            <div class="flex items-center gap-2">
              <span class="text-xl emoji-font">{{ categoriaSeleccionada.icono || defaultEmoji }}</span>
              <h4 class="font-semibold text-[#003d7a]">{{ categoriaSeleccionada.nombre }}</h4>
            </div>
            <button @click="categoriaSeleccionada = null" class="text-gray-400 hover:text-gray-600 transition">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <p class="text-xs text-gray-500 mb-3 flex items-center gap-1">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            Mostrando los {{ activosRecientes.length }} activos más recientes de esta categoría. Para ver el listado completo, ingrese al módulo de Inventario.
          </p>

          <div v-if="cargandoRecientes" class="py-6 text-center text-gray-400 text-sm">Cargando...</div>
          <div v-else-if="activosRecientes.length === 0" class="py-6 text-center text-gray-400 text-sm">No hay activos en esta categoría.</div>
          <div v-else class="overflow-x-auto rounded-xl border border-blue-100/60">
            <table class="w-full text-sm">
              <thead class="bg-blue-50/60">
                <tr>
                  <th class="text-left px-4 py-2.5 text-xs font-semibold text-gray-500 uppercase tracking-wide">Placa</th>
                  <th class="text-left px-4 py-2.5 text-xs font-semibold text-gray-500 uppercase tracking-wide">Artículo</th>
                  <th class="text-left px-4 py-2.5 text-xs font-semibold text-gray-500 uppercase tracking-wide">Marca / Modelo</th>
                  <th class="text-left px-4 py-2.5 text-xs font-semibold text-gray-500 uppercase tracking-wide">Ubicación</th>
                  <th class="text-left px-4 py-2.5 text-xs font-semibold text-gray-500 uppercase tracking-wide">Estado</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-blue-50">
                <tr v-for="activo in activosRecientes" :key="activo.placa" class="hover:bg-blue-50/30 transition">
                  <td class="px-4 py-3 font-mono text-[#003d7a] font-medium">{{ activo.placa }}</td>
                  <td class="px-4 py-3 text-gray-800">{{ activo.articulo }}</td>
                  <td class="px-4 py-3 text-gray-600">{{ activo.marca }} {{ activo.modelo }}</td>
                  <td class="px-4 py-3 text-gray-600">{{ activo.ubicacionActual }}</td>
                  <td class="px-4 py-3">
                    <span class="px-2 py-0.5 rounded-full text-xs font-semibold"
                      :class="{
                        'bg-green-100 text-green-700': activo.estado === 'Activo',
                        'bg-yellow-100 text-yellow-700': activo.estado === 'Mantenimiento',
                        'bg-red-100 text-red-700': activo.estado === 'Desecho'
                      }">
                      {{ activo.estado }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </Transition>
    </div>

    <!-- Actividad Reciente -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <h3 class="text-lg font-semibold text-[#003d7a] mb-4">Actividad Reciente</h3>
      <div v-if="actividad.length === 0" class="text-center py-8 text-gray-400">
        No hay actividad reciente.
      </div>
      <div v-else class="space-y-1">
        <template v-for="item in actividad" :key="item.id">
          <div
            @click="seleccionarActividad(item)"
            class="flex items-start gap-4 p-3 rounded-xl transition-all duration-200 cursor-pointer"
            :class="actividadSeleccionada?.id === item.id
              ? 'bg-blue-50 ring-1 ring-[#0066cc]/20'
              : 'hover:bg-blue-50/30'"
          >
            <div class="w-2 h-2 rounded-full mt-2 flex-shrink-0"
              :class="actividadSeleccionada?.id === item.id ? 'bg-[#003d7a]' : 'bg-[#0066cc]'" />
            <div class="flex-1 min-w-0">
              <p class="font-medium text-gray-800">{{ labelAccion(item.tipoAccion) }}</p>
              <p class="text-sm text-gray-600">Placa: {{ item.activoPlaca }}</p>
              <p class="text-xs text-gray-500 mt-1">
                {{ item.usuarioNombre }} • {{ formatFecha(item.fechaHora) }}
              </p>
            </div>
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-gray-400 flex-shrink-0 mt-1 transition-transform duration-200"
              :class="actividadSeleccionada?.id === item.id ? 'rotate-180 text-[#0066cc]' : ''"
              fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
            </svg>
          </div>

          <!-- Resumen del activo -->
          <Transition
            enter-active-class="transition-all duration-200 ease-out"
            enter-from-class="opacity-0 -translate-y-1"
            enter-to-class="opacity-100 translate-y-0"
            leave-active-class="transition-all duration-150 ease-in"
            leave-from-class="opacity-100 translate-y-0"
            leave-to-class="opacity-0 -translate-y-1"
          >
            <div v-if="actividadSeleccionada?.id === item.id" class="mx-3 mb-2 rounded-xl border border-blue-100 bg-gradient-to-br from-blue-50/60 to-white overflow-hidden">
              <div v-if="cargandoDetalle" class="py-4 text-center text-sm text-gray-400">Cargando...</div>
              <div v-else-if="activoDetalle" class="p-4">
                <div class="flex items-start justify-between gap-2 mb-3">
                  <div class="flex items-start gap-3 flex-1 min-w-0">
                    <div>
                      <p class="font-semibold text-[#003d7a] font-mono">{{ activoDetalle.placa }}</p>
                      <p class="text-sm text-gray-700">{{ activoDetalle.articulo }}</p>
                    </div>
                    <div class="border-l border-blue-100 pl-3">
                      <p class="text-xs text-gray-400 uppercase tracking-wide font-medium">Realizado por</p>
                      <p class="font-semibold text-gray-800 text-sm">{{ actividadSeleccionada.usuarioNombre }}</p>
                    </div>
                  </div>
                  <span class="px-2 py-0.5 rounded-full text-xs font-semibold flex-shrink-0"
                    :class="{
                      'bg-green-100 text-green-700': activoDetalle.estado === 'Activo',
                      'bg-yellow-100 text-yellow-700': activoDetalle.estado === 'Mantenimiento',
                      'bg-red-100 text-red-700': activoDetalle.estado === 'Desecho'
                    }">
                    {{ activoDetalle.estado }}
                  </span>
                </div>
                <div class="grid grid-cols-2 gap-x-6 gap-y-1.5 text-xs">
                  <div class="flex flex-col">
                    <span class="text-gray-400 uppercase tracking-wide font-medium">Marca / Modelo</span>
                    <span class="text-gray-700">{{ activoDetalle.marca }} {{ activoDetalle.modelo }}</span>
                  </div>
                  <div class="flex flex-col">
                    <span class="text-gray-400 uppercase tracking-wide font-medium">Categoría</span>
                    <span class="text-gray-700">{{ activoDetalle.categoriaNombre }}</span>
                  </div>
                  <div class="flex flex-col">
                    <span class="text-gray-400 uppercase tracking-wide font-medium">Ubicación</span>
                    <span class="text-gray-700">{{ activoDetalle.ubicacionActual }}</span>
                  </div>
                  <div class="flex flex-col">
                    <span class="text-gray-400 uppercase tracking-wide font-medium">Encargado</span>
                    <span class="text-gray-700">{{ activoDetalle.encargadoActual }}</span>
                  </div>
                </div>
              </div>
              <div v-else class="py-4 text-center text-sm text-gray-400">No se pudo cargar el activo.</div>
            </div>
          </Transition>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import activoService from '@/services/activoService'
import historialService from '@/services/historialService'
import encargadoService from '@/services/encargadoService'

const router = useRouter()
const defaultEmoji = String.fromCodePoint(0x1F4E6)
const stats = ref({ totalActivos: 0, enDesecho: 0, solicitudesPendientes: 0, porCategoria: [] })
const totalEncargados = ref(0)
const actividad = ref([])
const categoriaSeleccionada = ref(null)
const activosRecientes = ref([])
const cargandoRecientes = ref(false)
const actividadSeleccionada = ref(null)
const activoDetalle = ref(null)
const cargandoDetalle = ref(false)

onMounted(async () => {
  const [statsRes, histRes, encRes] = await Promise.all([
    activoService.stats(),
    historialService.listar({ pagina: 1, tamano: 6 }),
    encargadoService.listar()
  ])
  stats.value = statsRes.data
  actividad.value = histRes.data.items
  totalEncargados.value = encRes.data.length
})

async function seleccionarCategoria(cat) {
  if (categoriaSeleccionada.value?.id === cat.id) {
    categoriaSeleccionada.value = null
    return
  }
  categoriaSeleccionada.value = cat
  cargandoRecientes.value = true
  activosRecientes.value = []
  try {
    const res = await activoService.recientes(cat.id)
    activosRecientes.value = res.data
  } finally {
    cargandoRecientes.value = false
  }
}

async function seleccionarActividad(item) {
  if (actividadSeleccionada.value?.id === item.id) {
    actividadSeleccionada.value = null
    activoDetalle.value = null
    return
  }
  actividadSeleccionada.value = item
  activoDetalle.value = null
  cargandoDetalle.value = true
  try {
    const res = await activoService.obtener(item.activoPlaca)
    activoDetalle.value = res.data
  } catch {
    activoDetalle.value = null
  } finally {
    cargandoDetalle.value = false
  }
}

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

<style scoped>
.emoji-font {
  font-family: 'Segoe UI Emoji', 'Apple Color Emoji', 'Noto Color Emoji', 'Twemoji Mozilla', sans-serif;
}
</style>

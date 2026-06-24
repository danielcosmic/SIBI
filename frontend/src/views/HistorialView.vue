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
            <option value="SolicitudCambio">Solicitud de Cambio</option>
            <option value="SolicitudAprobada">Solicitud Aprobada</option>
            <option value="SolicitudRechazada">Solicitud Rechazada</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Nombre de usuario</label>
          <input v-model="usuarioNombre" type="text" placeholder="Buscar por nombre..." class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
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
        <div class="absolute left-6 top-0 bottom-0 w-0.5 bg-gray-300" />
        <div class="space-y-4">
          <div v-for="reg in registros" :key="reg.id" class="relative flex gap-6">
            <div class="flex-shrink-0 w-12 h-12 bg-white border-4 border-[#003d7a]/30 rounded-full flex items-center justify-center z-10 shadow-sm">
              <span class="text-lg">{{ iconoAccion(reg.tipoAccion) }}</span>
            </div>
            <div
              @click="abrirDetalle(reg)"
              class="flex-1 bg-white rounded-xl p-4 shadow-sm hover:shadow-md transition-all duration-200 border border-gray-200 hover:border-blue-300 cursor-pointer"
            >
              <div class="flex items-center gap-2 mb-2">
                <span class="px-3 py-1 rounded-full text-xs font-medium" :class="badgeAccion(reg.tipoAccion)">
                  {{ labelAccion(reg.tipoAccion) }}
                </span>
                <span class="text-xs text-gray-500">{{ formatFecha(reg.fechaHora) }}</span>
              </div>
              <p class="font-medium text-gray-900 mb-1">Placa: {{ reg.activoPlaca }}</p>
              <p v-if="reg.descripcion" class="text-sm text-gray-600">{{ reg.descripcion }}</p>
              <div class="flex items-center justify-between mt-2">
                <div class="flex items-center gap-2 text-sm text-gray-500">
                  <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
                  <span>{{ reg.usuarioNombre }}</span>
                </div>
                <span class="text-xs text-[#0066cc] font-medium">Ver detalles →</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Paginación -->
      <div v-if="totalPaginas > 1" class="flex items-center justify-between mt-6 pt-4 border-t border-gray-100 gap-2 flex-wrap">
        <p class="text-sm text-gray-500">
          {{ (pagina - 1) * tamano + 1 }}–{{ Math.min(pagina * tamano, total) }} de {{ total }} registros
        </p>
        <div class="flex items-center gap-1">
          <!-- Primera -->
          <button @click="pagina = 1" :disabled="pagina === 1"
            class="px-2.5 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-40 transition">
            «
          </button>
          <!-- Anterior -->
          <button @click="pagina--" :disabled="pagina === 1"
            class="px-2.5 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-40 transition">
            ‹
          </button>
          <!-- Números -->
          <template v-for="p in paginasVisibles" :key="p">
            <span v-if="p === '...'" class="px-2 text-gray-400 select-none">…</span>
            <button v-else @click="pagina = p"
              class="px-3 py-1.5 text-sm border rounded-lg transition font-medium"
              :class="p === pagina
                ? 'bg-[#003d7a] text-white border-[#003d7a]'
                : 'border-gray-300 hover:bg-gray-100 text-gray-700'">
              {{ p }}
            </button>
          </template>
          <!-- Siguiente -->
          <button @click="pagina++" :disabled="pagina >= totalPaginas"
            class="px-2.5 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-40 transition">
            ›
          </button>
          <!-- Última -->
          <button @click="pagina = totalPaginas" :disabled="pagina === totalPaginas"
            class="px-2.5 py-1.5 text-sm border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-40 transition">
            »
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Modal detalle del activo -->
  <Teleport to="body">
    <div v-if="modalAbierto" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] flex flex-col">

        <!-- Header -->
        <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl flex-shrink-0">
          <div>
            <h2 class="text-xl font-bold">Detalle del Activo</h2>
            <p class="text-blue-200 text-sm mt-0.5">{{ registroActivo?.activoPlaca }}</p>
          </div>
          <button @click="cerrarDetalle" class="p-1 hover:bg-white/10 rounded transition">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Cuerpo scrollable -->
        <div class="overflow-y-auto flex-1 p-4 space-y-3">

          <!-- Evento del historial -->
          <div class="rounded-xl border border-blue-200 bg-blue-50 p-4">
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-2">Evento registrado</p>
            <div class="flex flex-wrap items-center gap-2 mb-2">
              <span class="text-lg">{{ iconoAccion(registroActivo?.tipoAccion) }}</span>
              <span class="px-3 py-1 rounded-full text-xs font-medium" :class="badgeAccion(registroActivo?.tipoAccion)">
                {{ labelAccion(registroActivo?.tipoAccion) }}
              </span>
              <span class="text-sm text-gray-500">{{ formatFecha(registroActivo?.fechaHora) }}</span>
            </div>
            <p v-if="registroActivo?.descripcion" class="text-sm text-gray-700 mb-1">{{ registroActivo.descripcion }}</p>
            <p class="text-sm text-gray-500 flex items-center gap-1.5">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              {{ registroActivo?.usuarioNombre }}
            </p>
          </div>

          <!-- Estado de carga -->
          <div v-if="cargandoDetalle" class="py-8 text-center text-gray-400">Cargando datos del activo...</div>

          <!-- Activo eliminado o no encontrado -->
          <div v-else-if="activoNoEncontrado" class="py-8 text-center">
            <p class="text-gray-500 text-sm">Este activo ya no existe en el sistema (fue eliminado definitivamente).</p>
          </div>

          <!-- Datos del activo -->
          <template v-else-if="activoDetalle">
            <!-- Identificación -->
            <div class="rounded-xl border border-gray-200 overflow-hidden">
              <p class="text-xs font-semibold text-white uppercase tracking-wider bg-[#003d7a] px-4 py-2">Identificación</p>
              <div class="grid grid-cols-2 gap-3 p-4">
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Placa</p>
                  <p class="font-semibold text-[#003d7a] font-mono">{{ activoDetalle.placa }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Tipo de Placa</p>
                  <p class="font-medium text-gray-800">{{ activoDetalle.tipoPlaca }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Artículo</p>
                  <p class="font-medium text-gray-800">{{ activoDetalle.articulo }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Número Serial</p>
                  <p class="font-medium text-gray-800 font-mono text-sm">{{ activoDetalle.numSerial }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Marca</p>
                  <p class="font-medium text-gray-800">{{ activoDetalle.marca }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Modelo</p>
                  <p class="font-medium text-gray-800">{{ activoDetalle.modelo }}</p>
                </div>
              </div>
            </div>

            <!-- Clasificación -->
            <div class="rounded-xl border border-gray-200 overflow-hidden">
              <p class="text-xs font-semibold text-white uppercase tracking-wider bg-[#003d7a] px-4 py-2">Clasificación</p>
              <div class="grid grid-cols-2 gap-3 p-4">
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Categoría</p>
                  <p class="font-medium text-gray-800">{{ activoDetalle.categoriaNombre }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Estado</p>
                  <span class="inline-block px-2 py-0.5 rounded-full text-xs font-semibold mt-0.5"
                    :class="{
                      'bg-green-100 text-green-700': activoDetalle.estado === 'Activo',
                      'bg-yellow-100 text-yellow-700': activoDetalle.estado === 'Mantenimiento',
                      'bg-red-100 text-red-700': activoDetalle.estado === 'Desecho'
                    }">
                    {{ activoDetalle.estado }}
                  </span>
                </div>
                <div v-if="activoDetalle.fechaDesecho" class="bg-red-50 rounded-lg p-3 col-span-2">
                  <p class="text-xs text-red-400 mb-0.5">Fecha de Desecho</p>
                  <p class="font-medium text-red-700">{{ activoDetalle.fechaDesecho }}</p>
                </div>
              </div>
            </div>

            <!-- Ubicación -->
            <div class="rounded-xl border border-gray-200 overflow-hidden">
              <p class="text-xs font-semibold text-white uppercase tracking-wider bg-[#003d7a] px-4 py-2">Ubicación</p>
              <div class="grid grid-cols-2 gap-3 p-4">
                <div class="bg-blue-50 rounded-lg p-3 border border-blue-200">
                  <p class="text-xs text-blue-500 mb-0.5">Ubicación Actual</p>
                  <p class="font-semibold text-blue-900">{{ activoDetalle.ubicacionActual }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Ubicación Anterior</p>
                  <p class="font-medium text-gray-600">{{ activoDetalle.ubicacionAnterior || '—' }}</p>
                </div>
              </div>
            </div>

            <!-- Encargados -->
            <div class="rounded-xl border border-gray-200 overflow-hidden">
              <p class="text-xs font-semibold text-white uppercase tracking-wider bg-[#003d7a] px-4 py-2">Encargados</p>
              <div class="grid grid-cols-2 gap-3 p-4">
                <div class="bg-blue-50 rounded-lg p-3 border border-blue-200">
                  <p class="text-xs text-blue-500 mb-0.5">Encargado Actual</p>
                  <p class="font-semibold text-blue-900">{{ activoDetalle.encargadoActual }}</p>
                </div>
                <div class="bg-gray-50 rounded-lg p-3">
                  <p class="text-xs text-gray-400 mb-0.5">Encargado Anterior</p>
                  <p class="font-medium text-gray-600">{{ activoDetalle.encargadoAnterior || '—' }}</p>
                </div>
              </div>
            </div>

            <!-- Observaciones -->
            <div v-if="activoDetalle.observaciones" class="rounded-xl border border-gray-200 overflow-hidden">
              <p class="text-xs font-semibold text-white uppercase tracking-wider bg-[#003d7a] px-4 py-2">Observaciones</p>
              <div class="p-4 bg-gray-50">
                <p class="text-sm text-gray-700">{{ activoDetalle.observaciones }}</p>
              </div>
            </div>
          </template>
        </div>

        <!-- Footer -->
        <div class="px-6 py-4 border-t border-gray-200 flex-shrink-0 flex gap-3">
          <button @click="cerrarDetalle"
            class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
            Cerrar
          </button>
          <button v-if="!activoNoEncontrado && activoDetalle"
            @click="router.push('/activo/' + registroActivo?.activoPlaca); cerrarDetalle()"
            class="flex items-center justify-center gap-1.5 flex-1 px-4 py-2.5 text-[#0066cc] border border-[#0066cc]/30 rounded-lg hover:bg-blue-50 transition font-medium text-sm">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
            </svg>
            Ver Detalle
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import historialService from '@/services/historialService'
import activoService from '@/services/activoService'

const router = useRouter()

const registros = ref([])
const total = ref(0)
const pagina = ref(1)
const tamano = ref(20)
const tipoAccion = ref('')
const usuarioNombre = ref('')
const desde = ref('')
const hasta = ref('')
const loading = ref(false)

const totalPaginas = computed(() => Math.max(1, Math.ceil(total.value / tamano.value)))

const paginasVisibles = computed(() => {
  const total = totalPaginas.value
  const cur = pagina.value
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1)
  const pages = []
  pages.push(1)
  if (cur > 3) pages.push('...')
  for (let p = Math.max(2, cur - 1); p <= Math.min(total - 1, cur + 1); p++) pages.push(p)
  if (cur < total - 2) pages.push('...')
  pages.push(total)
  return pages
})

const modalAbierto = ref(false)
const registroActivo = ref(null)
const activoDetalle = ref(null)
const activoNoEncontrado = ref(false)
const cargandoDetalle = ref(false)

async function abrirDetalle(reg) {
  registroActivo.value = reg
  activoDetalle.value = null
  activoNoEncontrado.value = false
  cargandoDetalle.value = true
  modalAbierto.value = true
  try {
    const { data } = await activoService.obtener(reg.activoPlaca)
    activoDetalle.value = data
  } catch (e) {
    activoNoEncontrado.value = true
  } finally {
    cargandoDetalle.value = false
  }
}

function cerrarDetalle() {
  modalAbierto.value = false
  registroActivo.value = null
  activoDetalle.value = null
}

async function cargar() {
  loading.value = true
  try {
    const params = { pagina: pagina.value, tamano: tamano.value }
    if (tipoAccion.value) params.tipoAccion = tipoAccion.value
    if (usuarioNombre.value) params.usuarioNombre = usuarioNombre.value
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
watch([tipoAccion, usuarioNombre, desde, hasta], () => { pagina.value = 1; cargar() })

const ICONOS = { Creacion: '✨', CambioUbicacion: '📍', CambioEncargado: '👤', CambioEstado: '🔄', CambioPlaca: '🏷️', Eliminacion: '🗑️', Aprobacion: '✅', Rechazo: '❌', SolicitudCambio: '📝', SolicitudAprobada: '✔️', SolicitudRechazada: '🚫' }
const LABELS = { Creacion: 'Creación', CambioUbicacion: 'Cambio de Ubicación', CambioEncargado: 'Cambio de Encargado', CambioEstado: 'Cambio de Estado', CambioPlaca: 'Cambio de Placa', Eliminacion: 'Eliminación', Aprobacion: 'Aprobación', Rechazo: 'Rechazo', SolicitudCambio: 'Solicitud de Cambio', SolicitudAprobada: 'Solicitud Aprobada', SolicitudRechazada: 'Solicitud Rechazada' }
const BADGES = { Creacion: 'bg-indigo-100 text-indigo-800', CambioUbicacion: 'bg-blue-100 text-blue-800', CambioEncargado: 'bg-green-100 text-green-800', CambioEstado: 'bg-yellow-100 text-yellow-800', CambioPlaca: 'bg-purple-100 text-purple-800', Eliminacion: 'bg-red-100 text-red-800', Aprobacion: 'bg-teal-100 text-teal-800', Rechazo: 'bg-orange-100 text-orange-800', SolicitudCambio: 'bg-amber-100 text-amber-800', SolicitudAprobada: 'bg-green-100 text-green-800', SolicitudRechazada: 'bg-red-100 text-red-800' }

function iconoAccion(tipo) { return ICONOS[tipo] || '📋' }
function labelAccion(tipo) { return LABELS[tipo] || tipo }
function badgeAccion(tipo) { return BADGES[tipo] || 'bg-gray-100 text-gray-800' }
function formatFecha(iso) { return new Date(iso).toLocaleString('es-CR', { dateStyle: 'short', timeStyle: 'short' }) }
</script>

<template>
  <div class="space-y-6">
    <!-- Botón volver -->
    <button @click="router.push('/inventario')"
      class="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-200 rounded-lg hover:bg-gray-50 hover:border-blue-300 hover:text-[#003d7a] transition shadow-sm w-fit">
      <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
      </svg>
      Volver al Inventario
    </button>

    <!-- Loading -->
    <div v-if="cargando" class="text-center py-24 text-gray-400">Cargando...</div>

    <!-- Not found -->
    <div v-else-if="!activo" class="text-center py-24">
      <p class="text-gray-400 text-lg">Activo no encontrado.</p>
      <button @click="router.push('/inventario')" class="mt-4 text-[#0066cc] hover:underline text-sm">
        ← Volver al inventario
      </button>
    </div>

    <template v-else>
      <!-- Header con acciones -->
      <div class="flex items-start justify-between gap-4 flex-wrap">
        <div>
          <div class="flex items-center gap-3 flex-wrap">
            <h1 class="text-3xl font-bold text-[#003d7a]">{{ activo.articulo }}</h1>
            <span class="px-3 py-1 rounded-full text-sm font-semibold"
              :class="{
                'bg-green-100 text-green-700': activo.estado === 'Activo',
                'bg-yellow-100 text-yellow-700': activo.estado === 'Mantenimiento',
                'bg-red-100 text-red-700': activo.estado === 'Desecho'
              }">{{ activo.estado }}</span>
          </div>
          <p class="text-gray-500 mt-1 text-sm">
            <span class="font-mono font-medium text-gray-700">{{ activo.placa }}</span>
            <span class="mx-2 text-gray-300">·</span>
            <span>{{ activo.tipoPlaca }}</span>
            <span class="mx-2 text-gray-300">·</span>
            <span>{{ activo.categoriaNombre }}</span>
          </p>
        </div>
        <div class="flex gap-2 flex-wrap">
          <button
            v-if="puedeEliminarReciente && (auth.esGTI || auth.esAdministradora || auth.esJefaAdministrativa)"
            @click="eliminarReciente"
            :disabled="eliminandoReciente"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition font-medium text-sm flex items-center gap-2 disabled:bg-gray-400">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
            {{ eliminandoReciente ? 'Eliminando...' : `Eliminar (${tiempoRestanteElim})` }}
          </button>
          <button
            v-if="auth.esJefaAdministrativa && activo.estado !== 'Desecho' && !solicitudPendiente"
            @click="abrirModal('solicitud')"
            class="px-4 py-2 bg-amber-500 text-white rounded-lg hover:bg-amber-600 transition font-medium text-sm flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" />
            </svg>
            Solicitar Cambio
          </button>
          <button
            v-if="auth.esGTI && activo.estado !== 'Desecho'"
            @click="confirmarDesecho"
            class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition font-medium text-sm flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
            Desecho
          </button>
          <button
            v-if="auth.esGTI && activo.estado !== 'Desecho'"
            @click="abrirModal('edit')"
            class="px-4 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium text-sm flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
            Editar
          </button>
        </div>
      </div>

      <!-- Banner solicitud pendiente -->
      <div v-if="solicitudPendiente" class="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 space-y-2">
        <div class="flex items-center gap-2">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-amber-600 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-sm font-semibold text-amber-800">Cambio en espera de aprobación</p>
          <span class="ml-auto text-xs text-amber-500">{{ formatFecha(solicitudPendiente.fechaSolicitud) }}</span>
        </div>
        <ul v-if="camposCambiados.length" class="space-y-1 pl-6">
          <li v-for="campo in camposCambiados" :key="campo.label" class="text-xs text-amber-700">
            <span class="font-medium">{{ campo.label }}:</span>
            <span class="line-through text-amber-400 mx-1">{{ campo.actual }}</span>
            <span class="italic font-medium">→ {{ campo.propuesto }}</span>
          </li>
        </ul>
      </div>

      <!-- Info en dos columnas -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">

        <!-- Identificación -->
        <div class="bg-white rounded-2xl shadow-md p-6 border border-blue-200 space-y-4">
          <h3 class="text-xs font-semibold text-[#003d7a] uppercase tracking-wider border-b border-gray-200 pb-2">Identificación</h3>
          <div class="grid grid-cols-2 gap-3">
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-xs text-gray-400 mb-0.5">Placa</p>
              <p class="font-semibold text-[#003d7a] font-mono">{{ activo.placa }}</p>
            </div>
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-xs text-gray-400 mb-0.5">Tipo de Placa</p>
              <p class="font-medium text-gray-800">{{ activo.tipoPlaca }}</p>
            </div>
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-xs text-gray-400 mb-0.5">Artículo</p>
              <p class="font-medium text-gray-800">{{ activo.articulo }}</p>
            </div>
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-xs text-gray-400 mb-0.5">Número Serial</p>
              <p class="font-medium text-gray-800 font-mono text-sm">{{ activo.numSerial }}</p>
            </div>
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-xs text-gray-400 mb-0.5">Marca</p>
              <p class="font-medium text-gray-800">{{ activo.marca }}</p>
            </div>
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-xs text-gray-400 mb-0.5">Modelo</p>
              <p class="font-medium text-gray-800">{{ activo.modelo }}</p>
            </div>
          </div>
          <div v-if="activo.observaciones" class="bg-gray-50 rounded-lg p-3">
            <p class="text-xs text-gray-400 mb-0.5">Observaciones</p>
            <p class="text-sm text-gray-700">{{ activo.observaciones }}</p>
          </div>
        </div>

        <!-- Clasificación + Ubicación + Encargados -->
        <div class="space-y-4">
          <div class="bg-white rounded-2xl shadow-md p-6 border border-blue-200">
            <h3 class="text-xs font-semibold text-[#003d7a] uppercase tracking-wider border-b border-gray-200 pb-2 mb-3">Clasificación</h3>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Categoría</p>
                <p class="font-medium text-gray-800">{{ activo.categoriaNombre }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-1">Estado</p>
                <span class="inline-block px-2 py-0.5 rounded-full text-xs font-semibold"
                  :class="{
                    'bg-green-100 text-green-700': activo.estado === 'Activo',
                    'bg-yellow-100 text-yellow-700': activo.estado === 'Mantenimiento',
                    'bg-red-100 text-red-700': activo.estado === 'Desecho'
                  }">{{ activo.estado }}</span>
              </div>
              <div v-if="activo.fechaDesecho" class="bg-red-50 rounded-lg p-3 col-span-2">
                <p class="text-xs text-red-400 mb-0.5">Fecha de Desecho</p>
                <p class="font-medium text-red-700">{{ activo.fechaDesecho }}</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-2xl shadow-md p-6 border border-blue-200">
            <h3 class="text-xs font-semibold text-[#003d7a] uppercase tracking-wider border-b border-gray-200 pb-2 mb-3">Ubicación</h3>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-blue-50 rounded-lg p-3 border border-blue-200">
                <p class="text-xs text-blue-400 mb-0.5">Ubicación Actual</p>
                <p class="font-semibold text-blue-900">{{ activo.ubicacionActual }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Ubicación Anterior</p>
                <p class="font-medium text-gray-600">{{ activo.ubicacionAnterior || '—' }}</p>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-2xl shadow-md p-6 border border-blue-200">
            <h3 class="text-xs font-semibold text-[#003d7a] uppercase tracking-wider border-b border-gray-200 pb-2 mb-3">Encargados</h3>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-blue-50 rounded-lg p-3 border border-blue-200">
                <p class="text-xs text-blue-400 mb-0.5">Encargado Actual</p>
                <p class="font-semibold text-blue-900">{{ activo.encargadoActual }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Encargado Anterior</p>
                <p class="font-medium text-gray-600">{{ activo.encargadoAnterior || '—' }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Historial de ubicaciones y encargados -->
      <div v-if="historialMovimientos.length && !auth.esInvitado" class="bg-white rounded-2xl shadow-md p-6 border border-blue-200">
        <h3 class="text-lg font-semibold text-[#003d7a] mb-4">Historial de Ubicaciones y Encargados</h3>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-blue-100 border-b border-blue-200">
              <tr>
                <th class="text-left px-3 py-2 text-[10.5px] font-semibold text-[#003d7a] uppercase tracking-widest">Fecha</th>
                <th class="text-left px-3 py-2 text-[10.5px] font-semibold text-[#003d7a] uppercase tracking-widest">Tipo</th>
                <th class="text-left px-3 py-2 text-[10.5px] font-semibold text-[#003d7a] uppercase tracking-widest">Anterior</th>
                <th class="text-left px-3 py-2 text-[10.5px] font-semibold text-[#003d7a] uppercase tracking-widest">Nuevo</th>
                <th class="text-left px-3 py-2 text-[10.5px] font-semibold text-[#003d7a] uppercase tracking-widest">Realizado por</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200">
              <tr v-for="(mov, idx) in historialMovimientos" :key="mov.id"
                class="hover:bg-blue-50 transition"
                :class="idx % 2 === 0 ? 'bg-white' : 'bg-gray-50'">
                <td class="px-3 py-2.5 text-gray-500 whitespace-nowrap">{{ formatFecha(mov.fechaHora) }}</td>
                <td class="px-3 py-2.5">
                  <span class="px-2 py-0.5 rounded-full text-xs font-medium"
                    :class="mov.tipoAccion === 'CambioUbicacion' ? 'bg-blue-100 text-blue-800' : 'bg-green-100 text-green-800'">
                    {{ mov.tipoAccion === 'CambioUbicacion' ? 'Ubicación' : 'Encargado' }}
                  </span>
                </td>
                <td class="px-3 py-2.5 text-gray-500">{{ parsearCambio(mov.descripcion).de || '—' }}</td>
                <td class="px-3 py-2.5 font-medium text-gray-800">{{ parsearCambio(mov.descripcion).a }}</td>
                <td class="px-3 py-2.5 text-gray-500">{{ mov.usuarioNombre }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Historial del activo -->
      <div class="bg-white rounded-2xl shadow-md p-6 border border-blue-200">
        <h3 class="text-lg font-semibold text-[#003d7a] mb-5">Historial</h3>

        <div v-if="cargandoHistorial" class="text-center py-8 text-gray-400 text-sm">Cargando historial...</div>
        <div v-else-if="historial.length === 0" class="text-center py-8 text-gray-400 text-sm">
          No hay registros de historial para este activo.
        </div>

        <div v-else class="space-y-0">
          <div v-for="(item, idx) in historial" :key="item.id" class="flex gap-3">
            <!-- Dot + línea vertical -->
            <div class="flex flex-col items-center">
              <div class="w-10 h-10 rounded-full flex items-center justify-center text-base flex-shrink-0 z-10"
                :class="BADGES[item.tipoAccion] || 'bg-gray-100 text-gray-700'">
                {{ ICONOS[item.tipoAccion] || '📋' }}
              </div>
              <div v-if="idx < historial.length - 1" class="w-0.5 bg-blue-100 flex-1 my-1.5 min-h-[16px]"></div>
            </div>
            <!-- Contenido -->
            <div class="flex-1 min-w-0 pb-4 pt-1">
              <div class="flex items-start justify-between gap-2">
                <div class="min-w-0">
                  <p class="font-medium text-gray-800 text-sm leading-snug">{{ LABELS[item.tipoAccion] || item.tipoAccion }}</p>
                  <p v-if="item.descripcion" class="text-xs text-gray-500 mt-0.5 leading-relaxed">{{ item.descripcion }}</p>
                </div>
                <div class="text-right flex-shrink-0 ml-2">
                  <p class="text-xs text-gray-400 whitespace-nowrap">{{ formatFecha(item.fechaHora) }}</p>
                  <p class="text-xs text-gray-500 font-medium mt-0.5">{{ item.usuarioNombre }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Modal editar / solicitar cambio -->
    <ActivoModal
      v-if="modalAbierto"
      :mode="modalMode"
      :activo="activo"
      :categorias="categorias"
      :encargados="encargados"
      @close="modalAbierto = false"
      @saved="onSaved"
      @cancelEdit="modalAbierto = false"
      @enviarDesecho="confirmarDesecho"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'
import activoService from '@/services/activoService'
import historialService from '@/services/historialService'
import categoriaService from '@/services/categoriaService'
import encargadoService from '@/services/encargadoService'
import solicitudService from '@/services/solicitudService'
import ActivoModal from '@/components/ActivoModal.vue'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const dialog = useDialog()

const placa = route.params.placa
const activo = ref(null)
const historial = ref([])
const cargando = ref(true)
const cargandoHistorial = ref(true)
const categorias = ref([])
const encargados = ref([])
const solicitudPendiente = ref(null)
const modalAbierto = ref(false)
const modalMode = ref('edit')

onMounted(async () => {
  try {
    const [activoRes, histRes, catRes, encRes] = await Promise.all([
      activoService.obtener(placa),
      historialService.listar({ activoPlaca: placa, tamano: 200 }),
      categoriaService.listar(),
      encargadoService.listar()
    ])
    activo.value = activoRes.data
    historial.value = histRes.data.items
    categorias.value = catRes.data
    encargados.value = encRes.data
    await cargarSolicitudPendiente()
    iniciarTick()
  } catch {
    activo.value = null
  } finally {
    cargando.value = false
    cargandoHistorial.value = false
  }
})

async function cargarSolicitudPendiente() {
  if (!auth.esJefaAdministrativa) return
  try {
    const res = await solicitudService.obtenerPendienteDeActivo(placa)
    solicitudPendiente.value = res.status === 204 ? null : res.data
  } catch {
    solicitudPendiente.value = null
  }
}

function abrirModal(mode) {
  modalMode.value = mode
  modalAbierto.value = true
}

async function confirmarDesecho() {
  const ok = await dialog.confirm({
    title: 'Enviar a Desecho',
    message: `¿Está seguro de que desea enviar el activo ${placa} a desecho?\n\nEl activo quedará en estado "Desecho" y transcurridos 365 días naturales se habilitará su eliminación permanente del sistema.`,
    confirmText: 'Enviar a Desecho',
    type: 'danger'
  })
  if (!ok) return
  try {
    await activoService.editar(placa, {
      marca: activo.value.marca,
      modelo: activo.value.modelo,
      numSerial: activo.value.numSerial,
      articulo: activo.value.articulo,
      categoriaId: activo.value.categoriaId,
      observaciones: activo.value.observaciones || null,
      ubicacionActual: activo.value.ubicacionActual,
      encargadoId: activo.value.encargadoActualId,
      estado: 'Desecho'
    })
    await recargar()
  } catch (e) {
    await dialog.alert({
      title: 'Error',
      message: e.response?.data?.mensaje || 'No se pudo enviar el activo a desecho.',
      type: 'danger'
    })
  }
}

async function onSaved() {
  const wasSolicitud = modalMode.value === 'solicitud'
  modalAbierto.value = false
  if (wasSolicitud) {
    await dialog.alert({
      title: 'Solicitud enviada',
      message: 'Tu solicitud de cambio fue enviada y está pendiente de revisión por Administradora o GTI.',
      type: 'info'
    })
    await cargarSolicitudPendiente()
  } else {
    await recargar()
  }
}

async function recargar() {
  try {
    const [activoRes, histRes] = await Promise.all([
      activoService.obtener(placa),
      historialService.listar({ activoPlaca: placa, tamano: 200 })
    ])
    activo.value = activoRes.data
    historial.value = histRes.data.items
  } catch (_) { /* activo no encontrado */ }
}

// ── Eliminación dentro de los primeros 10 minutos ──
const now = ref(Date.now())
let tickInterval = null

function iniciarTick() {
  clearInterval(tickInterval)
  if (!activo.value?.fechaCreacion) return
  tickInterval = setInterval(() => {
    now.value = Date.now()
    if (!puedeEliminarReciente.value) clearInterval(tickInterval)
  }, 1000)
}

onUnmounted(() => clearInterval(tickInterval))

const tiempoRestanteElim = computed(() => {
  if (!activo.value?.fechaCreacion) return null
  const ms = 10 * 60 * 1000 - (now.value - new Date(activo.value.fechaCreacion).getTime())
  if (ms <= 0) return null
  const secs = Math.ceil(ms / 1000)
  return `${Math.floor(secs / 60)}:${String(secs % 60).padStart(2, '0')}`
})

const puedeEliminarReciente = computed(() => tiempoRestanteElim.value !== null)

const eliminandoReciente = ref(false)
async function eliminarReciente() {
  eliminandoReciente.value = true
  try {
    await activoService.eliminarReciente(placa)
    router.push('/inventario')
  } catch (e) {
    await dialog.alert({
      title: 'Error',
      message: e.response?.data?.mensaje || 'No se pudo eliminar el activo.',
      type: 'danger'
    })
  } finally {
    eliminandoReciente.value = false
  }
}

const historialMovimientos = computed(() =>
  historial.value.filter(h => h.tipoAccion === 'CambioUbicacion' || h.tipoAccion === 'CambioEncargado')
)

function parsearCambio(descripcion) {
  if (!descripcion) return { de: null, a: '—' }
  const flecha = descripcion.indexOf(' -> ')
  if (flecha === -1) return { de: null, a: descripcion }
  const colon = descripcion.indexOf(': ')
  const de = colon !== -1 ? descripcion.slice(colon + 2, flecha) : descripcion.slice(0, flecha)
  return { de, a: descripcion.slice(flecha + 4) }
}

const camposCambiados = computed(() => {
  if (!solicitudPendiente.value || !activo.value) return []
  const d = solicitudPendiente.value.datosNuevos
  const a = activo.value
  const cambios = []
  if (d.articulo !== a.articulo) cambios.push({ label: 'Artículo', actual: a.articulo, propuesto: d.articulo })
  if (d.marca !== a.marca) cambios.push({ label: 'Marca', actual: a.marca, propuesto: d.marca })
  if (d.modelo !== a.modelo) cambios.push({ label: 'Modelo', actual: a.modelo, propuesto: d.modelo })
  if (d.numSerial !== a.numSerial) cambios.push({ label: 'N° Serial', actual: a.numSerial, propuesto: d.numSerial })
  if (d.categoriaNombre !== a.categoriaNombre) cambios.push({ label: 'Categoría', actual: a.categoriaNombre, propuesto: d.categoriaNombre })
  if (d.ubicacionActual !== a.ubicacionActual) cambios.push({ label: 'Ubicación', actual: a.ubicacionActual, propuesto: d.ubicacionActual })
  if (d.encargadoNombre !== a.encargadoActual) cambios.push({ label: 'Encargado', actual: a.encargadoActual, propuesto: d.encargadoNombre })
  if (d.estado !== a.estado) cambios.push({ label: 'Estado', actual: a.estado, propuesto: d.estado })
  return cambios
})

function formatFecha(iso) {
  return new Date(iso).toLocaleString('es-CR', { dateStyle: 'short', timeStyle: 'short' })
}

const ICONOS = {
  Creacion: '✨', CambioUbicacion: '📍', CambioEncargado: '👤', CambioEstado: '🔄',
  CambioPlaca: '🏷️', Eliminacion: '🗑️', Aprobacion: '✅', Rechazo: '❌',
  SolicitudCambio: '📝', SolicitudAprobada: '✔️', SolicitudRechazada: '🚫'
}
const LABELS = {
  Creacion: 'Nuevo activo agregado', CambioUbicacion: 'Cambio de ubicación',
  CambioEncargado: 'Cambio de encargado', CambioEstado: 'Cambio de estado',
  CambioPlaca: 'Cambio de placa', Eliminacion: 'Activo eliminado',
  Aprobacion: 'Eliminación aprobada', Rechazo: 'Eliminación rechazada',
  SolicitudCambio: 'Solicitud de cambio enviada', SolicitudAprobada: 'Solicitud de cambio aprobada',
  SolicitudRechazada: 'Solicitud de cambio rechazada'
}
const BADGES = {
  Creacion: 'bg-indigo-100 text-indigo-800', CambioUbicacion: 'bg-blue-100 text-blue-800',
  CambioEncargado: 'bg-green-100 text-green-800', CambioEstado: 'bg-yellow-100 text-yellow-800',
  CambioPlaca: 'bg-purple-100 text-purple-800', Eliminacion: 'bg-red-100 text-red-800',
  Aprobacion: 'bg-teal-100 text-teal-800', Rechazo: 'bg-orange-100 text-orange-800',
  SolicitudCambio: 'bg-amber-100 text-amber-800', SolicitudAprobada: 'bg-green-100 text-green-800',
  SolicitudRechazada: 'bg-red-100 text-red-800'
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between gap-4 flex-wrap">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Desecho</h1>
        <p class="text-gray-600 mt-1">Administra activos en proceso de eliminación</p>
      </div>
      <button
        @click="imprimirPDF"
        :disabled="imprimiendo || items.length === 0"
        class="border border-red-500 text-red-600 px-4 py-2.5 rounded-lg hover:bg-red-50 transition flex items-center gap-2 font-medium text-sm disabled:opacity-50"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
        </svg>
        {{ imprimiendo ? 'Generando...' : 'Imprimir PDF' }}
      </button>
    </div>

    <!-- Alerta -->
    <div class="bg-gradient-to-r from-yellow-50 to-orange-50/50 border border-yellow-200/50 rounded-xl p-4 flex items-start gap-3">
      <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-yellow-600 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
      </svg>
      <div class="text-sm text-yellow-800">
        <p class="font-medium mb-1">Política de Eliminación</p>
        <p>Los activos deben permanecer en estado de desecho durante <strong>1 año (365 días)</strong> antes de poder ser eliminados permanentemente. La administradora debe aprobar todas las eliminaciones.</p>
      </div>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white/90 rounded-2xl shadow-lg p-6 bg-gradient-to-br from-red-50/50 to-white border border-red-100/50 hover:shadow-xl transition-all">
        <p class="text-sm text-gray-600">Total en Desecho</p>
        <p class="text-3xl font-bold text-red-600 mt-2">{{ items.length }}</p>
      </div>
      <div class="bg-white/90 rounded-2xl shadow-lg p-6 bg-gradient-to-br from-green-50/50 to-white border border-green-100/50 hover:shadow-xl transition-all">
        <p class="text-sm text-gray-600">Listos para Eliminar</p>
        <p class="text-3xl font-bold text-green-600 mt-2">{{ items.filter(i => i.puedeEliminar).length }}</p>
      </div>
      <div class="bg-white/90 rounded-2xl shadow-lg p-6 bg-gradient-to-br from-yellow-50/50 to-white border border-yellow-100/50 hover:shadow-xl transition-all">
        <p class="text-sm text-gray-600">En Espera</p>
        <p class="text-3xl font-bold text-yellow-600 mt-2">{{ items.filter(i => !i.puedeEliminar).length }}</p>
      </div>
    </div>

    <!-- Buscador -->
    <div class="flex items-center gap-3">
      <div class="relative max-w-xs w-full">
        <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          v-model="busqueda"
          type="text"
          placeholder="Buscar por artículo, placa..."
          class="w-full pl-9 pr-3 py-2 text-sm bg-white/80 border border-red-200/70 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-red-400/15 focus:border-red-400/40 transition-all"
        />
      </div>
      <span v-if="busqueda" class="text-sm text-gray-400">{{ itemsFiltrados.length }} resultado{{ itemsFiltrados.length !== 1 ? 's' : '' }}</span>
    </div>

    <!-- Lista de activos en desecho -->
    <div v-if="loading" class="text-center py-10 text-gray-400">Cargando...</div>
    <div v-else-if="items.length === 0" class="text-center py-10 text-gray-400 bg-white rounded-2xl shadow">No hay activos en estado de desecho.</div>
    <div v-else class="space-y-4">
      <div
        v-for="item in itemsFiltrados"
        :key="item.activo.placa"
        class="bg-white rounded-2xl shadow-md hover:shadow-lg transition-all duration-200 border border-red-200 border-l-4 border-l-red-400 p-6"
      >
        <div class="flex flex-col lg:flex-row gap-6">
          <div class="flex-1">
            <div class="flex items-start justify-between mb-4">
              <div>
                <h3 class="text-xl font-semibold text-gray-900">{{ item.activo.articulo }}</h3>
                <p class="text-sm text-gray-500">{{ item.activo.marca }} {{ item.activo.modelo }} • Placa: {{ item.activo.placa }}</p>
              </div>
              <span
                class="px-3 py-1 rounded-full text-xs font-medium flex items-center gap-1"
                :class="item.puedeEliminar ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'"
              >
                {{ item.puedeEliminar ? '✅ Listo para eliminar' : `⏳ ${365 - item.diasEnDesecho} días restantes` }}
              </span>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
              <div>
                <p class="text-gray-600"><strong>Categoría:</strong> {{ item.activo.categoriaNombre }}</p>
                <p class="text-gray-600"><strong>Ubicación:</strong> {{ item.activo.ubicacionActual }}</p>
              </div>
              <div>
                <p class="text-gray-600"><strong>Encargado:</strong> {{ item.activo.encargadoActual }}</p>
                <p v-if="item.activo.observaciones" class="text-gray-600"><strong>Observaciones:</strong> {{ item.activo.observaciones }}</p>
              </div>
            </div>

            <!-- Barra de progreso -->
            <div class="mt-4">
              <div class="flex items-center justify-between mb-2">
                <p class="text-sm font-medium text-gray-700">Tiempo en desecho</p>
                <p class="text-sm text-gray-600">{{ item.diasEnDesecho }} / 365 días</p>
              </div>
              <div class="h-3 bg-gray-200 rounded-full overflow-hidden">
                <div
                  class="h-full transition-all duration-300"
                  :class="item.puedeEliminar ? 'bg-green-500' : 'bg-yellow-500'"
                  :style="{ width: Math.min((item.diasEnDesecho / 365) * 100, 100) + '%' }"
                />
              </div>
            </div>

            <!-- Fecha -->
            <div v-if="item.activo.fechaDesecho" class="mt-4 flex items-center gap-2 text-sm text-gray-600">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>
              <span>Fecha de desecho: {{ formatFecha(item.activo.fechaDesecho) }}</span>
            </div>

            <!-- Responsable del desecho -->
            <div v-if="item.desechoInfo" class="mt-2 flex items-center gap-2 text-sm text-red-700">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-red-400 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              <span v-if="item.desechoInfo.tipo === 'solicitud'">
                Solicitado por <strong>{{ item.desechoInfo.solicitante }}</strong>
                <template v-if="item.desechoInfo.aprobadoPor">, aprobado por <strong>{{ item.desechoInfo.aprobadoPor }}</strong></template>
              </span>
              <span v-else>
                Enviado a desecho por <strong>{{ item.desechoInfo.realizadoPor }}</strong>
              </span>
            </div>
          </div>

          <!-- Acciones (solo Admin) -->
          <div v-if="auth.esAdministradora" class="flex lg:flex-col gap-2 lg:justify-center">
            <button
              v-if="item.puedeEliminar"
              @click="abrirAprobacion(item)"
              class="px-6 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition font-medium flex items-center gap-2"
            >
              ✅ Aprobar Eliminación
            </button>
            <button
              v-else
              disabled
              class="px-6 py-2 bg-gray-300 text-gray-500 rounded-lg cursor-not-allowed font-medium flex items-center gap-2"
            >
              ⏳ No Disponible
            </button>
            <button
              @click="rechazar(item.activo.placa)"
              class="px-6 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium"
            >
              Recuperar
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal confirmación -->
    <Teleport to="body">
    <div v-if="itemSeleccionado" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white/95 backdrop-blur-md rounded-2xl shadow-2xl max-w-lg w-full border border-red-100/50">
        <div class="bg-red-600 text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
          <h2 class="text-xl font-bold">Confirmar Eliminación</h2>
          <button @click="itemSeleccionado = null" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <div class="p-6">
          <div class="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6 flex items-start gap-3">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-yellow-600 flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
            <div class="text-sm text-yellow-800">
              <p class="font-medium mb-1">Advertencia</p>
              <p>Esta acción es permanente e irreversible. El activo será eliminado completamente del sistema.</p>
            </div>
          </div>
          <div class="space-y-3 mb-6 text-sm">
            <div><p class="text-gray-500">Placa</p><p class="font-semibold">{{ itemSeleccionado.activo.placa }}</p></div>
            <div><p class="text-gray-500">Artículo</p><p class="font-semibold">{{ itemSeleccionado.activo.articulo }} — {{ itemSeleccionado.activo.marca }} {{ itemSeleccionado.activo.modelo }}</p></div>
            <div><p class="text-gray-500">Tiempo en desecho</p><p class="font-semibold text-green-600">{{ itemSeleccionado.diasEnDesecho }} días (cumple el requisito)</p></div>
          </div>
          <div class="flex gap-3">
            <button @click="itemSeleccionado = null" class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">Cancelar</button>
            <button @click="segundaConfirmacion = true" class="flex-1 px-4 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition font-medium">
              Eliminar Permanentemente
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Segunda confirmación (shake) -->
    <div v-if="segundaConfirmacion" class="fixed inset-0 bg-black/60 flex items-center justify-center p-4" style="z-index: 10000">
        <div class="bg-white rounded-2xl shadow-2xl max-w-sm w-full border-2 border-red-500 shake-dialog">
          <div class="p-6 text-center space-y-4">
            <div class="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-8 h-8 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
            </div>
            <h3 class="text-xl font-bold text-gray-900">¿Está absolutamente seguro?</h3>
            <p class="text-sm text-gray-600">
              Esta acción es <strong class="text-red-600">permanente e irreversible</strong>.<br/>
              El activo <strong class="font-mono">{{ itemSeleccionado?.activo.placa }}</strong> será eliminado
              definitivamente del sistema y no podrá recuperarse.
            </p>
            <div class="flex gap-3 pt-1">
              <button @click="segundaConfirmacion = false"
                class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium text-sm">
                No, cancelar
              </button>
              <button @click="confirmarEliminar" :disabled="actionLoading"
                class="flex-1 px-4 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition font-medium text-sm disabled:bg-red-300 flex items-center justify-center gap-2">
                <svg v-if="actionLoading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
                </svg>
                {{ actionLoading ? 'Eliminando...' : 'Sí, eliminar definitivamente' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'
import desechoService from '@/services/desechoService'

const auth = useAuthStore()
const dialog = useDialog()
const items = ref([])
const busqueda = ref('')
const itemsFiltrados = computed(() => {
  const q = busqueda.value.trim().toLowerCase()
  if (!q) return items.value
  return items.value.filter(i => {
    const a = i.activo
    return (
      a.articulo?.toLowerCase().includes(q) ||
      a.placa?.toLowerCase().includes(q) ||
      a.marca?.toLowerCase().includes(q) ||
      a.modelo?.toLowerCase().includes(q) ||
      a.encargadoActual?.toLowerCase().includes(q)
    )
  })
})
const loading = ref(false)
const itemSeleccionado = ref(null)
const segundaConfirmacion = ref(false)
const actionLoading = ref(false)
const imprimiendo = ref(false)

onMounted(cargar)

async function cargar() {
  loading.value = true
  try { const { data } = await desechoService.listar(); items.value = data }
  finally { loading.value = false }
}

function abrirAprobacion(item) { itemSeleccionado.value = item }

async function confirmarEliminar() {
  actionLoading.value = true
  try {
    await desechoService.aprobar(itemSeleccionado.value.activo.placa)
    segundaConfirmacion.value = false
    itemSeleccionado.value = null
    await cargar()
  } catch (e) {
    segundaConfirmacion.value = false
    await dialog.alert({
      title: 'Error al eliminar',
      message: e.response?.data?.mensaje || 'No se pudo completar la eliminación.',
      type: 'danger'
    })
  } finally {
    actionLoading.value = false
  }
}

async function rechazar(placa) {
  const ok = await dialog.confirm({
    title: 'Recuperar Activo',
    message: '¿Recuperar este activo y restaurarlo a estado Activo?',
    confirmText: 'Recuperar',
    cancelText: 'Cancelar',
    type: 'warning'
  })
  if (!ok) return
  try {
    await desechoService.rechazar(placa)
    await cargar()
  } catch (e) {
    await dialog.alert({
      title: 'Error',
      message: e.response?.data?.mensaje || 'No se pudo rechazar la eliminación.',
      type: 'danger'
    })
  }
}

function formatFecha(f) {
  if (!f) return ''
  return new Date(f).toLocaleDateString('es-CR')
}

async function imprimirPDF() {
  imprimiendo.value = true
  try {
    const { jsPDF } = await import('jspdf')
    const { default: autoTable } = await import('jspdf-autotable')

    const doc = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' })
    const hoy = new Date().toLocaleDateString('es-CR')
    const lista = itemsFiltrados.value

    doc.setFontSize(14)
    doc.setTextColor(0, 61, 122)
    doc.text('Activos en Desecho · SIBI EIC UCR', 14, 16)

    doc.setFontSize(9)
    doc.setTextColor(100)
    const subtitulo = `Generado: ${hoy}  ·  Total: ${lista.length} activo${lista.length !== 1 ? 's' : ''}${busqueda.value ? `  ·  Filtro: "${busqueda.value}"` : ''}`
    doc.text(subtitulo, 14, 22)

    autoTable(doc, {
      head: [['Placa', 'Artículo', 'Marca / Modelo', 'Categoría', 'Encargado', 'Ubicación', 'Fecha Desecho', 'Días', 'Estado']],
      body: lista.map(({ activo: a, diasEnDesecho, puedeEliminar }) => [
        a.placa,
        a.articulo,
        `${a.marca} ${a.modelo}`,
        a.categoriaNombre,
        a.encargadoActual,
        a.ubicacionActual,
        formatFecha(a.fechaDesecho),
        String(diasEnDesecho),
        puedeEliminar ? 'Listo para eliminar' : `${365 - diasEnDesecho} días restantes`
      ]),
      startY: 27,
      styles: { fontSize: 7.5, cellPadding: 2 },
      headStyles: { fillColor: [185, 28, 28], textColor: 255, fontStyle: 'bold' },
      alternateRowStyles: { fillColor: [255, 245, 245] },
      columnStyles: {
        0: { cellWidth: 20 },
        1: { cellWidth: 30 },
        2: { cellWidth: 30 },
        3: { cellWidth: 25 },
        4: { cellWidth: 30 },
        5: { cellWidth: 28 },
        6: { cellWidth: 22 },
        7: { cellWidth: 12 },
        8: { cellWidth: 33 }
      },
      didDrawCell(data) {
        if (data.section === 'body' && data.column.index === 8) {
          const val = data.cell.text[0]
          if (val === 'Listo para eliminar') {
            doc.setTextColor(21, 128, 61)
          } else {
            doc.setTextColor(161, 98, 7)
          }
        }
      },
      didParseCell(data) {
        if (data.section === 'body' && data.column.index === 8) {
          const val = data.cell.text[0]
          data.cell.styles.textColor = val === 'Listo para eliminar' ? [21, 128, 61] : [161, 98, 7]
          data.cell.styles.fontStyle = 'bold'
        }
      }
    })

    const fecha = new Date().toISOString().slice(0, 10)
    doc.save(`desecho_${fecha}.pdf`)
  } finally {
    imprimiendo.value = false
  }
}
</script>

<style scoped>
@keyframes shake {
  0%   { transform: translateX(0) rotate(0deg); }
  15%  { transform: translateX(-8px) rotate(-1deg); }
  30%  { transform: translateX(8px) rotate(1deg); }
  45%  { transform: translateX(-6px) rotate(-0.5deg); }
  60%  { transform: translateX(6px) rotate(0.5deg); }
  75%  { transform: translateX(-3px) rotate(0deg); }
  90%  { transform: translateX(3px) rotate(0deg); }
  100% { transform: translateX(0) rotate(0deg); }
}

.shake-dialog {
  animation: shake 0.55s ease-in-out;
}
</style>

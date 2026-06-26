<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between gap-4 flex-wrap">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Inventario</h1>
        <p class="text-gray-600 mt-1">Administra los activos institucionales</p>
      </div>
      <div class="flex items-center gap-2">
        <!-- Exportar (solo roles internos) -->
        <template v-if="auth.esGTI || auth.esJefaAdministrativa">
          <button
            @click="exportarExcel"
            :disabled="exportando"
            class="border border-green-600 text-green-700 px-4 py-2.5 rounded-lg hover:bg-green-50 transition flex items-center gap-2 font-medium text-sm disabled:opacity-50"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
            </svg>
            Excel
          </button>
          <button
            @click="exportarPDF"
            :disabled="exportando"
            class="border border-red-500 text-red-600 px-4 py-2.5 rounded-lg hover:bg-red-50 transition flex items-center gap-2 font-medium text-sm disabled:opacity-50"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
            </svg>
            PDF
          </button>
        </template>
        <!-- Importar / Nuevo Activo -->
        <template v-if="auth.esGTI">
          <button
            @click="importarOpen = true"
            class="border border-[#003d7a] text-[#003d7a] px-4 py-2.5 rounded-lg hover:bg-blue-50 transition flex items-center gap-2 font-medium text-sm"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
            </svg>
            Importar Excel
          </button>
          <button
            @click="abrirModal('create', null)"
            class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
            </svg>
            Nuevo Activo
          </button>
        </template>
      </div>
    </div>

    <!-- Filtros -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 border border-blue-100/50">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="relative">
          <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            v-model="busqueda"
            type="text"
            placeholder="Buscar por placa, marca, modelo..."
            class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none"
          />
        </div>
        <select
          v-model="categoriaId"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none"
        >
          <option value="">Todas las categorías</option>
          <option v-for="cat in categorias" :key="cat.id" :value="cat.id">{{ cat.nombre }}</option>
        </select>
        <select
          v-model="estado"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none"
        >
          <option value="">Todos los estados</option>
          <option value="Activo">Activo</option>
          <option value="Mantenimiento">Mantenimiento</option>
          <option value="Desecho">Desecho</option>
        </select>
      </div>
    </div>

    <!-- Tabla -->
    <div class="bg-white rounded-2xl shadow-lg overflow-hidden border border-blue-200">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-blue-100 border-b border-blue-200">
            <tr>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Placa</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Artículo</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Marca / Modelo</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Categoría</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Ubicación</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Encargado</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Estado</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <tr v-if="loading">
              <td colspan="7" class="px-6 py-10 text-center text-gray-400">Cargando...</td>
            </tr>
            <tr v-else-if="activos.length === 0">
              <td colspan="7" class="px-6 py-10 text-center text-gray-400">No se encontraron activos.</td>
            </tr>
            <tr
              v-for="(activo, idx) in activos"
              :key="activo.placa"
              @click="abrirModal('view', activo)"
              class="transition-colors duration-150 cursor-pointer"
              :class="idx % 2 === 0 ? 'bg-white hover:bg-blue-50' : 'bg-gray-50 hover:bg-blue-50'"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex flex-col">
                  <span class="font-medium text-gray-900">{{ activo.placa }}</span>
                  <span class="text-xs text-gray-500">{{ activo.tipoPlaca }}</span>
                </div>
              </td>
              <td class="px-6 py-4 text-gray-900">{{ activo.articulo }}</td>
              <td class="px-6 py-4">
                <div class="flex flex-col">
                  <span class="text-gray-900">{{ activo.marca }}</span>
                  <span class="text-sm text-gray-500">{{ activo.modelo }}</span>
                </div>
              </td>
              <td class="px-6 py-4 text-gray-900">{{ activo.categoriaNombre }}</td>
              <td class="px-6 py-4 text-sm text-gray-900">{{ activo.ubicacionActual }}</td>
              <td class="px-6 py-4 text-sm text-gray-900">{{ activo.encargadoActual }}</td>
              <td class="px-6 py-4">
                <span
                  class="px-3 py-1 rounded-full text-xs font-medium"
                  :class="estadoClase(activo.estado)"
                >{{ activo.estado }}</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Paginación -->
      <div class="bg-gray-50 px-6 py-4 flex items-center justify-between border-t border-gray-200">
        <p class="text-sm text-gray-700">
          Mostrando <span class="font-medium">{{ (pagina - 1) * tamano + 1 }}</span> –
          <span class="font-medium">{{ Math.min(pagina * tamano, total) }}</span> de
          <span class="font-medium">{{ total }}</span>
        </p>
        <div class="flex items-center gap-2">
          <button @click="pagina--" :disabled="pagina === 1" class="px-3 py-1 border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 transition">‹</button>
          <span class="text-sm text-gray-700">Página {{ pagina }} de {{ totalPaginas }}</span>
          <button @click="pagina++" :disabled="pagina >= totalPaginas" class="px-3 py-1 border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 transition">›</button>
        </div>
      </div>
    </div>

    <!-- Modal activo -->
    <ActivoModal
      v-if="modalOpen"
      :mode="modalMode"
      :activo="activoSeleccionado"
      :categorias="categorias"
      :encargados="encargados"
      @close="modalOpen = false"
      @saved="onSaved"
      @edit="modalMode = 'edit'"
      @solicitar="modalMode = 'solicitud'"
      @cancelEdit="modalMode = 'view'"
      @enviarDesecho="confirmarDesecho"
      @entityCreated="recargarEntidades"
    />

    <!-- Modal importar -->
    <ImportarActivosModal
      v-if="importarOpen"
      :categorias="categorias"
      :encargados="encargados"
      @close="importarOpen = false"
      @importado="cargarActivos"
    />
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'
import activoService from '@/services/activoService'
import categoriaService from '@/services/categoriaService'
import encargadoService from '@/services/encargadoService'
import ActivoModal from '@/components/ActivoModal.vue'
import ImportarActivosModal from '@/components/ImportarActivosModal.vue'

const route = useRoute()
const auth = useAuthStore()
const dialog = useDialog()

const activos = ref([])
const total = ref(0)
const pagina = ref(1)
const tamano = ref(10)
const busqueda = ref('')
const categoriaId = ref('')
const estado = ref('')
const categorias = ref([])
const encargados = ref([])
const loading = ref(false)
const importarOpen = ref(false)
const modalOpen = ref(false)
const modalMode = ref('create')
const activoSeleccionado = ref(null)

const totalPaginas = computed(() => Math.max(1, Math.ceil(total.value / tamano.value)))
const exportando = ref(false)

const COLUMNAS = ['Placa', 'Tipo Placa', 'Artículo', 'Marca', 'Modelo', 'N° Serial', 'Categoría', 'Ubicación', 'Encargado', 'Estado']

function activoToFila(a) {
  return [a.placa, a.tipoPlaca, a.articulo, a.marca, a.modelo, a.numSerial, a.categoriaNombre, a.ubicacionActual, a.encargadoActual, a.estado]
}

async function obtenerTodosParaExportar() {
  const params = { pagina: 1, tamano: 9999 }
  if (busqueda.value) params.busqueda = busqueda.value
  if (categoriaId.value) params.categoriaId = categoriaId.value
  if (estado.value) params.estado = estado.value
  const { data } = await activoService.listar(params)
  return data.items
}

function nombreArchivo(ext) {
  const hoy = new Date().toISOString().slice(0, 10)
  const partes = ['Inventario_SIBI', hoy]
  if (estado.value) partes.push(estado.value)
  return partes.join('_') + '.' + ext
}

async function exportarExcel() {
  exportando.value = true
  try {
    const XLSX = await import('xlsx')
    const items = await obtenerTodosParaExportar()
    const filas = items.map(activoToFila)
    const ws = XLSX.utils.aoa_to_sheet([COLUMNAS, ...filas])
    ws['!cols'] = [8, 10, 20, 15, 15, 18, 15, 20, 20, 12].map(w => ({ wch: w }))
    const wb = XLSX.utils.book_new()
    XLSX.utils.book_append_sheet(wb, ws, 'Inventario')
    XLSX.writeFile(wb, nombreArchivo('xlsx'))
  } finally {
    exportando.value = false
  }
}

async function exportarPDF() {
  exportando.value = true
  try {
    const { jsPDF } = await import('jspdf')
    const { default: autoTable } = await import('jspdf-autotable')
    const items = await obtenerTodosParaExportar()
    const doc = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' })
    const hoy = new Date().toLocaleDateString('es-CR')

    doc.setFontSize(14)
    doc.setTextColor(0, 61, 122)
    doc.text('Inventario de Activos · SIBI EIC UCR', 14, 16)
    doc.setFontSize(9)
    doc.setTextColor(100)
    const filtros = [
      busqueda.value ? `Búsqueda: "${busqueda.value}"` : null,
      categoriaId.value ? `Categoría: ${categorias.value.find(c => c.id == categoriaId.value)?.nombre ?? categoriaId.value}` : null,
      estado.value ? `Estado: ${estado.value}` : null
    ].filter(Boolean)
    doc.text(`Generado: ${hoy}  ·  Total: ${items.length} activos${filtros.length ? '  ·  Filtros: ' + filtros.join(', ') : ''}`, 14, 22)

    autoTable(doc, {
      head: [COLUMNAS],
      body: items.map(activoToFila),
      startY: 27,
      styles: { fontSize: 7.5, cellPadding: 2 },
      headStyles: { fillColor: [0, 61, 122], textColor: 255, fontStyle: 'bold' },
      alternateRowStyles: { fillColor: [240, 246, 255] },
      columnStyles: {
        0: { cellWidth: 18 }, 1: { cellWidth: 16 }, 2: { cellWidth: 28 },
        3: { cellWidth: 20 }, 4: { cellWidth: 20 }, 5: { cellWidth: 28 },
        6: { cellWidth: 22 }, 7: { cellWidth: 28 }, 8: { cellWidth: 28 }, 9: { cellWidth: 18 }
      }
    })
    doc.save(nombreArchivo('pdf'))
  } finally {
    exportando.value = false
  }
}

async function cargarActivos() {
  loading.value = true
  try {
    const params = { pagina: pagina.value, tamano: tamano.value }
    if (busqueda.value) params.busqueda = busqueda.value
    if (categoriaId.value) params.categoriaId = categoriaId.value
    if (estado.value) params.estado = estado.value
    const { data } = await activoService.listar(params)
    activos.value = data.items
    total.value = data.total
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  if (route.query.busqueda) busqueda.value = route.query.busqueda
  const [catRes, encRes] = await Promise.all([categoriaService.listar(), encargadoService.listar()])
  categorias.value = catRes.data
  encargados.value = encRes.data
  await cargarActivos()
})

watch([pagina], cargarActivos)
watch([busqueda, categoriaId, estado], () => { pagina.value = 1; cargarActivos() })

function abrirModal(mode, activo) {
  modalMode.value = mode
  activoSeleccionado.value = activo
  modalOpen.value = true
}

async function recargarEntidades() {
  const [catRes, encRes] = await Promise.all([categoriaService.listar(), encargadoService.listar()])
  categorias.value = catRes.data
  encargados.value = encRes.data
}

async function confirmarDesecho(activo) {
  const ok = await dialog.confirm({
    title: 'Enviar a Desecho',
    message: `¿Está seguro de que desea enviar el activo ${activo.placa} a desecho?\n\nEl activo quedará en estado "Desecho" y transcurridos 365 días naturales se habilitará su eliminación permanente del sistema.`,
    confirmText: 'Enviar a Desecho',
    type: 'danger'
  })
  if (!ok) return
  try {
    await activoService.editar(activo.placa, {
      marca: activo.marca,
      modelo: activo.modelo,
      numSerial: activo.numSerial,
      articulo: activo.articulo,
      categoriaId: activo.categoriaId,
      observaciones: activo.observaciones || null,
      ubicacionActual: activo.ubicacionActual,
      encargadoId: activo.encargadoActualId,
      estado: 'Desecho'
    })
    modalOpen.value = false
    await cargarActivos()
  } catch (e) {
    await dialog.alert({
      title: 'No se pudo actualizar',
      message: e.response?.data?.mensaje || 'No se pudo enviar el activo a desecho.',
      type: 'danger'
    })
  }
}

async function onSaved() {
  const wasSolicitud = modalMode.value === 'solicitud'
  modalOpen.value = false
  if (wasSolicitud) {
    await dialog.alert({
      title: 'Solicitud enviada',
      message: 'Tu solicitud de cambio fue enviada y está pendiente de revisión por Administradora o GTI.',
      type: 'info'
    })
  } else {
    await cargarActivos()
  }
}

const estadoClases = {
  Activo: 'bg-green-100 text-green-800',
  Mantenimiento: 'bg-yellow-100 text-yellow-800',
  Desecho: 'bg-red-100 text-red-800'
}

function estadoClase(e) {
  return estadoClases[e] || 'bg-gray-100 text-gray-800'
}
</script>

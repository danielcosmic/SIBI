<template>
  <Teleport to="body">
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] flex flex-col">

      <!-- Header -->
      <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl shrink-0">
        <div class="flex items-center gap-3">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
          </svg>
          <h2 class="text-xl font-bold">Importar Activos desde Excel</h2>
        </div>
        <button @click="$emit('close')" class="p-1 hover:bg-white/10 rounded">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Pasos indicadores -->
      <div class="flex border-b border-gray-100 shrink-0">
        <div v-for="(label, idx) in ['1. Preparar archivo', '2. Vista previa', '3. Resultados']" :key="idx"
          class="flex-1 py-3 text-sm font-medium text-center transition-colors"
          :class="paso === idx + 1
            ? 'text-[#003d7a] border-b-2 border-[#003d7a] bg-blue-50/40'
            : 'text-gray-400'"
        >
          {{ label }}
        </div>
      </div>

      <!-- Contenido scrollable -->
      <div class="flex-1 overflow-y-auto p-6">

        <!-- PASO 1: Instrucciones + carga -->
        <div v-if="paso === 1" class="space-y-5">
          <div class="bg-blue-50 border border-blue-200 rounded-xl p-4 space-y-2">
            <p class="text-sm font-semibold text-[#003d7a]">Instrucciones</p>
            <ol class="text-sm text-gray-700 space-y-1 list-decimal list-inside">
              <li>Descarga la plantilla de Excel.</li>
              <li>Completa los datos de cada activo (una fila por activo).</li>
              <li>Los campos <strong>Categoría</strong> y <strong>Encargado</strong> deben escribirse exactamente como aparecen en el sistema.</li>
              <li>El campo <strong>Tipo Placa</strong> debe ser <code class="bg-blue-100 px-1 rounded">Institucional</code> o <code class="bg-blue-100 px-1 rounded">Interno</code>.</li>
              <li>No modifiques los encabezados de la plantilla.</li>
            </ol>
          </div>

          <!-- Categorías y encargados disponibles -->
          <div class="grid grid-cols-2 gap-4">
            <div class="border border-gray-200 rounded-xl overflow-hidden">
              <div class="bg-gray-50 px-4 py-2 text-xs font-semibold text-gray-600 uppercase tracking-wide">Categorías disponibles</div>
              <ul class="divide-y divide-gray-100 max-h-36 overflow-y-auto">
                <li v-for="c in categorias" :key="c.id" class="px-4 py-1.5 text-sm text-gray-700">{{ c.nombre }}</li>
              </ul>
            </div>
            <div class="border border-gray-200 rounded-xl overflow-hidden">
              <div class="bg-gray-50 px-4 py-2 text-xs font-semibold text-gray-600 uppercase tracking-wide">Encargados disponibles</div>
              <ul class="divide-y divide-gray-100 max-h-36 overflow-y-auto">
                <li v-for="e in encargados" :key="e.id" class="px-4 py-1.5 text-sm text-gray-700">{{ e.nombre }}</li>
              </ul>
            </div>
          </div>

          <!-- Botón plantilla -->
          <button @click="descargarPlantilla"
            class="flex items-center gap-2 px-4 py-2.5 border border-green-600 text-green-700 rounded-lg hover:bg-green-50 transition text-sm font-medium">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
            </svg>
            Descargar plantilla (.xlsx)
          </button>

          <!-- Upload -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Seleccionar archivo Excel</label>
            <div
              class="border-2 border-dashed rounded-xl p-8 text-center transition-all"
              :class="archivoError
                ? 'border-red-300 bg-red-50'
                : archivoNombre
                  ? 'border-green-400 bg-green-50'
                  : 'border-gray-300 hover:border-[#003d7a] hover:bg-blue-50/30'"
              @dragover.prevent
              @drop.prevent="onDrop"
            >
              <!-- Ícono: check si hay archivo, documento si no -->
              <template v-if="archivoNombre">
                <div class="w-12 h-12 mx-auto mb-3 bg-green-100 rounded-full flex items-center justify-center">
                  <svg xmlns="http://www.w3.org/2000/svg" class="w-7 h-7 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <p class="text-sm font-semibold text-green-700 mb-0.5">Archivo cargado</p>
                <p class="text-sm text-green-600 mb-3 truncate max-w-xs mx-auto">{{ archivoNombre }}</p>
              </template>
              <template v-else>
                <svg xmlns="http://www.w3.org/2000/svg" class="w-10 h-10 mx-auto mb-3 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <p class="text-sm text-gray-600 mb-1">Arrastra tu archivo aquí o haz clic para seleccionar</p>
                <p class="text-xs text-gray-400 mb-3">.xlsx o .xls</p>
              </template>
              <input ref="fileInput" type="file" accept=".xlsx,.xls" class="hidden" @change="onFileChange" />
              <button type="button" @click="fileInput.click()"
                class="px-4 py-1.5 text-sm rounded-lg transition"
                :class="archivoNombre
                  ? 'border border-green-500 text-green-700 hover:bg-green-100'
                  : 'bg-[#003d7a] text-white hover:bg-[#002d5a]'">
                {{ archivoNombre ? 'Cambiar archivo' : 'Seleccionar archivo' }}
              </button>
            </div>
            <p v-if="archivoError" class="mt-1.5 text-sm text-red-600">{{ archivoError }}</p>
          </div>
        </div>

        <!-- PASO 2: Vista previa -->
        <div v-else-if="paso === 2" class="space-y-4">
          <div class="flex items-center justify-between">
            <p class="text-sm text-gray-600">
              <span class="font-semibold text-gray-900">{{ filas.length }}</span> filas detectadas ·
              <span class="text-red-600 font-medium">{{ filasConError.length }} con errores</span> ·
              <span class="text-green-700 font-medium">{{ filasValidas.length }} listas para importar</span>
            </p>
            <span v-if="filasConError.length" class="text-xs bg-red-50 text-red-700 border border-red-200 px-2 py-1 rounded-full">
              Corrige los errores en el Excel y vuelve a subir el archivo
            </span>
          </div>

          <div class="border border-gray-200 rounded-xl overflow-hidden">
            <div class="overflow-x-auto max-h-96">
              <table class="w-full text-sm">
                <thead class="bg-gray-50 sticky top-0">
                  <tr>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">#</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Placa</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Artículo</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Marca / Modelo</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Categoría</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Encargado</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Estado</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-gray-100">
                  <tr v-for="fila in filas" :key="fila._idx"
                    :class="fila._errores.length ? 'bg-red-50' : 'hover:bg-gray-50'">
                    <td class="px-3 py-2 text-gray-400 text-xs">{{ fila._idx }}</td>
                    <td class="px-3 py-2 font-mono font-medium text-gray-900">{{ fila.placa || '—' }}</td>
                    <td class="px-3 py-2 text-gray-700">{{ fila.articulo || '—' }}</td>
                    <td class="px-3 py-2 text-gray-700">{{ fila.marca }} {{ fila.modelo }}</td>
                    <td class="px-3 py-2">
                      <span :class="categoriaValida(fila.categoriaNombre) ? 'text-gray-700' : 'text-red-600 font-medium'">
                        {{ fila.categoriaNombre || '—' }}
                      </span>
                    </td>
                    <td class="px-3 py-2">
                      <span :class="encargadoValido(fila.encargadoNombre) ? 'text-gray-700' : 'text-red-600 font-medium'">
                        {{ fila.encargadoNombre || '—' }}
                      </span>
                    </td>
                    <td class="px-3 py-2">
                      <span v-if="fila._errores.length" class="text-red-600 text-xs">
                        {{ fila._errores.join(' · ') }}
                      </span>
                      <span v-else class="text-green-700 text-xs font-medium">OK</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- PASO 3: Resultados -->
        <div v-else-if="paso === 3" class="space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div class="bg-green-50 border border-green-200 rounded-xl p-4 text-center">
              <p class="text-3xl font-bold text-green-700">{{ resultadosExitosos }}</p>
              <p class="text-sm text-green-600 mt-1">Activos creados</p>
            </div>
            <div class="bg-red-50 border border-red-200 rounded-xl p-4 text-center">
              <p class="text-3xl font-bold text-red-600">{{ resultadosError }}</p>
              <p class="text-sm text-red-500 mt-1">Filas con error</p>
            </div>
          </div>

          <div v-if="resultadosError > 0" class="border border-red-200 rounded-xl overflow-hidden">
            <div class="bg-red-50 px-4 py-2 text-xs font-semibold text-red-700 uppercase tracking-wide">Errores por fila</div>
            <ul class="divide-y divide-red-100 max-h-64 overflow-y-auto">
              <li v-for="r in resultados.filter(r => !r.exitoso)" :key="r.fila"
                class="px-4 py-2.5 flex items-start gap-3 text-sm">
                <span class="text-gray-400 shrink-0 w-14">Fila {{ r.fila }}</span>
                <span class="font-mono font-medium text-gray-700 shrink-0 w-28">{{ r.placa }}</span>
                <span class="text-red-600">{{ r.error }}</span>
              </li>
            </ul>
          </div>

          <p v-if="resultadosExitosos > 0" class="text-sm text-gray-600 bg-blue-50 border border-blue-100 rounded-lg px-4 py-3">
            El inventario fue actualizado. Los activos creados ya aparecen en la tabla de inventario.
          </p>
        </div>

      </div>

      <!-- Footer -->
      <div class="border-t border-gray-100 px-6 py-4 flex justify-between items-center shrink-0">
        <button v-if="paso > 1 && paso < 3" @click="paso--"
          class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition text-sm font-medium">
          Atrás
        </button>
        <span v-else></span>

        <div class="flex gap-3">
          <button @click="$emit('close')" class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition text-sm font-medium">
            {{ paso === 3 ? 'Cerrar' : 'Cancelar' }}
          </button>
          <button v-if="paso === 1" @click="parsearArchivo"
            :disabled="!archivoNombre"
            class="px-5 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition text-sm font-medium disabled:bg-gray-300">
            Previsualizar
          </button>
          <button v-if="paso === 2" @click="confirmarImportacion"
            :disabled="filasValidas.length === 0 || importando"
            class="px-5 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition text-sm font-medium disabled:bg-gray-300">
            {{ importando ? 'Importando...' : `Importar ${filasValidas.length} activo${filasValidas.length !== 1 ? 's' : ''}` }}
          </button>
        </div>
      </div>

    </div>
  </div>
  </Teleport>
</template>

<script setup>
import { ref, computed } from 'vue'
import activoService from '@/services/activoService'
import { useDialog } from '@/composables/useDialog'

const props = defineProps({
  categorias: { type: Array, default: () => [] },
  encargados: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'importado'])
const dialog = useDialog()

const paso = ref(1)
const fileInput = ref(null)
const archivoNombre = ref('')
const archivoError = ref('')
const filas = ref([])
const resultados = ref([])
const importando = ref(false)

const COLUMNAS = ['Placa', 'Tipo Placa', 'Artículo', 'Marca', 'Modelo', 'N° Serial', 'Categoría', 'Ubicación Actual', 'Encargado', 'Observaciones']

const categoriasPorNombre = computed(() =>
  new Set(props.categorias.map(c => c.nombre.toLowerCase()))
)
const encargadosPorNombre = computed(() =>
  new Set(props.encargados.map(e => e.nombre.toLowerCase()))
)

function categoriaValida(nombre) {
  return nombre && categoriasPorNombre.value.has(nombre.toLowerCase())
}
function encargadoValido(nombre) {
  return nombre && encargadosPorNombre.value.has(nombre.toLowerCase())
}

const filasConError = computed(() => filas.value.filter(f => f._errores.length > 0))
const filasValidas = computed(() => filas.value.filter(f => f._errores.length === 0))

const resultadosExitosos = computed(() => resultados.value.filter(r => r.exitoso).length)
const resultadosError = computed(() => resultados.value.filter(r => !r.exitoso).length)

async function descargarPlantilla() {
  const XLSX = await import('xlsx')
  const ws = XLSX.utils.aoa_to_sheet([COLUMNAS])
  ws['!cols'] = [12, 12, 25, 15, 15, 18, 18, 20, 20, 20].map(w => ({ wch: w }))
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, 'Activos')
  XLSX.writeFile(wb, 'Plantilla_Importacion_SIBI.xlsx')
}

function onDrop(e) {
  const file = e.dataTransfer.files[0]
  if (file) procesarArchivo(file)
}

function onFileChange(e) {
  const file = e.target.files[0]
  if (file) procesarArchivo(file)
}

function procesarArchivo(file) {
  archivoError.value = ''
  if (!file.name.match(/\.(xlsx|xls)$/i)) {
    archivoError.value = 'El archivo debe ser .xlsx o .xls'
    return
  }
  archivoNombre.value = file.name
  fileInput.value._file = file
}

async function parsearArchivo() {
  archivoError.value = ''
  const file = fileInput.value._file
  if (!file) { archivoError.value = 'Selecciona un archivo primero.'; return }

  const XLSX = await import('xlsx')
  const buffer = await file.arrayBuffer()
  const wb = XLSX.read(buffer, { type: 'array' })
  const ws = wb.Sheets[wb.SheetNames[0]]
  const rows = XLSX.utils.sheet_to_json(ws, { header: 1, defval: '' })

  if (rows.length < 2) {
    archivoError.value = 'El archivo no contiene datos (solo el encabezado o está vacío).'
    return
  }

  const dataRows = rows.slice(1).filter(r => r.some(c => c !== ''))

  filas.value = dataRows.map((r, i) => {
    const fila = {
      _idx: i + 2,
      placa: String(r[0] ?? '').trim(),
      tipoPlaca: String(r[1] ?? '').trim(),
      articulo: String(r[2] ?? '').trim(),
      marca: String(r[3] ?? '').trim(),
      modelo: String(r[4] ?? '').trim(),
      numSerial: String(r[5] ?? '').trim(),
      categoriaNombre: String(r[6] ?? '').trim(),
      ubicacionActual: String(r[7] ?? '').trim(),
      encargadoNombre: String(r[8] ?? '').trim(),
      observaciones: String(r[9] ?? '').trim(),
      _errores: []
    }

    if (!fila.placa) fila._errores.push('Placa vacía')
    if (!fila.articulo) fila._errores.push('Artículo vacío')
    if (!fila.marca) fila._errores.push('Marca vacía')
    if (!fila.modelo) fila._errores.push('Modelo vacío')
    if (!fila.numSerial) fila._errores.push('N° Serial vacío')
    if (!['Institucional', 'Interno'].includes(fila.tipoPlaca)) fila._errores.push('Tipo Placa inválido (Institucional / Interno)')
    if (!fila.ubicacionActual) fila._errores.push('Ubicación vacía')
    if (!categoriaValida(fila.categoriaNombre)) fila._errores.push('Categoría no reconocida')
    if (!encargadoValido(fila.encargadoNombre)) fila._errores.push('Encargado no reconocido')

    return fila
  })

  paso.value = 2
}

async function confirmarImportacion() {
  if (filasConError.value.length > 0) {
    const ok = await dialog.confirm({
      type: 'warning',
      title: 'Hay filas con errores',
      message: `Solo se importarán los <strong>${filasValidas.value.length} activo${filasValidas.value.length !== 1 ? 's' : ''}</strong> sin errores. Las <strong>${filasConError.value.length} fila${filasConError.value.length !== 1 ? 's' : ''}</strong> marcadas en rojo serán ignoradas y no se agregarán al inventario. ¿Desea continuar?`,
      confirmText: 'Sí, importar los válidos',
      cancelText: 'Cancelar'
    })
    if (!ok) return
  }
  importando.value = true
  try {
    const payload = filasValidas.value.map(f => ({
      placa: f.placa,
      tipoPlaca: f.tipoPlaca,
      articulo: f.articulo,
      marca: f.marca,
      modelo: f.modelo,
      numSerial: f.numSerial,
      categoriaNombre: f.categoriaNombre,
      ubicacionActual: f.ubicacionActual,
      encargadoNombre: f.encargadoNombre,
      observaciones: f.observaciones || null
    }))
    const { data } = await activoService.importar(payload)
    resultados.value = data
    paso.value = 3
    if (resultadosExitosos.value > 0) emit('importado')
  } finally {
    importando.value = false
  }
}
</script>

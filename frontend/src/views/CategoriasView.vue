<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Categorías</h1>
        <p class="text-gray-600 mt-1">Administra las categorías de activos del sistema</p>
      </div>
      <button v-if="auth.esAdministradora" @click="abrirModalCategoria(null)" class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" /></svg>
        Nueva Categoría
      </button>
    </div>

    <div class="bg-gradient-to-r from-blue-50 to-blue-100/50 border border-blue-200/50 rounded-xl p-4">
      <p class="text-sm text-blue-800">
        <strong>Nota:</strong> Solo usuarios con rol de Administradora pueden agregar nuevas categorías.
      </p>
    </div>

    <!-- Buscador -->
    <div class="flex items-center gap-3">
      <div class="relative max-w-xs w-full">
        <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          v-model="busquedaCat"
          type="text"
          placeholder="Buscar categoría..."
          class="w-full pl-9 pr-3 py-2 text-sm bg-white/80 border border-blue-200/70 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-[#0066cc]/15 focus:border-[#0066cc]/40 transition-all"
        />
      </div>
      <span v-if="busquedaCat" class="text-sm text-gray-400">{{ categoriasFiltradas.length }} resultado{{ categoriasFiltradas.length !== 1 ? 's' : '' }}</span>
    </div>

    <!-- Grid de tarjetas -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="cat in categoriasFiltradas"
        :key="cat.id"
        @click="seleccionarCategoria(cat)"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 hover:shadow-xl transition-all duration-300 border cursor-pointer"
        :class="panelCat?.id === cat.id
          ? 'border-[#0066cc] ring-2 ring-[#0066cc]/30 scale-105 shadow-blue-200/60 shadow-xl'
          : 'border-gray-200 hover:border-blue-300 hover:shadow-md hover:scale-105'"
      >
        <div class="flex items-start justify-between">
          <div class="flex items-center gap-3">
            <div class="text-4xl">{{ cat.icono || DEFAULT_EMOJI }}</div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">{{ cat.nombre }}</h3>
              <p class="text-xs mt-0.5" :class="panelCat?.id === cat.id ? 'text-[#0066cc] font-medium' : 'text-gray-400'">
                {{ panelCat?.id === cat.id ? 'Seleccionada ▾' : 'Ver activos →' }}
              </p>
            </div>
          </div>
          <div v-if="auth.esAdministradora" class="flex gap-1" @click.stop>
            <button @click="abrirModalCategoria(cat)" class="p-2 text-green-600 hover:bg-green-50 rounded transition" title="Editar">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
            </button>
            <button @click="eliminar(cat)" class="p-2 text-red-600 hover:bg-red-50 rounded transition" title="Eliminar">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Panel de activos de la categoría seleccionada -->
    <Transition name="panel-slide">
      <div v-if="panelCat" class="bg-white rounded-2xl shadow-lg overflow-hidden border border-blue-200">

        <!-- Encabezado del panel -->
        <div class="bg-gradient-to-r from-[#003d7a] to-[#0055a8] text-white px-6 py-4 flex items-center justify-between">
          <div class="flex items-center gap-3">
            <span class="text-2xl">{{ panelCat.icono || DEFAULT_EMOJI }}</span>
            <div>
              <h2 class="text-lg font-bold">{{ panelCat.nombre }}</h2>
              <p class="text-blue-200 text-sm">
                {{ panelCargando ? 'Cargando...' : `${panelActivos.length} activo${panelActivos.length !== 1 ? 's' : ''}` }}
              </p>
            </div>
          </div>
          <div class="flex items-center gap-3">
            <!-- Buscador -->
            <div class="relative">
              <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-blue-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <input
                v-model="panelBusqueda"
                type="text"
                placeholder="Buscar..."
                class="pl-9 pr-4 py-1.5 bg-white/10 border border-white/20 rounded-lg text-sm text-white placeholder-blue-300 focus:outline-none focus:bg-white/20 w-48"
              />
            </div>
            <button @click="cerrarPanel" class="p-2 hover:bg-white/10 rounded-lg transition" title="Cerrar">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
            </button>
          </div>
        </div>

        <!-- Tabla (mismo estilo que Inventario) -->
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-[#003d7a]">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-semibold text-white uppercase tracking-wider">Placa</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-white uppercase tracking-wider">Artículo</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-white uppercase tracking-wider">Marca / Modelo</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-white uppercase tracking-wider">Ubicación</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-white uppercase tracking-wider">Encargado</th>
                <th class="px-6 py-3 text-left text-xs font-semibold text-white uppercase tracking-wider">Estado</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200">
              <tr v-if="panelCargando">
                <td colspan="6" class="px-6 py-10 text-center text-gray-400">Cargando...</td>
              </tr>
              <tr v-else-if="panelActivosFiltrados.length === 0">
                <td colspan="6" class="px-6 py-10 text-center text-gray-400">
                  {{ panelBusqueda ? 'No se encontraron resultados.' : 'Esta categoría no tiene activos.' }}
                </td>
              </tr>
              <tr
                v-for="(activo, idx) in panelActivosFiltrados"
                :key="activo.placa"
                @click="abrirActivo(activo)"
                class="hover:bg-blue-50 transition-all duration-200 cursor-pointer"
                :class="idx % 2 === 0 ? 'bg-white' : 'bg-gray-50'"
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
                <td class="px-6 py-4 text-sm text-gray-900">{{ activo.ubicacionActual }}</td>
                <td class="px-6 py-4 text-sm text-gray-900">{{ activo.encargadoActual }}</td>
                <td class="px-6 py-4">
                  <span class="px-3 py-1 rounded-full text-xs font-medium" :class="estadoClase(activo.estado)">
                    {{ activo.estado }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Footer con conteo -->
        <div v-if="!panelCargando && panelActivos.length > 0" class="bg-gray-50 px-6 py-3 border-t border-gray-200">
          <p class="text-sm text-gray-500">
            Mostrando <span class="font-medium text-gray-700">{{ panelActivosFiltrados.length }}</span>
            de <span class="font-medium text-gray-700">{{ panelActivos.length }}</span> activos
          </p>
        </div>
      </div>
    </Transition>

    <!-- ActivoModal -->
    <ActivoModal
      v-if="activoModalOpen"
      :mode="activoModalMode"
      :activo="activoSeleccionado"
      :categorias="categorias"
      :encargados="encargados"
      @close="activoModalOpen = false"
      @saved="onActivoSaved"
      @edit="activoModalMode = 'edit'"
      @cancelEdit="activoModalMode = 'view'"
      @enviarDesecho="confirmarDesecho"
    />

    <!-- Modal crear/editar categoría -->
    <Teleport to="body">
    <div v-if="modalOpen" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white/95 backdrop-blur-sm rounded-2xl shadow-2xl max-w-lg w-full border border-blue-100/50 max-h-[90vh] flex flex-col">
        <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl flex-shrink-0">
          <h2 class="text-xl font-bold">{{ editando ? 'Editar Categoría' : 'Nueva Categoría' }}</h2>
          <button @click="modalOpen = false" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <form @submit.prevent="guardar" class="p-6 space-y-4 overflow-y-auto">
          <p v-if="formError" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ formError }}</p>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
            <input v-model="form.nombre" type="text" maxlength="30" required class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Ícono</label>
            <div class="flex items-center gap-3 mb-3">
              <div class="w-12 h-12 rounded-xl border-2 border-blue-200 bg-blue-50 flex items-center justify-center text-3xl flex-shrink-0 emoji-font">
                {{ form.icono || DEFAULT_EMOJI }}
              </div>
              <span class="text-sm text-gray-500">Selecciona uno de la lista o escribe un emoji personalizado</span>
            </div>
            <div class="border border-gray-200 rounded-xl overflow-hidden">
              <div v-for="grupo in EMOJI_GRUPOS" :key="grupo.label">
                <p class="text-xs font-semibold text-gray-500 uppercase tracking-wider bg-gray-50 px-3 py-1.5 border-b border-gray-100">{{ grupo.label }}</p>
                <div class="flex flex-wrap gap-1 p-2">
                  <button
                    v-for="emoji in grupo.emojis"
                    :key="emoji"
                    type="button"
                    @click="form.icono = emoji"
                    class="w-9 h-9 text-xl rounded-lg transition-all hover:bg-blue-50 hover:scale-110 flex items-center justify-center emoji-font"
                    :class="form.icono === emoji ? 'bg-blue-100 ring-2 ring-[#0066cc]' : ''"
                  >{{ emoji }}</button>
                </div>
              </div>
            </div>
            <div class="mt-2 flex items-center gap-2">
              <label class="text-xs text-gray-500 whitespace-nowrap">Otro emoji:</label>
              <input v-model="form.icono" type="text" maxlength="4" :placeholder="DEFAULT_EMOJI"
                class="w-24 px-3 py-1.5 text-center text-lg border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
            </div>
          </div>
          <div class="flex gap-3 pt-2">
            <button type="button" @click="modalOpen = false" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">Cancelar</button>
            <button type="submit" :disabled="formLoading" class="flex-1 px-4 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400">
              {{ formLoading ? 'Guardando...' : editando ? 'Guardar' : 'Crear' }}
            </button>
          </div>
        </form>
      </div>
    </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'
import categoriaService from '@/services/categoriaService'
import activoService from '@/services/activoService'
import encargadoService from '@/services/encargadoService'
import ActivoModal from '@/components/ActivoModal.vue'
import EMOJI_GRUPOS from '@/utils/emojiGrupos'

const auth = useAuthStore()
const dialog = useDialog()

const DEFAULT_EMOJI = String.fromCodePoint(0x1F4E6) // 📦

// ── Datos base ───────────────────────────────────────────────────────────────
const categorias = ref([])
const busquedaCat = ref('')
const categoriasFiltradas = computed(() => {
  const q = busquedaCat.value.trim().toLowerCase()
  if (!q) return categorias.value
  return categorias.value.filter(c => c.nombre.toLowerCase().includes(q))
})
const encargados = ref([])

onMounted(async () => {
  await cargar()
  const { data } = await encargadoService.listar()
  encargados.value = data
})

async function cargar() {
  const { data } = await categoriaService.listar()
  categorias.value = data
}

// ── Panel inline de activos ──────────────────────────────────────────────────
const panelCat = ref(null)
const panelActivos = ref([])
const panelCargando = ref(false)
const panelBusqueda = ref('')

const panelActivosFiltrados = computed(() => {
  const q = panelBusqueda.value.trim().toLowerCase()
  if (!q) return panelActivos.value
  return panelActivos.value.filter(a =>
    a.placa?.toLowerCase().includes(q) ||
    a.articulo?.toLowerCase().includes(q) ||
    a.marca?.toLowerCase().includes(q) ||
    a.modelo?.toLowerCase().includes(q) ||
    a.encargadoActual?.toLowerCase().includes(q)
  )
})

async function seleccionarCategoria(cat) {
  if (panelCat.value?.id === cat.id) {
    cerrarPanel()
    return
  }
  panelCat.value = cat
  panelBusqueda.value = ''
  panelActivos.value = []
  panelCargando.value = true
  try {
    const { data } = await activoService.listar({ categoriaId: cat.id, tamano: 500 })
    panelActivos.value = data.items
  } finally {
    panelCargando.value = false
  }
}

function cerrarPanel() {
  panelCat.value = null
  panelActivos.value = []
  panelBusqueda.value = ''
}

// ── ActivoModal ──────────────────────────────────────────────────────────────
const activoModalOpen = ref(false)
const activoModalMode = ref('view')
const activoSeleccionado = ref(null)

function abrirActivo(activo) {
  activoSeleccionado.value = activo
  activoModalMode.value = 'view'
  activoModalOpen.value = true
}

async function onActivoSaved() {
  activoModalOpen.value = false
  if (panelCat.value) await seleccionarCategoria(panelCat.value)
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
    activoModalOpen.value = false
    if (panelCat.value) await seleccionarCategoria(panelCat.value)
  } catch (e) {
    await dialog.alert({
      title: 'No se pudo actualizar',
      message: e.response?.data?.mensaje || 'No se pudo enviar el activo a desecho.',
      type: 'danger'
    })
  }
}

// ── Modal crear/editar categoría ─────────────────────────────────────────────
const modalOpen = ref(false)
const editando = ref(null)
const formError = ref('')
const formLoading = ref(false)
const form = ref({ nombre: '', icono: '' })


function abrirModalCategoria(cat) {
  editando.value = cat
  form.value = { nombre: cat?.nombre || '', icono: cat?.icono || '' }
  formError.value = ''
  modalOpen.value = true
}

async function guardar() {
  formError.value = ''
  formLoading.value = true
  try {
    const payload = { nombre: form.value.nombre, icono: form.value.icono || null }
    if (editando.value) {
      await categoriaService.editar(editando.value.id, payload)
    } else {
      await categoriaService.crear(payload)
    }
    modalOpen.value = false
    await cargar()
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al guardar.'
  } finally {
    formLoading.value = false
  }
}

async function eliminar(cat) {
  const ok = await dialog.confirm({
    title: 'Eliminar Categoría',
    message: `¿Eliminar la categoría "${cat.nombre}"? Solo es posible si no tiene activos asociados.`,
    confirmText: 'Eliminar',
    type: 'danger'
  })
  if (!ok) return
  try {
    await categoriaService.eliminar(cat.id)
    if (panelCat.value?.id === cat.id) cerrarPanel()
    await cargar()
  } catch (e) {
    await dialog.alert({
      title: 'No se pudo eliminar',
      message: e.response?.data?.mensaje || 'No se pudo eliminar la categoría.',
      type: 'danger'
    })
  }
}

const estadoClases = {
  Activo: 'bg-green-100 text-green-800',
  Mantenimiento: 'bg-yellow-100 text-yellow-800',
  Desecho: 'bg-red-100 text-red-800'
}
function estadoClase(e) { return estadoClases[e] || 'bg-gray-100 text-gray-800' }
</script>

<style scoped>
.panel-slide-enter-active { animation: slideDown 0.3s cubic-bezier(0.4, 0, 0.2, 1); }
.panel-slide-leave-active { animation: slideUp 0.2s cubic-bezier(0.4, 0, 0.2, 1); }
@keyframes slideDown {
  from { opacity: 0; transform: translateY(-12px); }
  to   { opacity: 1; transform: translateY(0); }
}
@keyframes slideUp {
  from { opacity: 1; transform: translateY(0); }
  to   { opacity: 0; transform: translateY(-12px); }
}
.emoji-font {
  font-family: 'Segoe UI Emoji', 'Apple Color Emoji', 'Noto Color Emoji', 'Twemoji Mozilla', sans-serif;
}
</style>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Inventario</h1>
        <p class="text-gray-600 mt-1">Administra los activos institucionales</p>
      </div>
      <button
        v-if="auth.esGTI"
        @click="abrirModal('create', null)"
        class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Nuevo Activo
      </button>
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
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg overflow-hidden border border-blue-100/50">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gradient-to-r from-blue-50/50 to-blue-100/30 border-b border-blue-100/50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Placa</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Artículo</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Marca / Modelo</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Categoría</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Ubicación</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Estado</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Acciones</th>
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
              v-for="activo in activos"
              :key="activo.placa"
              class="hover:bg-blue-50/20 transition-all duration-200"
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
              <td class="px-6 py-4">
                <span
                  class="px-3 py-1 rounded-full text-xs font-medium"
                  :class="estadoClase(activo.estado)"
                >{{ activo.estado }}</span>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <button @click="abrirModal('view', activo)" class="p-2 text-blue-600 hover:bg-blue-50 rounded transition" title="Ver">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" /></svg>
                  </button>
                  <button v-if="auth.esGTI" @click="abrirModal('edit', activo)" class="p-2 text-green-600 hover:bg-green-50 rounded transition" title="Editar">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                  </button>
                  <button v-if="auth.esAdministradora" @click="confirmarEliminar(activo)" class="p-2 text-red-600 hover:bg-red-50 rounded transition" title="Eliminar">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Paginación -->
      <div class="bg-gradient-to-r from-gray-50 to-blue-50/30 px-6 py-4 flex items-center justify-between border-t border-blue-100/50">
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

    <!-- Modal -->
    <ActivoModal
      v-if="modalOpen"
      :mode="modalMode"
      :activo="activoSeleccionado"
      :categorias="categorias"
      @close="modalOpen = false"
      @saved="onSaved"
    />
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import activoService from '@/services/activoService'
import categoriaService from '@/services/categoriaService'
import ActivoModal from '@/components/ActivoModal.vue'

const auth = useAuthStore()

const activos = ref([])
const total = ref(0)
const pagina = ref(1)
const tamano = ref(10)
const busqueda = ref('')
const categoriaId = ref('')
const estado = ref('')
const categorias = ref([])
const loading = ref(false)
const modalOpen = ref(false)
const modalMode = ref('create')
const activoSeleccionado = ref(null)

const totalPaginas = computed(() => Math.max(1, Math.ceil(total.value / tamano.value)))

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
  const { data } = await categoriaService.listar()
  categorias.value = data
  await cargarActivos()
})

watch([pagina], cargarActivos)
watch([busqueda, categoriaId, estado], () => { pagina.value = 1; cargarActivos() })

function abrirModal(mode, activo) {
  modalMode.value = mode
  activoSeleccionado.value = activo
  modalOpen.value = true
}

async function confirmarEliminar(activo) {
  if (!confirm(`¿Eliminar el activo ${activo.placa}? Solo es posible si lleva +1 año en Desecho.`)) return
  try {
    await activoService.eliminar(activo.placa)
    await cargarActivos()
  } catch (e) {
    alert(e.response?.data?.mensaje || 'No se pudo eliminar el activo.')
  }
}

async function onSaved() {
  modalOpen.value = false
  await cargarActivos()
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

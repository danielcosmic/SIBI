<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Encargados</h1>
        <p class="text-gray-600 mt-1">Personas responsables de los activos institucionales</p>
      </div>
      <button
        v-if="auth.esGTI"
        @click="abrirModalCrear"
        class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Nuevo Encargado
      </button>
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
          placeholder="Buscar encargado..."
          class="w-full pl-9 pr-3 py-2 text-sm bg-white/80 border border-blue-200/70 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-[#0066cc]/15 focus:border-[#0066cc]/40 transition-all"
        />
      </div>
      <span v-if="busqueda" class="text-sm text-gray-400">{{ encargadosFiltrados.length }} resultado{{ encargadosFiltrados.length !== 1 ? 's' : '' }}</span>
    </div>

    <!-- Grid de tarjetas -->
    <div v-if="loading" class="text-center py-12 text-gray-400">Cargando...</div>
    <div v-else-if="encargados.length === 0" class="bg-white/90 rounded-2xl shadow-lg p-12 text-center text-gray-400 border border-blue-100/50">
      No hay encargados registrados.
    </div>
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="e in encargadosFiltrados"
        :key="e.id"
        @click="seleccionarEncargado(e)"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-md hover:shadow-lg p-5 border cursor-pointer transition-all duration-200 hover:scale-[1.02]"
        :class="encargadoSeleccionado?.id === e.id
          ? 'border-[#0066cc] ring-2 ring-[#0066cc]/30 shadow-lg shadow-blue-100'
          : 'border-gray-200 hover:border-blue-300 hover:shadow-md'"
      >
        <div class="flex items-start gap-4">
          <!-- Avatar con iniciales -->
          <div class="w-12 h-12 rounded-full flex items-center justify-center flex-shrink-0 text-white font-bold text-sm shadow-md"
            :class="encargadoSeleccionado?.id === e.id ? 'bg-gradient-to-br from-[#0066cc] to-[#003d7a]' : 'bg-gradient-to-br from-[#4da6ff] to-[#003d7a]'">
            {{ iniciales(e.nombre) }}
          </div>
          <div class="flex-1 min-w-0">
            <p class="font-semibold text-gray-900 truncate">{{ e.nombre }}</p>
            <span class="inline-block mt-1 px-2.5 py-0.5 bg-blue-50 text-blue-700 rounded-full text-xs font-medium">{{ e.rol }}</span>
          </div>
          <svg xmlns="http://www.w3.org/2000/svg"
            class="w-4 h-4 flex-shrink-0 mt-1 transition-transform duration-200"
            :class="encargadoSeleccionado?.id === e.id ? 'rotate-180 text-[#0066cc]' : 'text-gray-300'"
            fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
          </svg>
        </div>

        <!-- Estadística de activos -->
        <div class="mt-4 pt-3 border-t border-gray-100 flex items-center justify-between">
          <div class="flex items-center gap-1.5 text-sm text-gray-500">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
            </svg>
            <span>Activos asignados</span>
          </div>
          <span class="text-lg font-bold" :class="e.totalActivos > 0 ? 'text-[#003d7a]' : 'text-gray-300'">
            {{ e.totalActivos }}
          </span>
        </div>
      </div>
    </div>

    <!-- Panel expandible de activos (debajo del grid) -->
    <Transition name="panel-slide">
      <div v-if="encargadoSeleccionado" class="bg-white rounded-2xl shadow-lg overflow-hidden border border-blue-200">
        <!-- Cabecera del panel -->
        <div class="bg-gradient-to-r from-[#003d7a] to-[#0055a8] text-white px-6 py-4 flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-full bg-white/20 flex items-center justify-center font-bold text-sm">
              {{ iniciales(encargadoSeleccionado.nombre) }}
            </div>
            <div>
              <p class="font-bold">{{ encargadoSeleccionado.nombre }}</p>
              <p class="text-blue-200 text-sm">{{ encargadoSeleccionado.rol }}</p>
            </div>
          </div>
          <div class="flex items-center gap-2">
            <button
              v-if="auth.esAdministradora"
              @click.stop="eliminar(encargadoSeleccionado)"
              class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium text-white/80 hover:bg-white/10 rounded-lg transition"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              Eliminar
            </button>
            <button
              v-if="auth.esGTI"
              @click.stop="abrirModalEditar(encargadoSeleccionado)"
              class="flex items-center gap-1.5 px-3 py-1.5 text-xs font-medium bg-white/15 hover:bg-white/25 rounded-lg transition"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
              Editar
            </button>
            <button @click="encargadoSeleccionado = null" class="p-1.5 hover:bg-white/10 rounded-lg transition">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Tabla de activos -->
        <div v-if="cargandoActivos" class="py-10 text-center text-gray-400 text-sm">Cargando activos...</div>
        <div v-else-if="activosDelEncargado.length === 0" class="py-10 text-center text-gray-400 text-sm">Este encargado no tiene activos asignados.</div>
        <div v-else class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead class="bg-[#003d7a] border-b-2 border-[#002d5a]">
              <tr>
                <th class="text-left px-6 py-3 text-xs font-semibold text-white uppercase tracking-wider">Placa</th>
                <th class="text-left px-6 py-3 text-xs font-semibold text-white uppercase tracking-wider">Artículo</th>
                <th class="text-left px-6 py-3 text-xs font-semibold text-white uppercase tracking-wider">Marca / Modelo</th>
                <th class="text-left px-6 py-3 text-xs font-semibold text-white uppercase tracking-wider">Ubicación</th>
                <th class="text-left px-6 py-3 text-xs font-semibold text-white uppercase tracking-wider">Estado</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200">
              <tr v-for="(activo, idx) in activosDelEncargado" :key="activo.placa"
                class="transition-colors duration-150"
                :class="idx % 2 === 0 ? 'bg-white hover:bg-blue-50' : 'bg-gray-50 hover:bg-blue-50'">
                <td class="px-6 py-3 font-mono text-[#003d7a] font-medium">{{ activo.placa }}</td>
                <td class="px-6 py-3 text-gray-800">{{ activo.articulo }}</td>
                <td class="px-6 py-3 text-gray-700">{{ activo.marca }} {{ activo.modelo }}</td>
                <td class="px-6 py-3 text-gray-700">{{ activo.ubicacionActual }}</td>
                <td class="px-6 py-3">
                  <span class="px-2.5 py-0.5 rounded-full text-xs font-semibold"
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

    <!-- Modal Crear -->
    <Teleport to="body">
      <div v-if="mostrarModalCrear" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
        <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full">
          <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
            <h2 class="text-xl font-bold">Nuevo Encargado</h2>
            <button @click="cerrarModalCrear" class="p-1 hover:bg-white/10 rounded">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
          <form @submit.prevent="guardarCrear" class="p-6 space-y-4">
            <p v-if="formError" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ formError }}</p>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Nombre <span class="text-red-500">*</span></label>
              <input v-model="formCrear.nombre" type="text" maxlength="50" required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
                placeholder="Nombre completo" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1">Rol <span class="text-red-500">*</span></label>
              <input v-model="formCrear.rol" type="text" maxlength="30" required
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
                placeholder="Cargo o función" />
            </div>
            <div class="flex gap-3 pt-2">
              <button type="button" @click="cerrarModalCrear"
                class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
                Cancelar
              </button>
              <button type="submit" :disabled="formLoading"
                class="flex-1 px-4 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400">
                {{ formLoading ? 'Guardando...' : 'Crear' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Teleport>

    <!-- Modal Editar -->
    <Teleport to="body">
      <div v-if="mostrarModalEditar" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] flex flex-col">
          <!-- Header fijo -->
          <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl flex-shrink-0">
            <h2 class="text-xl font-bold">Editar Encargado</h2>
            <button @click="cerrarModalEditar" class="p-1 hover:bg-white/10 rounded">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <!-- Cuerpo scrollable -->
          <div class="overflow-y-auto flex-1 p-6 space-y-6">
            <p v-if="formError" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ formError }}</p>

            <!-- Info del encargado -->
            <div class="space-y-4">
              <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wide">Información</h3>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Nombre <span class="text-red-500">*</span></label>
                  <input v-model="formEditar.nombre" type="text" maxlength="50" required
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
                    placeholder="Nombre completo" />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">Rol <span class="text-red-500">*</span></label>
                  <input v-model="formEditar.rol" type="text" maxlength="30" required
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
                    placeholder="Cargo o función" />
                </div>
              </div>
            </div>

            <hr class="border-gray-200" />

            <!-- Activos asignados -->
            <div>
              <h3 class="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
                Activos asignados ({{ activosEditar.length }})
              </h3>

              <!-- Barra de acciones (solo cuando hay selección) -->
              <Transition
                enter-active-class="transition-all duration-200 ease-out"
                enter-from-class="opacity-0 -translate-y-1"
                enter-to-class="opacity-100 translate-y-0"
                leave-active-class="transition-all duration-150 ease-in"
                leave-from-class="opacity-100 translate-y-0"
                leave-to-class="opacity-0 -translate-y-1"
              >
                <div v-if="seleccionados.length > 0" class="mb-3 p-3 bg-blue-50 border border-blue-200 rounded-xl flex flex-wrap items-center gap-3">
                  <span class="text-sm font-medium text-[#003d7a]">
                    {{ seleccionados.length }} activo{{ seleccionados.length > 1 ? 's' : '' }} seleccionado{{ seleccionados.length > 1 ? 's' : '' }}
                  </span>
                  <div class="flex items-center gap-2 ml-auto">
                    <select
                      v-model="encargadoDestino"
                      class="px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
                    >
                      <option value="">Reasignar a...</option>
                      <option v-for="enc in otrosEncargados" :key="enc.id" :value="enc.id">
                        {{ enc.nombre }} — {{ enc.rol }}
                      </option>
                    </select>
                    <button
                      @click="aplicarReasignar"
                      :disabled="!encargadoDestino || reasignando"
                      class="px-3 py-1.5 text-sm font-medium text-white bg-[#0066cc] hover:bg-[#0055aa] rounded-lg transition disabled:bg-gray-300 disabled:cursor-not-allowed"
                    >
                      {{ reasignando ? 'Aplicando...' : 'Aplicar' }}
                    </button>
                  </div>
                </div>
              </Transition>

              <!-- Tabla de activos con checkboxes -->
              <div v-if="cargandoActivosEditar" class="py-6 text-center text-sm text-gray-400">Cargando activos...</div>
              <div v-else-if="activosEditar.length === 0" class="py-6 text-center text-sm text-gray-400">
                Este encargado no tiene activos asignados.
              </div>
              <div v-else class="rounded-xl border border-gray-200 overflow-hidden">
                <table class="w-full text-sm">
                  <thead class="bg-[#003d7a]">
                    <tr>
                      <th class="px-4 py-2.5 w-10">
                        <input
                          type="checkbox"
                          :checked="todosSeleccionados"
                          :indeterminate="seleccionados.length > 0 && !todosSeleccionados"
                          @change="toggleTodos"
                          class="rounded border-white/50 text-white focus:ring-white cursor-pointer"
                        />
                      </th>
                      <th class="text-left px-4 py-2.5 text-xs font-semibold text-white uppercase tracking-wide">Placa</th>
                      <th class="text-left px-4 py-2.5 text-xs font-semibold text-white uppercase tracking-wide">Artículo</th>
                      <th class="text-left px-4 py-2.5 text-xs font-semibold text-white uppercase tracking-wide">Marca / Modelo</th>
                      <th class="text-left px-4 py-2.5 text-xs font-semibold text-white uppercase tracking-wide">Estado</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-gray-200">
                    <tr
                      v-for="(activo, idx) in activosEditar"
                      :key="activo.placa"
                      @click="toggleSeleccion(activo.placa)"
                      class="transition cursor-pointer"
                      :class="seleccionados.includes(activo.placa)
                        ? 'bg-blue-100 hover:bg-blue-100'
                        : idx % 2 === 0 ? 'bg-white hover:bg-blue-50' : 'bg-gray-50 hover:bg-blue-50'"
                    >
                      <td class="px-4 py-3" @click.stop>
                        <input
                          type="checkbox"
                          :value="activo.placa"
                          v-model="seleccionados"
                          class="rounded border-gray-300 text-[#0066cc] focus:ring-[#0066cc] cursor-pointer"
                        />
                      </td>
                      <td class="px-4 py-3 font-mono text-[#003d7a] font-medium">{{ activo.placa }}</td>
                      <td class="px-4 py-3 text-gray-800">{{ activo.articulo }}</td>
                      <td class="px-4 py-3 text-gray-600">{{ activo.marca }} {{ activo.modelo }}</td>
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
          </div>

          <!-- Footer fijo -->
          <div class="flex gap-3 px-6 py-4 border-t border-gray-100 flex-shrink-0">
            <button type="button" @click="cerrarModalEditar"
              class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
              Cancelar
            </button>
            <button @click="guardarEditar" :disabled="formLoading"
              class="flex-1 px-4 py-2.5 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400">
              {{ formLoading ? 'Guardando...' : 'Guardar Cambios' }}
            </button>
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
import encargadoService from '@/services/encargadoService'

const auth = useAuthStore()
const dialog = useDialog()

const encargados = ref([])
const busqueda = ref('')
const encargadosFiltrados = computed(() => {
  const q = busqueda.value.trim().toLowerCase()
  if (!q) return encargados.value
  return encargados.value.filter(e =>
    e.nombre.toLowerCase().includes(q) || e.rol.toLowerCase().includes(q)
  )
})
const loading = ref(false)

const encargadoSeleccionado = ref(null)
const activosDelEncargado = ref([])
const cargandoActivos = ref(false)

const mostrarModalCrear = ref(false)
const formCrear = ref({ nombre: '', rol: '' })

const mostrarModalEditar = ref(false)
const encargadoEditando = ref(null)
const formEditar = ref({ nombre: '', rol: '' })
const activosEditar = ref([])
const cargandoActivosEditar = ref(false)
const seleccionados = ref([])
const encargadoDestino = ref('')
const reasignando = ref(false)

const formError = ref('')
const formLoading = ref(false)

const todosSeleccionados = computed(
  () => activosEditar.value.length > 0 && seleccionados.value.length === activosEditar.value.length
)

const otrosEncargados = computed(
  () => encargados.value.filter(e => e.id !== encargadoEditando.value?.id)
)

onMounted(cargar)

async function cargar() {
  loading.value = true
  try {
    const { data } = await encargadoService.listar()
    encargados.value = data
  } finally {
    loading.value = false
  }
}

async function seleccionarEncargado(e) {
  if (encargadoSeleccionado.value?.id === e.id) {
    encargadoSeleccionado.value = null
    activosDelEncargado.value = []
    return
  }
  encargadoSeleccionado.value = e
  activosDelEncargado.value = []
  cargandoActivos.value = true
  try {
    const { data } = await encargadoService.activosDe(e.id)
    activosDelEncargado.value = data
  } finally {
    cargandoActivos.value = false
  }
}

function abrirModalCrear() {
  formCrear.value = { nombre: '', rol: '' }
  formError.value = ''
  mostrarModalCrear.value = true
}

function cerrarModalCrear() {
  mostrarModalCrear.value = false
  formError.value = ''
}

async function guardarCrear() {
  formError.value = ''
  formLoading.value = true
  try {
    await encargadoService.crear(formCrear.value)
    cerrarModalCrear()
    await cargar()
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al crear el encargado.'
  } finally {
    formLoading.value = false
  }
}

async function abrirModalEditar(e) {
  encargadoEditando.value = e
  formEditar.value = { nombre: e.nombre, rol: e.rol }
  formError.value = ''
  seleccionados.value = []
  encargadoDestino.value = ''
  mostrarModalEditar.value = true

  // Reutiliza los activos ya cargados en el panel si son del mismo encargado
  if (encargadoSeleccionado.value?.id === e.id && activosDelEncargado.value.length >= 0) {
    activosEditar.value = activosDelEncargado.value
  } else {
    cargandoActivosEditar.value = true
    activosEditar.value = []
    try {
      const { data } = await encargadoService.activosDe(e.id)
      activosEditar.value = data
    } finally {
      cargandoActivosEditar.value = false
    }
  }
}

function cerrarModalEditar() {
  mostrarModalEditar.value = false
  encargadoEditando.value = null
  seleccionados.value = []
  encargadoDestino.value = ''
  formError.value = ''
}

async function guardarEditar() {
  formError.value = ''
  formLoading.value = true
  try {
    await encargadoService.editar(encargadoEditando.value.id, formEditar.value)
    cerrarModalEditar()
    await cargar()
    // Refresca el panel abierto si era el mismo encargado
    if (encargadoSeleccionado.value) {
      const actualizado = encargados.value.find(e => e.id === encargadoSeleccionado.value.id)
      if (actualizado) encargadoSeleccionado.value = actualizado
    }
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al guardar el encargado.'
  } finally {
    formLoading.value = false
  }
}

function toggleTodos(event) {
  seleccionados.value = event.target.checked
    ? activosEditar.value.map(a => a.placa)
    : []
}

function toggleSeleccion(placa) {
  const idx = seleccionados.value.indexOf(placa)
  if (idx === -1) seleccionados.value.push(placa)
  else seleccionados.value.splice(idx, 1)
}

async function aplicarReasignar() {
  if (!encargadoDestino.value || seleccionados.value.length === 0) return
  reasignando.value = true
  try {
    await encargadoService.reasignar(encargadoEditando.value.id, {
      placas: seleccionados.value,
      nuevoEncargadoId: encargadoDestino.value
    })
    // Refresca activos en el modal
    const { data } = await encargadoService.activosDe(encargadoEditando.value.id)
    activosEditar.value = data
    activosDelEncargado.value = data
    seleccionados.value = []
    encargadoDestino.value = ''
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al reasignar los activos.'
  } finally {
    reasignando.value = false
  }
}

function iniciales(nombre) {
  return nombre.split(' ').filter(Boolean).slice(0, 2).map(p => p[0].toUpperCase()).join('')
}

async function eliminar(e) {
  const ok = await dialog.confirm({
    title: 'Eliminar Encargado',
    message: `¿Eliminar a ${e.nombre}? No es posible si tiene activos asignados.`,
    confirmText: 'Eliminar',
    type: 'danger'
  })
  if (!ok) return
  try {
    await encargadoService.eliminar(e.id)
    encargadoSeleccionado.value = null
    await cargar()
  } catch (err) {
    await dialog.alert({
      title: 'No se pudo eliminar',
      message: err.response?.data?.mensaje || 'No se pudo eliminar el encargado.',
      type: 'danger'
    })
  }
}
</script>

<style scoped>
.panel-slide-enter-active { animation: slideDown 0.25s cubic-bezier(0.4,0,0.2,1); }
.panel-slide-leave-active { animation: slideUp 0.2s cubic-bezier(0.4,0,0.2,1); }
@keyframes slideDown { from { opacity:0; transform:translateY(-10px); } to { opacity:1; transform:translateY(0); } }
@keyframes slideUp   { from { opacity:1; transform:translateY(0); } to { opacity:0; transform:translateY(-10px); } }
</style>

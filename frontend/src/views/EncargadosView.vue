<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Encargados</h1>
        <p class="text-gray-600 mt-1">Personas responsables de los activos institucionales</p>
      </div>
      <button
        v-if="auth.esGTI"
        @click="mostrarModal = true"
        class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Nuevo Encargado
      </button>
    </div>

    <!-- Tabla -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg overflow-hidden border border-blue-100/50">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gradient-to-r from-blue-50/50 to-blue-100/30 border-b border-blue-100/50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Nombre</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Rol</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Acciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <tr v-if="loading">
              <td colspan="3" class="px-6 py-10 text-center text-gray-400">Cargando...</td>
            </tr>
            <tr v-else-if="encargados.length === 0">
              <td colspan="3" class="px-6 py-10 text-center text-gray-400">No hay encargados registrados.</td>
            </tr>
            <tr
              v-for="e in encargados"
              :key="e.id"
              class="hover:bg-blue-50/20 transition-all duration-200"
            >
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-gradient-to-br from-[#003d7a] to-[#0066cc] rounded-full flex items-center justify-center flex-shrink-0">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                  </div>
                  <span class="font-medium text-gray-900">{{ e.nombre }}</span>
                </div>
              </td>
              <td class="px-6 py-4">
                <span class="px-3 py-1 bg-blue-50 text-blue-800 rounded-full text-xs font-medium">{{ e.rol }}</span>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <button
                    v-if="auth.esGTI"
                    @click="editar(e)"
                    class="p-2 text-green-600 hover:bg-green-50 rounded transition"
                    title="Editar"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  <button
                    v-if="auth.esAdministradora"
                    @click="eliminar(e)"
                    class="p-2 text-red-600 hover:bg-red-50 rounded transition"
                    title="Eliminar"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Error de eliminación -->
    <p v-if="errorEliminar" class="text-sm text-red-600 bg-red-50 border border-red-200 p-3 rounded-lg">{{ errorEliminar }}</p>

    <!-- Modal crear/editar -->
    <Teleport to="body">
    <div v-if="mostrarModal" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full">
        <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
          <h2 class="text-xl font-bold">{{ editando ? 'Editar Encargado' : 'Nuevo Encargado' }}</h2>
          <button @click="cerrarModal" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <form @submit.prevent="guardar" class="p-6 space-y-4">
          <p v-if="formError" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ formError }}</p>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nombre <span class="text-red-500">*</span></label>
            <input
              v-model="form.nombre"
              type="text"
              maxlength="50"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
              placeholder="Nombre completo"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Rol <span class="text-red-500">*</span></label>
            <input
              v-model="form.rol"
              type="text"
              maxlength="30"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none"
              placeholder="Cargo o función"
            />
          </div>
          <div class="flex gap-3 pt-2">
            <button type="button" @click="cerrarModal" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
              Cancelar
            </button>
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
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'
import encargadoService from '@/services/encargadoService'

const auth = useAuthStore()
const dialog = useDialog()

const encargados = ref([])
const loading = ref(false)
const mostrarModal = ref(false)
const editando = ref(null)
const formError = ref('')
const errorEliminar = ref('')
const formLoading = ref(false)
const form = ref({ nombre: '', rol: '' })

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

function editar(e) {
  editando.value = e.id
  form.value = { nombre: e.nombre, rol: e.rol }
  formError.value = ''
  errorEliminar.value = ''
  mostrarModal.value = true
}

function cerrarModal() {
  mostrarModal.value = false
  editando.value = null
  formError.value = ''
  form.value = { nombre: '', rol: '' }
}

async function guardar() {
  formError.value = ''
  formLoading.value = true
  try {
    if (editando.value) {
      await encargadoService.editar(editando.value, form.value)
    } else {
      await encargadoService.crear(form.value)
    }
    cerrarModal()
    await cargar()
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al guardar el encargado.'
  } finally {
    formLoading.value = false
  }
}

async function eliminar(e) {
  errorEliminar.value = ''
  const ok = await dialog.confirm({
    title: 'Eliminar Encargado',
    message: `¿Eliminar a ${e.nombre}? No es posible si tiene activos asignados.`,
    confirmText: 'Eliminar',
    type: 'danger'
  })
  if (!ok) return
  try {
    await encargadoService.eliminar(e.id)
    await cargar()
  } catch (err) {
    errorEliminar.value = err.response?.data?.mensaje || 'No se pudo eliminar el encargado.'
  }
}
</script>

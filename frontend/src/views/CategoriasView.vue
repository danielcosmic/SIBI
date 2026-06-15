<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Categorías</h1>
        <p class="text-gray-600 mt-1">Administra las categorías de activos del sistema</p>
      </div>
      <button v-if="auth.esAdministradora" @click="abrirModal(null)" class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" /></svg>
        Nueva Categoría
      </button>
    </div>

    <div class="bg-gradient-to-r from-blue-50 to-blue-100/50 border border-blue-200/50 rounded-xl p-4">
      <p class="text-sm text-blue-800">
        <strong>Nota:</strong> Solo usuarios con rol de Administradora pueden agregar nuevas categorías.
      </p>
    </div>

    <!-- Grid de tarjetas -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="cat in categorias"
        :key="cat.id"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 hover:shadow-xl transition-all duration-300 border border-blue-100/50 bg-gradient-to-br from-blue-50/30 to-white hover:scale-105"
      >
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="text-4xl">{{ cat.icono || '📦' }}</div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">{{ cat.nombre }}</h3>
            </div>
          </div>
          <div v-if="auth.esAdministradora" class="flex gap-1">
            <button @click="abrirModal(cat)" class="p-2 text-green-600 hover:bg-green-50 rounded transition" title="Editar">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
            </button>
            <button @click="eliminar(cat)" class="p-2 text-red-600 hover:bg-red-50 rounded transition" title="Eliminar">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal crear -->
    <Teleport to="body">
    <div v-if="modalOpen" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white/95 backdrop-blur-sm rounded-2xl shadow-2xl max-w-md w-full border border-blue-100/50">
        <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
          <h2 class="text-xl font-bold">{{ editando ? 'Editar Categoría' : 'Nueva Categoría' }}</h2>
          <button @click="modalOpen = false" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <form @submit.prevent="guardar" class="p-6 space-y-4">
          <p v-if="formError" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ formError }}</p>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
            <input v-model="form.nombre" type="text" maxlength="30" required class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Ícono (emoji o texto)</label>
            <input v-model="form.icono" type="text" maxlength="30" placeholder="📦" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
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
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'
import categoriaService from '@/services/categoriaService'

const auth = useAuthStore()
const dialog = useDialog()
const categorias = ref([])
const modalOpen = ref(false)
const editando = ref(null)
const formError = ref('')
const formLoading = ref(false)
const form = ref({ nombre: '', icono: '' })

onMounted(cargar)

async function cargar() {
  const { data } = await categoriaService.listar()
  categorias.value = data
}

function abrirModal(cat) {
  editando.value = cat
  form.value = { nombre: cat?.nombre || '', icono: cat?.icono || '' }
  formError.value = ''
  modalOpen.value = true
}

async function guardar() {
  formError.value = ''
  formLoading.value = true
  try {
    await categoriaService.crear({ nombre: form.value.nombre, icono: form.value.icono || null })
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
    await cargar()
  } catch (e) {
    await dialog.alert({
      title: 'No se pudo eliminar',
      message: e.response?.data?.mensaje || 'No se pudo eliminar la categoría.',
      type: 'danger'
    })
  }
}
</script>

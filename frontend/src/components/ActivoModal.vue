<template>
  <Teleport to="body">
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
      <!-- Header -->
      <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
        <h2 class="text-xl font-bold">
          {{ mode === 'create' ? 'Nuevo Activo' : mode === 'edit' ? 'Editar Activo' : 'Detalle del Activo' }}
        </h2>
        <button @click="$emit('close')" class="p-1 hover:bg-white/10 rounded transition">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Body -->
      <form @submit.prevent="handleSubmit" class="p-6 space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <!-- Placa -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Placa <span class="text-red-500">*</span></label>
            <input
              v-model="form.placa"
              type="text"
              maxlength="8"
              :disabled="mode !== 'create'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
              placeholder="UCR12345"
            />
          </div>

          <!-- Tipo Placa -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Tipo de Placa <span class="text-red-500">*</span></label>
            <select
              v-model="form.tipoPlaca"
              :disabled="mode === 'view'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            >
              <option value="Institucional">Institucional</option>
              <option value="Interno">Interno</option>
            </select>
          </div>

          <!-- Marca -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Marca <span class="text-red-500">*</span></label>
            <input
              v-model="form.marca"
              type="text"
              maxlength="30"
              :disabled="mode === 'view'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            />
          </div>

          <!-- Modelo -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Modelo <span class="text-red-500">*</span></label>
            <input
              v-model="form.modelo"
              type="text"
              maxlength="20"
              :disabled="mode === 'view'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            />
          </div>

          <!-- Número Serial -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Número Serial <span class="text-red-500">*</span></label>
            <input
              v-model="form.numSerial"
              type="text"
              maxlength="30"
              :disabled="mode === 'view'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            />
          </div>

          <!-- Artículo -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Artículo <span class="text-red-500">*</span></label>
            <input
              v-model="form.articulo"
              type="text"
              maxlength="20"
              :disabled="mode === 'view'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            />
          </div>

          <!-- Categoría -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Categoría <span class="text-red-500">*</span></label>
            <SearchableSelect
              v-model="form.categoriaId"
              :options="categoriasOptions"
              :disabled="mode === 'view'"
              placeholder="Buscar categoría..."
            />
          </div>

          <!-- Estado (solo en edición) -->
          <div v-if="mode !== 'create'">
            <label class="block text-sm font-medium text-gray-700 mb-1">Estado</label>
            <select
              v-model="form.estado"
              :disabled="mode === 'view'"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            >
              <option value="Activo">Activo</option>
              <option value="Mantenimiento">Mantenimiento</option>
              <option value="Desecho">Desecho</option>
            </select>
          </div>

          <!-- Ubicación Actual -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Ubicación Actual <span class="text-red-500">*</span></label>
            <input
              v-model="form.ubicacionActual"
              type="text"
              maxlength="30"
              :disabled="mode === 'view'"
              required
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100"
            />
          </div>

          <!-- Encargado -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Encargado <span class="text-red-500">*</span></label>
            <SearchableSelect
              v-model="form.encargadoId"
              :options="encargadosOptions"
              :disabled="mode === 'view'"
              placeholder="Buscar encargado..."
            />
          </div>
        </div>

        <!-- Observaciones (full width) -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Observaciones</label>
          <textarea
            v-model="form.observaciones"
            maxlength="200"
            rows="2"
            :disabled="mode === 'view'"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none resize-none disabled:bg-gray-100"
          />
        </div>

        <!-- Error -->
        <p v-if="error" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ error }}</p>

        <!-- Buttons -->
        <div v-if="mode !== 'view'" class="flex gap-3 pt-2">
          <button
            type="button"
            @click="$emit('close')"
            class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium"
          >
            Cancelar
          </button>
          <button
            type="submit"
            :disabled="loading"
            class="flex-1 px-4 py-2.5 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400"
          >
            {{ loading ? 'Guardando...' : mode === 'create' ? 'Crear Activo' : 'Guardar Cambios' }}
          </button>
        </div>
      </form>
    </div>
  </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import activoService from '@/services/activoService'
import SearchableSelect from '@/components/SearchableSelect.vue'

const props = defineProps({
  mode: { type: String, required: true }, // 'create' | 'edit' | 'view'
  activo: { type: Object, default: null },
  categorias: { type: Array, default: () => [] },
  encargados: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'saved'])

const categoriasOptions = computed(() =>
  props.categorias.map(c => ({ value: c.id, label: `${c.icono ?? ''} ${c.nombre}`.trim() }))
)
const encargadosOptions = computed(() =>
  props.encargados.map(e => ({ value: e.id, label: `${e.nombre} — ${e.rol}` }))
)

const loading = ref(false)
const error = ref('')

const form = ref({
  placa: '',
  tipoPlaca: 'Institucional',
  marca: '',
  modelo: '',
  numSerial: '',
  articulo: '',
  categoriaId: '',
  observaciones: '',
  ubicacionActual: '',
  encargadoId: '',
  estado: 'Activo'
})

watch(() => props.activo, (activo) => {
  if (activo) {
    form.value = {
      placa: activo.placa,
      tipoPlaca: activo.tipoPlaca,
      marca: activo.marca,
      modelo: activo.modelo,
      numSerial: activo.numSerial,
      articulo: activo.articulo,
      categoriaId: activo.categoriaId,
      observaciones: activo.observaciones || '',
      ubicacionActual: activo.ubicacionActual,
      encargadoId: activo.encargadoActualId || '',
      estado: activo.estado
    }
  }
}, { immediate: true })

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    if (props.mode === 'create') {
      await activoService.crear({
        placa: form.value.placa,
        tipoPlaca: form.value.tipoPlaca,
        marca: form.value.marca,
        modelo: form.value.modelo,
        numSerial: form.value.numSerial,
        articulo: form.value.articulo,
        categoriaId: Number(form.value.categoriaId),
        observaciones: form.value.observaciones || null,
        ubicacionActual: form.value.ubicacionActual,
        encargadoId: form.value.encargadoId
      })
    } else {
      await activoService.editar(form.value.placa, {
        marca: form.value.marca,
        modelo: form.value.modelo,
        numSerial: form.value.numSerial,
        articulo: form.value.articulo,
        categoriaId: Number(form.value.categoriaId),
        observaciones: form.value.observaciones || null,
        ubicacionActual: form.value.ubicacionActual,
        encargadoId: form.value.encargadoId,
        estado: form.value.estado
      })
    }
    emit('saved')
  } catch (e) {
    error.value = e.response?.data?.mensaje || 'Error al guardar el activo.'
  } finally {
    loading.value = false
  }
}
</script>

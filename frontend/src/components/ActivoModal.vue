<template>
  <Teleport to="body">
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] flex flex-col">

      <!-- Header -->
      <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl flex-shrink-0">
        <div>
          <h2 class="text-xl font-bold">
            {{ mode === 'create' ? 'Nuevo Activo' : mode === 'edit' ? 'Editar Activo' : 'Detalle del Activo' }}
          </h2>
          <p v-if="mode === 'view'" class="text-blue-200 text-sm mt-0.5">{{ props.activo?.placa }}</p>
        </div>
        <button @click="$emit('close')" class="p-1 hover:bg-white/10 rounded transition">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- ── MODO VIEW: tarjetas detalladas ── -->
      <template v-if="mode === 'view'">
        <div class="overflow-y-auto flex-1 p-6 space-y-6">

          <!-- Identificación -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Identificación</p>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Placa</p>
                <p class="font-semibold text-[#003d7a] font-mono">{{ props.activo?.placa }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Tipo de Placa</p>
                <p class="font-medium text-gray-800">{{ props.activo?.tipoPlaca }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Artículo</p>
                <p class="font-medium text-gray-800">{{ props.activo?.articulo }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Número Serial</p>
                <p class="font-medium text-gray-800 font-mono text-sm">{{ props.activo?.numSerial }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Marca</p>
                <p class="font-medium text-gray-800">{{ props.activo?.marca }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Modelo</p>
                <p class="font-medium text-gray-800">{{ props.activo?.modelo }}</p>
              </div>
            </div>
          </div>

          <!-- Clasificación -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Clasificación</p>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Categoría</p>
                <p class="font-medium text-gray-800">{{ props.activo?.categoriaNombre }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-1">Estado</p>
                <span class="inline-block px-2 py-0.5 rounded-full text-xs font-semibold"
                  :class="{
                    'bg-green-100 text-green-700': props.activo?.estado === 'Activo',
                    'bg-yellow-100 text-yellow-700': props.activo?.estado === 'Mantenimiento',
                    'bg-red-100 text-red-700': props.activo?.estado === 'Desecho'
                  }">
                  {{ props.activo?.estado }}
                </span>
              </div>
              <div v-if="props.activo?.fechaDesecho" class="bg-red-50 rounded-lg p-3 col-span-2">
                <p class="text-xs text-red-400 mb-0.5">Fecha de Desecho</p>
                <p class="font-medium text-red-700">{{ props.activo?.fechaDesecho }}</p>
              </div>
            </div>
          </div>

          <!-- Ubicación -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Ubicación</p>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-blue-50/60 rounded-lg p-3 border border-blue-100">
                <p class="text-xs text-blue-400 mb-0.5">Ubicación Actual</p>
                <p class="font-semibold text-blue-900">{{ props.activo?.ubicacionActual }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Ubicación Anterior</p>
                <p class="font-medium text-gray-600">{{ props.activo?.ubicacionAnterior || '—' }}</p>
              </div>
            </div>
          </div>

          <!-- Encargados -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Encargados</p>
            <div class="grid grid-cols-2 gap-3">
              <div class="bg-blue-50/60 rounded-lg p-3 border border-blue-100">
                <p class="text-xs text-blue-400 mb-0.5">Encargado Actual</p>
                <p class="font-semibold text-blue-900">{{ props.activo?.encargadoActual }}</p>
              </div>
              <div class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Encargado Anterior</p>
                <p class="font-medium text-gray-600">{{ props.activo?.encargadoAnterior || '—' }}</p>
              </div>
            </div>
          </div>

          <!-- Observaciones -->
          <div v-if="props.activo?.observaciones">
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-2">Observaciones</p>
            <div class="bg-gray-50 rounded-lg p-3">
              <p class="text-sm text-gray-700">{{ props.activo?.observaciones }}</p>
            </div>
          </div>
        </div>

        <!-- Footer view -->
        <div class="flex gap-3 px-6 py-4 border-t border-gray-100 flex-shrink-0">
          <button type="button" @click="$emit('close')"
            class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
            Cerrar
          </button>
          <button
            v-if="auth.esGTI && props.activo?.estado !== 'Desecho'"
            type="button"
            @click="$emit('enviarDesecho', props.activo)"
            class="flex-1 px-4 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition font-medium flex items-center justify-center gap-2"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
            Enviar a Desecho
          </button>
          <button
            v-if="auth.esGTI"
            type="button"
            @click="$emit('edit')"
            class="flex-1 px-4 py-2.5 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium flex items-center justify-center gap-2"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
            Editar
          </button>
        </div>
      </template>

      <!-- ── MODO CREATE / EDIT: formulario ── -->
      <template v-else>
        <div class="overflow-y-auto flex-1 p-6 space-y-6">

          <!-- Identificación -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Identificación</p>
            <div class="bg-gray-50/70 rounded-xl p-4 space-y-3">
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Placa <span class="text-red-500">*</span></label>
                  <input v-model="form.placa" type="text" maxlength="8" :disabled="mode !== 'create'" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none disabled:bg-gray-100 disabled:text-gray-500 font-mono"
                    placeholder="UCR12345" />
                </div>
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Tipo de Placa <span class="text-red-500">*</span></label>
                  <select v-model="form.tipoPlaca" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white">
                    <option value="Institucional">Institucional</option>
                    <option value="Interno">Interno</option>
                  </select>
                </div>
              </div>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Artículo <span class="text-red-500">*</span></label>
                  <input v-model="form.articulo" type="text" maxlength="20" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white" />
                </div>
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Número Serial <span class="text-red-500">*</span></label>
                  <input v-model="form.numSerial" type="text" maxlength="30" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white font-mono" />
                </div>
              </div>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Marca <span class="text-red-500">*</span></label>
                  <input v-model="form.marca" type="text" maxlength="30" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white" />
                </div>
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Modelo <span class="text-red-500">*</span></label>
                  <input v-model="form.modelo" type="text" maxlength="20" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white" />
                </div>
              </div>
            </div>
          </div>

          <!-- Clasificación -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Clasificación</p>
            <div class="bg-gray-50/70 rounded-xl p-4">
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Categoría <span class="text-red-500">*</span></label>
                  <SearchableSelect v-model="form.categoriaId" :options="categoriasOptions" placeholder="Buscar categoría..." />
                </div>
                <div v-if="mode !== 'create'">
                  <label class="block text-xs font-medium text-gray-500 mb-1">Estado</label>
                  <select v-model="form.estado"
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white">
                    <option value="Activo">Activo</option>
                    <option value="Mantenimiento">Mantenimiento</option>
                    <option value="Desecho">Desecho</option>
                  </select>
                </div>
              </div>
            </div>
          </div>

          <!-- Ubicación y Responsable -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Ubicación y Responsable</p>
            <div class="bg-gray-50/70 rounded-xl p-4">
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Ubicación Actual <span class="text-red-500">*</span></label>
                  <input v-model="form.ubicacionActual" type="text" maxlength="30" required
                    class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none bg-white" />
                </div>
                <div>
                  <label class="block text-xs font-medium text-gray-500 mb-1">Encargado <span class="text-red-500">*</span></label>
                  <SearchableSelect v-model="form.encargadoId" :options="encargadosOptions" placeholder="Buscar encargado..." />
                </div>
              </div>
            </div>
          </div>

          <!-- Observaciones -->
          <div>
            <p class="text-xs text-gray-400 uppercase tracking-wide font-medium mb-3">Observaciones</p>
            <div class="bg-gray-50/70 rounded-xl p-4">
              <textarea v-model="form.observaciones" maxlength="200" rows="3"
                class="w-full px-3 py-2 text-sm border border-gray-200 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none resize-none bg-white"
                placeholder="Notas adicionales sobre el activo..." />
              <p class="text-xs text-gray-400 mt-1 text-right">{{ (form.observaciones || '').length }}/200</p>
            </div>
          </div>

          <p v-if="error" class="text-sm text-red-600 bg-red-50 border border-red-100 p-3 rounded-lg">{{ error }}</p>
        </div>

        <!-- Footer create/edit -->
        <div class="flex gap-3 px-6 py-4 border-t border-gray-100 flex-shrink-0">
          <button type="button" @click="mode === 'edit' ? $emit('cancelEdit') : $emit('close')"
            class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
            Cancelar
          </button>
          <button @click="handleSubmit" :disabled="loading"
            class="flex-1 px-4 py-2.5 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400 flex items-center justify-center gap-2">
            <svg v-if="loading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
            </svg>
            {{ loading ? 'Guardando...' : mode === 'create' ? 'Crear Activo' : 'Guardar Cambios' }}
          </button>
        </div>
      </template>

    </div>
  </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import activoService from '@/services/activoService'
import SearchableSelect from '@/components/SearchableSelect.vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()

const props = defineProps({
  mode: { type: String, required: true }, // 'create' | 'edit' | 'view'
  activo: { type: Object, default: null },
  categorias: { type: Array, default: () => [] },
  encargados: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'saved', 'edit', 'enviarDesecho', 'cancelEdit'])

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

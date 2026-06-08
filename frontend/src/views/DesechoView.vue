<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Desecho</h1>
      <p class="text-gray-600 mt-1">Administra activos en proceso de eliminación</p>
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

    <!-- Lista de activos en desecho -->
    <div v-if="loading" class="text-center py-10 text-gray-400">Cargando...</div>
    <div v-else-if="items.length === 0" class="text-center py-10 text-gray-400 bg-white rounded-2xl shadow">No hay activos en estado de desecho.</div>
    <div v-else class="space-y-4">
      <div
        v-for="item in items"
        :key="item.activo.placa"
        class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg p-6 hover:shadow-xl transition-all duration-300 border border-red-100/50 bg-gradient-to-r from-red-50/30 to-white"
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
              Rechazar
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal confirmación -->
    <div v-if="itemSeleccionado" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
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
            <button @click="confirmarEliminar" :disabled="actionLoading" class="flex-1 px-4 py-2.5 bg-red-600 text-white rounded-lg hover:bg-red-700 transition font-medium disabled:bg-red-300">
              {{ actionLoading ? 'Eliminando...' : 'Eliminar Permanentemente' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import desechoService from '@/services/desechoService'

const auth = useAuthStore()
const items = ref([])
const loading = ref(false)
const itemSeleccionado = ref(null)
const actionLoading = ref(false)

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
    itemSeleccionado.value = null
    await cargar()
  } catch (e) {
    alert(e.response?.data?.mensaje || 'Error al eliminar.')
  } finally {
    actionLoading.value = false
  }
}

async function rechazar(placa) {
  if (!confirm('¿Rechazar la eliminación y restaurar este activo a estado Activo?')) return
  try { await desechoService.rechazar(placa); await cargar() }
  catch (e) { alert(e.response?.data?.mensaje || 'Error.') }
}

function formatFecha(f) {
  if (!f) return ''
  return new Date(f).toLocaleDateString('es-CR')
}
</script>

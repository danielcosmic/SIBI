<template>
  <div class="flex h-screen bg-gradient-to-br from-gray-50 to-blue-50/30">
    <!-- Sidebar -->
    <div class="w-64 bg-[#003d7a] text-white flex flex-col flex-shrink-0">
      <!-- Logo — redirige al dashboard -->
      <RouterLink to="/dashboard" class="p-6 border-b border-white/10 hover:bg-white/5 transition">
        <div class="flex items-center justify-center">
          <div class="w-32 h-16 flex-shrink-0">
            <img :src="sibiLogo" alt="SIBI" class="w-full h-full object-contain rounded-lg" />
          </div>
        </div>
      </RouterLink>

      <!-- Navigation -->
      <nav class="flex-1 p-4 space-y-1">
        <RouterLink
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          class="flex items-center gap-3 px-4 py-3 rounded-lg transition font-medium"
          :class="$route.path === item.to
            ? 'bg-[#0066cc] text-white'
            : 'text-blue-100 hover:bg-blue-900/50'"
        >
          <component :is="item.icon" class="w-5 h-5" />
          <span>{{ item.label }}</span>
        </RouterLink>
      </nav>

      <!-- Cerrar Sesión -->
      <div class="p-4 border-t border-white/10">
        <button
          @click="cerrarSesion"
          class="w-full flex items-center gap-3 px-4 py-3 text-blue-100 hover:bg-blue-900/50 rounded-lg transition font-medium"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
          <span>Cerrar Sesión</span>
        </button>
      </div>
    </div>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <!-- Top Bar -->
      <div class="bg-white/80 backdrop-blur-md border-b border-gray-200/50 px-8 py-4 shadow-sm relative z-10">
        <div class="flex items-center justify-between">
          <div class="flex-1 max-w-xl" ref="searchWrapper">
            <div class="relative">
              <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <svg v-if="cargandoBusqueda" class="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-blue-400 animate-spin pointer-events-none" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
              </svg>
              <input
                v-model="busquedaGlobal"
                @input="onBusquedaInput"
                @focus="mostrarDropdown = resultados.length > 0"
                @keydown.escape="cerrarBusqueda"
                type="text"
                placeholder="Buscar activos, placas, ubicaciones..."
                class="w-full pl-10 pr-4 py-2 rounded-lg outline-none transition-all duration-200
                       border border-blue-200/80 shadow-sm
                       focus:border-[#0066cc]/50 focus:ring-2 focus:ring-[#0066cc]/15 focus:shadow-[0_0_0_3px_rgba(0,102,204,0.08)]"
                autocomplete="off"
              />

              <!-- Dropdown de resultados -->
              <Transition
                enter-active-class="transition ease-out duration-150"
                enter-from-class="opacity-0 translate-y-1"
                enter-to-class="opacity-100 translate-y-0"
                leave-active-class="transition ease-in duration-100"
                leave-from-class="opacity-100 translate-y-0"
                leave-to-class="opacity-0 translate-y-1"
              >
                <div
                  v-if="mostrarDropdown"
                  class="absolute top-full mt-2 left-0 right-0 bg-white rounded-xl shadow-xl border border-blue-100/80 overflow-hidden z-50"
                >
                  <!-- Sin resultados -->
                  <div v-if="resultados.length === 0 && !cargandoBusqueda" class="px-4 py-6 text-center text-sm text-gray-400">
                    No se encontraron activos para "{{ busquedaGlobal }}"
                  </div>

                  <!-- Lista de resultados -->
                  <div v-else class="divide-y divide-gray-50">
                    <button
                      v-for="a in resultados"
                      :key="a.placa"
                      @click="irAActivo(a)"
                      class="w-full flex items-center gap-3 px-4 py-3 hover:bg-blue-50/60 transition-colors text-left group"
                    >
                      <div class="flex-1 min-w-0">
                        <div class="flex items-center gap-2">
                          <span class="font-semibold text-gray-900 text-sm">{{ a.placa }}</span>
                          <span class="text-gray-300 text-xs">·</span>
                          <span class="text-sm text-gray-700 truncate">{{ a.articulo }}</span>
                        </div>
                        <div class="flex items-center gap-2 mt-0.5">
                          <span class="text-xs text-gray-400 truncate">{{ a.marca }} {{ a.modelo }}</span>
                          <span v-if="a.encargadoActual" class="text-gray-300 text-xs">·</span>
                          <span v-if="a.encargadoActual" class="text-xs text-gray-400 truncate">{{ a.encargadoActual }}</span>
                        </div>
                      </div>
                      <span class="flex-shrink-0 px-2 py-0.5 rounded-full text-xs font-medium" :class="estadoBadge(a.estado)">
                        {{ a.estado }}
                      </span>
                    </button>
                  </div>

                  <!-- Pie: ver todos -->
                  <div v-if="resultados.length > 0" class="border-t border-gray-100 px-4 py-2.5 bg-gray-50/60">
                    <button @click="irAInventario" class="text-xs text-[#0066cc] hover:underline font-medium">
                      Ver todos los resultados en Inventario →
                    </button>
                  </div>
                </div>
              </Transition>
            </div>
          </div>

          <!-- Botón de perfil -->
          <div class="flex items-center gap-4 ml-8">
            <div class="pl-4 border-l border-gray-300">
              <button
                ref="perfilBtn"
                @click="togglePerfil"
                class="flex items-center gap-3 hover:bg-gray-100 rounded-xl px-3 py-2 transition"
              >
                <div class="w-10 h-10 bg-[#003d7a] rounded-full flex items-center justify-center flex-shrink-0">
                  <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                </div>
                <div class="text-left">
                  <p class="text-sm font-medium text-gray-800">{{ auth.nombre }}</p>
                  <p class="text-xs text-gray-500">{{ auth.permisos }}</p>
                </div>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="w-4 h-4 text-gray-400 transition-transform duration-200"
                  :class="perfilAbierto ? 'rotate-180' : ''"
                  fill="none" viewBox="0 0 24 24" stroke="currentColor"
                >
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Page Content -->
      <div class="flex-1 overflow-auto p-8">
        <RouterView />
      </div>
    </div>

    <!-- Dropdown teleportado al body para evitar problemas de stacking context -->
    <Teleport to="body">
      <Transition
        enter-active-class="transition ease-out duration-150"
        enter-from-class="opacity-0 scale-95"
        enter-to-class="opacity-100 scale-100"
        leave-active-class="transition ease-in duration-100"
        leave-from-class="opacity-100 scale-100"
        leave-to-class="opacity-0 scale-95"
      >
        <div
          v-if="perfilAbierto"
          class="fixed w-72 bg-white rounded-2xl shadow-2xl border border-gray-100 overflow-hidden"
          :style="dropdownStyle"
          style="z-index: 9999"
        >
          <!-- Cabecera -->
          <div class="bg-gradient-to-br from-[#003d7a] to-[#0066cc] px-5 py-4">
            <div class="flex items-center gap-3">
              <div class="w-12 h-12 bg-white/20 rounded-full flex items-center justify-center flex-shrink-0">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </div>
              <div>
                <p class="text-white font-semibold">{{ auth.nombre }}</p>
                <p class="text-blue-200 text-xs mt-0.5">{{ auth.correo }}</p>
              </div>
            </div>
          </div>

          <!-- Detalles -->
          <div class="p-4 space-y-1">
            <div class="flex items-center justify-between py-2 border-b border-gray-100">
              <span class="text-xs text-gray-500 uppercase tracking-wide font-medium">Rol</span>
              <span class="px-3 py-1 rounded-full text-xs font-semibold" :class="rolClase(auth.permisos)">
                {{ auth.permisos }}
              </span>
            </div>
            <div class="flex items-center justify-between py-2 border-b border-gray-100">
              <span class="text-xs text-gray-500 uppercase tracking-wide font-medium">Correo</span>
              <span class="text-sm text-gray-700 font-medium">{{ auth.correo }}</span>
            </div>
            <div class="flex items-center justify-between py-2">
              <span class="text-xs text-gray-500 uppercase tracking-wide font-medium">Estado</span>
              <span class="flex items-center gap-1.5 text-sm text-green-600 font-medium">
                <span class="w-2 h-2 bg-green-500 rounded-full"></span>
                Activo
              </span>
            </div>
          </div>

          <!-- Cerrar sesión -->
          <div class="px-4 pb-4">
            <button
              @click="cerrarSesion"
              class="w-full flex items-center justify-center gap-2 px-4 py-2.5 bg-red-50 hover:bg-red-100 text-red-600 rounded-xl transition font-medium text-sm"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
              Cerrar Sesión
            </button>
          </div>

        </div>
      </Transition>

      <!-- Overlay para cerrar al hacer click fuera -->
      <div
        v-if="perfilAbierto"
        class="fixed inset-0"
        style="z-index: 9998"
        @click="perfilAbierto = false"
      />
    </Teleport>

    <AppDialog />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import AppDialog from '@/components/AppDialog.vue'
import sibiLogo from '@/assets/SIBI_logo_4096_fondo_oscuro.png'
import activoService from '@/services/activoService'

const auth = useAuthStore()
const router = useRouter()
const perfilAbierto = ref(false)
const perfilBtn = ref(null)
const dropdownStyle = ref({})

// ── Búsqueda global ──────────────────────────────────────────────────────────
const searchWrapper = ref(null)
const busquedaGlobal = ref('')
const resultados = ref([])
const mostrarDropdown = ref(false)
const cargandoBusqueda = ref(false)
let debounceTimer = null

function onBusquedaInput() {
  clearTimeout(debounceTimer)
  const q = busquedaGlobal.value.trim()
  if (!q) {
    resultados.value = []
    mostrarDropdown.value = false
    return
  }
  debounceTimer = setTimeout(() => buscar(q), 300)
}

async function buscar(q) {
  cargandoBusqueda.value = true
  mostrarDropdown.value = true
  try {
    const { data } = await activoService.listar({ busqueda: q, tamano: 6 })
    resultados.value = data.items
  } catch {
    resultados.value = []
  } finally {
    cargandoBusqueda.value = false
  }
}

function irAActivo(activo) {
  cerrarBusqueda()
  router.push({ path: '/inventario', query: { busqueda: activo.placa } })
}

function irAInventario() {
  const q = busquedaGlobal.value.trim()
  cerrarBusqueda()
  router.push({ path: '/inventario', query: { busqueda: q } })
}

function cerrarBusqueda() {
  mostrarDropdown.value = false
  busquedaGlobal.value = ''
  resultados.value = []
}

function onClickFuera(e) {
  if (searchWrapper.value && !searchWrapper.value.contains(e.target)) {
    mostrarDropdown.value = false
  }
}

onMounted(() => document.addEventListener('mousedown', onClickFuera))
onBeforeUnmount(() => document.removeEventListener('mousedown', onClickFuera))

const estadosBadge = {
  Activo: 'bg-green-100 text-green-700',
  Mantenimiento: 'bg-yellow-100 text-yellow-700',
  Desecho: 'bg-red-100 text-red-700'
}
function estadoBadge(e) { return estadosBadge[e] || 'bg-gray-100 text-gray-600' }

function togglePerfil() {
  if (!perfilAbierto.value) {
    const rect = perfilBtn.value.getBoundingClientRect()
    dropdownStyle.value = {
      top: `${rect.bottom + 8}px`,
      right: `${window.innerWidth - rect.right}px`
    }
  }
  perfilAbierto.value = !perfilAbierto.value
}

const navItems = computed(() => {
  const items = [
    { to: '/dashboard', label: 'Dashboard', icon: IconDashboard },
    { to: '/inventario', label: 'Inventario', icon: IconBox },
  ]
  if (auth.esGTI) {
    items.push({ to: '/encargados', label: 'Encargados', icon: IconPersonCard })
  }
  items.push({ to: '/historial', label: 'Historial', icon: IconHistory })
  if (auth.esAdministradora) {
    items.push({ to: '/usuarios', label: 'Usuarios', icon: IconUsers })
  }
  items.push({ to: '/categorias', label: 'Categorías', icon: IconFolder })
  items.push({ to: '/desecho', label: 'Desecho', icon: IconTrash })
  return items
})

const rolClases = {
  Administradora: 'bg-purple-100 text-purple-800',
  GTI: 'bg-blue-100 text-blue-800',
  JefaAdministrativa: 'bg-green-100 text-green-800',
  Invitado: 'bg-gray-100 text-gray-800'
}
function rolClase(r) { return rolClases[r] || 'bg-gray-100 text-gray-800' }

function cerrarSesion() {
  auth.logout()
  router.push('/')
}
</script>

<!-- Inline icon components -->
<script>
const IconDashboard = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" /></svg>`
}
const IconBox = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" /></svg>`
}
const IconHistory = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>`
}
const IconUsers = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" /></svg>`
}
const IconFolder = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7a2 2 0 012-2h4l2 2h8a2 2 0 012 2v7a2 2 0 01-2 2H5a2 2 0 01-2-2V7z" /></svg>`
}
const IconTrash = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>`
}
const IconPersonCard = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c0 1.657 1.343 2 3 2s3-.343 3-2" /></svg>`
}
</script>

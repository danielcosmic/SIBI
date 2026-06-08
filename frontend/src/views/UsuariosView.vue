<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold text-[#003d7a]">Gestión de Usuarios</h1>
        <p class="text-gray-600 mt-1">Administra roles y permisos del sistema</p>
      </div>
      <button @click="mostrarModal = true" class="bg-[#003d7a] text-white px-6 py-2.5 rounded-lg hover:bg-[#002d5a] transition flex items-center gap-2 font-medium">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" /></svg>
        Nuevo Usuario
      </button>
    </div>

    <!-- Tabla -->
    <div class="bg-white/90 backdrop-blur-sm rounded-2xl shadow-lg overflow-hidden border border-blue-100/50">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gradient-to-r from-blue-50/50 to-blue-100/30 border-b border-blue-100/50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Usuario</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Rol</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Estado</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-700 uppercase tracking-wider">Acciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <tr v-if="loading"><td colspan="4" class="px-6 py-10 text-center text-gray-400">Cargando...</td></tr>
            <tr v-for="u in usuarios" :key="u.correo" class="hover:bg-blue-50/20 transition-all duration-200">
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-[#003d7a] rounded-full flex items-center justify-center">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
                  </div>
                  <div>
                    <p class="font-medium text-gray-900">{{ u.nombre }}</p>
                    <p class="text-sm text-gray-500">{{ u.correo }}</p>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <span class="px-3 py-1 rounded-full text-xs font-medium" :class="rolClase(u.permisos)">{{ u.permisos }}</span>
              </td>
              <td class="px-6 py-4">
                <span class="px-3 py-1 rounded-full text-xs font-medium" :class="u.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                  {{ u.activo ? 'Activo' : 'Inactivo' }}
                </span>
              </td>
              <td class="px-6 py-4">
                <button @click="editarUsuario(u)" class="p-2 text-green-600 hover:bg-green-50 rounded transition" title="Editar">
                  <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Tarjetas de permisos por rol -->
    <h2 class="text-xl font-bold text-[#003d7a]">Roles y Permisos</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="rol in rolesInfo" :key="rol.nombre" class="bg-white rounded-2xl shadow-lg p-6" :class="`border-t-4 ${rol.borde}`">
        <div class="flex items-center gap-3 mb-4">
          <div class="w-12 h-12 rounded-lg flex items-center justify-center" :class="rol.bg">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" :class="rol.icono" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" /></svg>
          </div>
          <h3 class="text-lg font-semibold text-gray-900">{{ rol.nombre }}</h3>
        </div>
        <ul class="space-y-2">
          <li v-for="perm in rol.permisos" :key="perm" class="flex items-start gap-2 text-sm text-gray-600">
            <span :class="rol.check" class="mt-1">✓</span>
            <span>{{ perm }}</span>
          </li>
        </ul>
      </div>
    </div>

    <!-- Modal crear/editar -->
    <div v-if="mostrarModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full">
        <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
          <h2 class="text-xl font-bold">{{ editando ? 'Editar Usuario' : 'Nuevo Usuario' }}</h2>
          <button @click="cerrarModal" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <form @submit.prevent="guardarUsuario" class="p-6 space-y-4">
          <p v-if="formError" class="text-sm text-red-600 bg-red-50 p-3 rounded-lg">{{ formError }}</p>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
            <input v-model="form.nombre" type="text" required class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
          </div>
          <div v-if="!editando">
            <label class="block text-sm font-medium text-gray-700 mb-1">Correo (@ucr.ac.cr) *</label>
            <input v-model="form.correo" type="email" required class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Rol *</label>
            <select v-model="form.permisos" required class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none">
              <option value="Administradora">Administradora</option>
              <option value="GTI">GTI</option>
              <option value="JefaAdministrativa">Jefa Administrativa</option>
              <option value="Invitado">Invitado</option>
            </select>
          </div>
          <div v-if="editando">
            <label class="block text-sm font-medium text-gray-700 mb-1">Estado</label>
            <select v-model="form.activo" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none">
              <option :value="true">Activo</option>
              <option :value="false">Inactivo</option>
            </select>
          </div>
          <div v-if="contrasenaTemp" class="bg-gray-100 rounded-lg p-4">
            <p class="text-sm text-gray-600 mb-1">Contraseña temporal generada:</p>
            <p class="text-xl font-mono font-bold text-[#003d7a] tracking-wider">{{ contrasenaTemp }}</p>
            <p class="text-xs text-gray-500 mt-1">Comparte esta clave de forma segura con el usuario.</p>
          </div>
          <div class="flex gap-3 pt-2">
            <button type="button" @click="cerrarModal" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">Cancelar</button>
            <button type="submit" :disabled="formLoading" class="flex-1 px-4 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400">
              {{ formLoading ? 'Guardando...' : editando ? 'Guardar' : 'Crear' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import usuarioService from '@/services/usuarioService'

const usuarios = ref([])
const loading = ref(false)
const mostrarModal = ref(false)
const editando = ref(null)
const contrasenaTemp = ref('')
const formError = ref('')
const formLoading = ref(false)
const form = ref({ nombre: '', correo: '', permisos: 'Invitado', activo: true })

onMounted(cargar)

async function cargar() {
  loading.value = true
  try { const { data } = await usuarioService.listar(); usuarios.value = data }
  finally { loading.value = false }
}

function editarUsuario(u) {
  editando.value = u.correo
  form.value = { nombre: u.nombre, correo: u.correo, permisos: u.permisos, activo: u.activo }
  contrasenaTemp.value = ''
  formError.value = ''
  mostrarModal.value = true
}

function cerrarModal() {
  mostrarModal.value = false
  editando.value = null
  contrasenaTemp.value = ''
  formError.value = ''
  form.value = { nombre: '', correo: '', permisos: 'Invitado', activo: true }
}

async function guardarUsuario() {
  formError.value = ''
  formLoading.value = true
  try {
    if (editando.value) {
      await usuarioService.editar(editando.value, { nombre: form.value.nombre, permisos: form.value.permisos, activo: form.value.activo })
      cerrarModal()
    } else {
      const { data } = await usuarioService.crear({ nombre: form.value.nombre, correo: form.value.correo, permisos: form.value.permisos })
      contrasenaTemp.value = data.contrasenaTemp
    }
    await cargar()
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al guardar el usuario.'
  } finally {
    formLoading.value = false
  }
}

const rolClases = { Administradora: 'bg-purple-100 text-purple-800', GTI: 'bg-blue-100 text-blue-800', JefaAdministrativa: 'bg-green-100 text-green-800', Invitado: 'bg-gray-100 text-gray-800' }
function rolClase(r) { return rolClases[r] || 'bg-gray-100 text-gray-800' }

const rolesInfo = [
  { nombre: 'Administradora', borde: 'border-purple-500', bg: 'bg-purple-100', icono: 'text-purple-600', check: 'text-purple-500', permisos: ['Aprobar cambios de placa', 'Crear usuarios', 'Generar claves temporales', 'Aceptar/rechazar eliminaciones', 'Agregar categorías'] },
  { nombre: 'GTI', borde: 'border-blue-500', bg: 'bg-blue-100', icono: 'text-blue-600', check: 'text-blue-500', permisos: ['Crear activos', 'Editar ubicación', 'Editar responsable'] },
  { nombre: 'Jefa Administrativa', borde: 'border-green-500', bg: 'bg-green-100', icono: 'text-green-600', check: 'text-green-500', permisos: ['Solicitar cambios', 'Aprobar procesos administrativos'] },
  { nombre: 'Invitado', borde: 'border-gray-500', bg: 'bg-gray-100', icono: 'text-gray-600', check: 'text-gray-500', permisos: ['Solo lectura limitada'] }
]
</script>

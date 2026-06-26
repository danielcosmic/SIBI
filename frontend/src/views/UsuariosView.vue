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
    <div class="bg-white rounded-2xl shadow-lg overflow-hidden border border-blue-200">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-blue-100 border-b border-blue-200">
            <tr>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Usuario</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Rol</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Estado</th>
              <th class="px-6 py-3.5 text-left text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest">Acciones</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200">
            <tr v-if="loading"><td colspan="4" class="px-6 py-10 text-center text-gray-400">Cargando...</td></tr>
            <tr v-for="(u, idx) in usuarios" :key="u.correo"
              class="transition-colors duration-150"
              :class="idx % 2 === 0 ? 'bg-white hover:bg-blue-50' : 'bg-gray-50 hover:bg-blue-50'"
            >
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
                <span class="px-3 py-1 rounded-full text-xs font-medium" :class="rolClase(u.permisos)">{{ rolDisplay(u.permisos) }}</span>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <span class="px-3 py-1 rounded-full text-xs font-medium" :class="u.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'">
                    {{ u.activo ? 'Activo' : 'Inactivo' }}
                  </span>
                  <span v-if="u.intentosFallidos >= 3" class="px-3 py-1 rounded-full text-xs font-medium bg-orange-100 text-orange-800">
                    Bloqueado
                  </span>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-1">
                  <button
                    v-if="u.intentosFallidos >= 3"
                    @click="desbloquearUsuario(u)"
                    class="p-2 text-orange-500 hover:bg-orange-50 rounded transition"
                    title="Desbloquear cuenta"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 11V7a4 4 0 018 0m-4 8v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2z" /></svg>
                  </button>
                  <button
                    @click="abrirModalReset(u)"
                    class="p-2 text-amber-600 hover:bg-amber-50 rounded transition"
                    :class="(u.correo === authCorreo || u.correo === 'soporte.eic@ucr.ac.cr') ? 'opacity-30 cursor-not-allowed' : ''"
                    :disabled="u.correo === authCorreo || u.correo === 'soporte.eic@ucr.ac.cr'"
                    title="Restablecer contraseña"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" /></svg>
                  </button>
                  <button @click="editarUsuario(u)" class="p-2 text-green-600 hover:bg-green-50 rounded transition" title="Editar">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                  </button>
                  <button
                    @click="eliminarUsuario(u)"
                    class="p-2 text-red-500 hover:bg-red-50 rounded transition"
                    :class="(u.correo === authCorreo || u.correo === 'soporte.eic@ucr.ac.cr') ? 'opacity-30 cursor-not-allowed' : ''"
                    :disabled="u.correo === authCorreo || u.correo === 'soporte.eic@ucr.ac.cr'"
                    title="Eliminar"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                  </button>
                </div>
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
    <Teleport to="body">
    <div v-if="mostrarModal" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
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
            <select
              v-model="form.permisos"
              required
              :disabled="editando === 'soporte.eic@ucr.ac.cr'"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none disabled:bg-gray-100 disabled:text-gray-400 disabled:cursor-not-allowed"
            >
              <option value="Administradora">Administradora</option>
              <option value="GTI">GTI</option>
              <option value="JefaAdministrativa">Jefa Administrativa</option>
              <option value="Invitado">Invitado</option>
            </select>
            <p v-if="editando === 'soporte.eic@ucr.ac.cr'" class="mt-1 text-xs text-gray-400">El rol de la cuenta de soporte no puede modificarse.</p>
          </div>
          <div v-if="editando">
            <label class="block text-sm font-medium text-gray-700 mb-1">Estado</label>
            <select v-model="form.activo" class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] outline-none">
              <option :value="true">Activo</option>
              <option :value="false">Inactivo</option>
            </select>
          </div>
          <!-- Credenciales generadas -->
          <div v-if="contrasenaTemp" class="rounded-xl border border-[#c7dff7] overflow-hidden">
            <div class="bg-gradient-to-r from-[#003d7a] to-[#0055a8] px-4 py-2.5 flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-white/80" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
              </svg>
              <p class="text-white text-sm font-medium">Usuario creado exitosamente</p>
            </div>
            <div class="bg-[#f0f6ff] p-4 space-y-3">
              <div>
                <p class="text-xs text-gray-500 mb-0.5">Correo</p>
                <p class="text-sm font-medium text-gray-800">{{ form.correo }}</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 mb-0.5">Contraseña temporal</p>
                <p class="text-2xl font-mono font-bold text-[#003d7a] tracking-widest">{{ contrasenaTemp }}</p>
              </div>
              <p class="text-xs text-amber-700 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
                El usuario deberá cambiar esta contraseña en su primer ingreso al sistema.
              </p>
              <button
                type="button"
                @click="copiarCredenciales"
                class="w-full flex items-center justify-center gap-2 py-2 rounded-lg border transition text-sm font-medium"
                :class="copiado
                  ? 'bg-green-50 border-green-300 text-green-700'
                  : 'bg-white border-gray-200 text-gray-700 hover:bg-gray-50'"
              >
                <svg v-if="!copiado" xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                {{ copiado ? 'Credenciales copiadas' : 'Copiar credenciales' }}
              </button>
            </div>
          </div>
          <div class="flex gap-3 pt-2">
            <button type="button" @click="cerrarModal" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">{{ contrasenaTemp ? 'Cerrar' : 'Cancelar' }}</button>
            <button v-if="!contrasenaTemp" type="submit" :disabled="formLoading" class="flex-1 px-4 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition font-medium disabled:bg-gray-400">
              {{ formLoading ? 'Guardando...' : editando ? 'Guardar' : 'Crear' }}
            </button>
          </div>
        </form>
      </div>
    </div>
    </Teleport>

    <!-- Modal desbloqueo -->
    <Teleport to="body">
    <div v-if="mostrarModalDesbloqueo" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full">
        <div class="bg-orange-500 text-white px-6 py-4 flex items-center justify-between rounded-t-2xl">
          <div class="flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 11V7a4 4 0 018 0m-4 8v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2z" /></svg>
            <h2 class="text-xl font-bold">Cuenta Desbloqueada</h2>
          </div>
          <button @click="cerrarModalDesbloqueo" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <div class="p-6 space-y-4">
          <p class="text-sm text-gray-600">La cuenta fue desbloqueada y se generó una contraseña temporal. Entrégala al usuario para que pueda ingresar.</p>
          <div class="rounded-xl border border-[#c7dff7] overflow-hidden">
            <div class="bg-gradient-to-r from-[#003d7a] to-[#0055a8] px-4 py-2.5 flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-white/80" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" /></svg>
              <p class="text-white text-sm font-medium">Nuevas credenciales de acceso</p>
            </div>
            <div class="bg-[#f0f6ff] p-4 space-y-3">
              <div>
                <p class="text-xs text-gray-500 mb-0.5">Correo</p>
                <p class="text-sm font-medium text-gray-800">{{ desbloqueoData.correo }}</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 mb-0.5">Contraseña temporal</p>
                <p class="text-2xl font-mono font-bold text-[#003d7a] tracking-widest">{{ desbloqueoData.contrasenaTemp }}</p>
              </div>
              <p class="text-xs text-amber-700 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
                El usuario deberá cambiar esta contraseña en su primer ingreso al sistema.
              </p>
              <button
                type="button"
                @click="copiarDesbloqueo"
                class="w-full flex items-center justify-center gap-2 py-2 rounded-lg border transition text-sm font-medium"
                :class="copiadoDesbloqueo
                  ? 'bg-green-50 border-green-300 text-green-700'
                  : 'bg-white border-gray-200 text-gray-700 hover:bg-gray-50'"
              >
                <svg v-if="!copiadoDesbloqueo" xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" /></svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" /></svg>
                {{ copiadoDesbloqueo ? 'Credenciales copiadas' : 'Copiar credenciales' }}
              </button>
            </div>
          </div>
          <button @click="cerrarModalDesbloqueo"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
            Cerrar
          </button>
        </div>
      </div>
    </div>
    </Teleport>

    <!-- Modal reset contraseña -->
    <Teleport to="body">
    <div v-if="mostrarModalReset" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
      <div class="bg-white rounded-2xl shadow-2xl max-w-md w-full">
        <!-- Header -->
        <div
          class="text-white px-6 py-4 flex items-center justify-between rounded-t-2xl transition-colors duration-300"
          :class="resetFase === 2 ? 'bg-gradient-to-r from-[#003d7a] to-[#0055a8]' : 'bg-amber-600'"
        >
          <div class="flex items-center gap-2">
            <svg v-if="resetFase === 1" xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" /></svg>
            <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" /></svg>
            <h2 class="text-xl font-bold">{{ resetFase === 1 ? 'Restablecer contraseña' : 'Contraseña restablecida' }}</h2>
          </div>
          <button @click="cerrarModalReset" class="p-1 hover:bg-white/10 rounded">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>

        <!-- Fase 1: Confirmación -->
        <div v-if="resetFase === 1" class="p-6 space-y-5">
          <p class="text-sm text-gray-600">Está a punto de invalidar la contraseña actual y generar una nueva clave temporal para:</p>

          <!-- Tarjeta del usuario -->
          <div class="bg-gray-50 rounded-xl border border-gray-200 p-4 flex items-center gap-3">
            <div class="w-12 h-12 bg-[#003d7a] rounded-full flex items-center justify-center flex-shrink-0">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
            </div>
            <div>
              <p class="font-semibold text-gray-900 text-base">{{ resetTarget?.nombre }}</p>
              <p class="text-sm text-gray-500">{{ resetTarget?.correo }}</p>
              <span class="text-xs px-2 py-0.5 rounded-full mt-1 inline-block" :class="rolClase(resetTarget?.permisos)">{{ rolDisplay(resetTarget?.permisos) }}</span>
            </div>
          </div>

          <!-- Advertencia -->
          <div class="bg-amber-50 border border-amber-200 rounded-lg px-4 py-3 flex items-start gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-amber-600 mt-0.5 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
            <p class="text-xs text-amber-700">La contraseña actual quedará <strong>invalidada de inmediato</strong>. El usuario deberá usar la nueva clave temporal para ingresar y cambiarla en su próximo acceso.</p>
          </div>

          <!-- Input de confirmación -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">
              Escriba el correo del usuario para confirmar:
            </label>
            <input
              v-model="resetConfirmEmail"
              type="email"
              :placeholder="resetTarget?.correo"
              class="w-full px-4 py-2 border rounded-lg focus:ring-2 outline-none text-sm transition-colors"
              :class="resetConfirmEmail && resetConfirmEmail !== resetTarget?.correo
                ? 'border-red-300 bg-red-50 focus:ring-red-300'
                : 'border-gray-300 focus:ring-amber-400'"
              @keydown.enter.prevent="resetConfirmEmail === resetTarget?.correo && !resetLoading && confirmarReset()"
            />
            <p v-if="resetConfirmEmail && resetConfirmEmail !== resetTarget?.correo" class="text-xs text-red-600 mt-1">
              El correo no coincide.
            </p>
          </div>

          <!-- Botones -->
          <div class="flex gap-3">
            <button type="button" @click="cerrarModalReset" class="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium text-sm">Cancelar</button>
            <button
              type="button"
              @click="confirmarReset"
              :disabled="resetConfirmEmail !== resetTarget?.correo || resetLoading"
              class="flex-1 px-4 py-2 rounded-lg text-white font-medium text-sm transition"
              :class="resetConfirmEmail === resetTarget?.correo && !resetLoading
                ? 'bg-amber-600 hover:bg-amber-700'
                : 'bg-gray-300 cursor-not-allowed'"
            >
              {{ resetLoading ? 'Procesando...' : 'Confirmar reset' }}
            </button>
          </div>
        </div>

        <!-- Fase 2: Resultado -->
        <div v-else class="p-6 space-y-4">
          <p class="text-sm text-gray-600">La cuenta de <strong>{{ resetTarget?.nombre }}</strong> tiene una nueva contraseña temporal. Compártela de forma segura con el usuario.</p>
          <div class="rounded-xl border border-[#c7dff7] overflow-hidden">
            <div class="bg-gradient-to-r from-[#003d7a] to-[#0055a8] px-4 py-2.5 flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-white/80" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" /></svg>
              <p class="text-white text-sm font-medium">Nuevas credenciales de acceso</p>
            </div>
            <div class="bg-[#f0f6ff] p-4 space-y-3">
              <div>
                <p class="text-xs text-gray-500 mb-0.5">Correo</p>
                <p class="text-sm font-medium text-gray-800">{{ resetTarget?.correo }}</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 mb-0.5">Contraseña temporal</p>
                <p class="text-2xl font-mono font-bold text-[#003d7a] tracking-widest">{{ resetContrasenaTemp }}</p>
              </div>
              <p class="text-xs text-amber-700 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
                El usuario deberá cambiar esta contraseña en su primer ingreso al sistema.
              </p>
              <button
                type="button"
                @click="copiarReset"
                class="w-full flex items-center justify-center gap-2 py-2 rounded-lg border transition text-sm font-medium"
                :class="copiadoReset ? 'bg-green-50 border-green-300 text-green-700' : 'bg-white border-gray-200 text-gray-700 hover:bg-gray-50'"
              >
                <svg v-if="!copiadoReset" xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" /></svg>
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" /></svg>
                {{ copiadoReset ? 'Credenciales copiadas' : 'Copiar credenciales' }}
              </button>
            </div>
          </div>
          <button @click="cerrarModalReset" class="w-full px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium text-sm">
            Cerrar
          </button>
        </div>
      </div>
    </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import usuarioService from '@/services/usuarioService'
import { useAuthStore } from '@/stores/auth'
import { useDialog } from '@/composables/useDialog'

const auth = useAuthStore()
const dialog = useDialog()
const authCorreo = computed(() => auth.correo)

const usuarios = ref([])
const loading = ref(false)
const mostrarModal = ref(false)
const editando = ref(null)
const contrasenaTemp = ref('')
const copiado = ref(false)
const formError = ref('')
const formLoading = ref(false)
const form = ref({ nombre: '', correo: '', permisos: 'Invitado', activo: true })

const mostrarModalDesbloqueo = ref(false)
const desbloqueoData = ref({ correo: '', nombre: '', contrasenaTemp: '' })
const copiadoDesbloqueo = ref(false)

const mostrarModalReset = ref(false)
const resetTarget = ref(null)
const resetConfirmEmail = ref('')
const resetFase = ref(1)
const resetContrasenaTemp = ref('')
const resetLoading = ref(false)
const copiadoReset = ref(false)

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
  copiado.value = false
  formError.value = ''
  mostrarModal.value = true
}

async function cerrarModal() {
  if (contrasenaTemp.value) {
    const confirmado = await dialog.confirm({
      type: 'warning',
      title: '¿Ya guardó las credenciales?',
      message: 'Una vez que cierre este recuadro, la contraseña temporal <strong>no podrá verse nuevamente</strong>. Asegúrese de haberla anotado o copiado antes de continuar.',
      confirmText: 'Sí, cerrar',
      cancelText: 'Volver'
    })
    if (!confirmado) return
  }
  mostrarModal.value = false
  editando.value = null
  contrasenaTemp.value = ''
  copiado.value = false
  formError.value = ''
  form.value = { nombre: '', correo: '', permisos: 'Invitado', activo: true }
}

async function cerrarModalDesbloqueo() {
  const confirmado = await dialog.confirm({
    type: 'warning',
    title: '¿Ya guardó las credenciales?',
    message: 'Una vez que cierre este recuadro, la contraseña temporal <strong>no podrá verse nuevamente</strong>. Asegúrese de haberla anotado o copiado antes de continuar.',
    confirmText: 'Sí, cerrar',
    cancelText: 'Volver'
  })
  if (!confirmado) return
  mostrarModalDesbloqueo.value = false
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
      copiado.value = false
    }
    await cargar()
  } catch (e) {
    formError.value = e.response?.data?.mensaje || 'Error al guardar el usuario.'
  } finally {
    formLoading.value = false
  }
}

async function eliminarUsuario(u) {
  const confirmado = await dialog.confirm({
    type: 'danger',
    title: 'Eliminar usuario',
    message: `¿Estás seguro de que deseas eliminar a <strong>${u.nombre}</strong> (${u.correo})? Esta acción no se puede deshacer.`,
    confirmText: 'Eliminar',
    cancelText: 'Cancelar'
  })
  if (!confirmado) return
  try {
    await usuarioService.eliminar(u.correo)
    await cargar()
  } catch (e) {
    await dialog.alert({
      type: 'danger',
      title: 'No se pudo eliminar',
      message: e.response?.data?.mensaje || 'Ocurrió un error al eliminar el usuario.'
    })
  }
}

async function copiarCredenciales() {
  const texto = `Correo: ${form.value.correo}\nContraseña temporal: ${contrasenaTemp.value}\n\nAl ingresar por primera vez, el sistema le pedirá establecer una nueva contraseña.`
  await navigator.clipboard.writeText(texto)
  copiado.value = true
  setTimeout(() => { copiado.value = false }, 3000)
}

async function desbloquearUsuario(u) {
  try {
    const { data } = await usuarioService.desbloquear(u.correo)
    desbloqueoData.value = { correo: data.correo, nombre: u.nombre, contrasenaTemp: data.contrasenaTemp }
    copiadoDesbloqueo.value = false
    mostrarModalDesbloqueo.value = true
    await cargar()
  } catch (e) {
    await dialog.alert({
      type: 'danger',
      title: 'No se pudo desbloquear',
      message: e.response?.data?.mensaje || 'Ocurrió un error al desbloquear la cuenta.'
    })
  }
}

async function copiarDesbloqueo() {
  const texto = `Correo: ${desbloqueoData.value.correo}\nContraseña temporal: ${desbloqueoData.value.contrasenaTemp}\n\nAl ingresar por primera vez, el sistema le pedirá establecer una nueva contraseña.`
  await navigator.clipboard.writeText(texto)
  copiadoDesbloqueo.value = true
  setTimeout(() => { copiadoDesbloqueo.value = false }, 3000)
}

function abrirModalReset(u) {
  resetTarget.value = u
  resetConfirmEmail.value = ''
  resetFase.value = 1
  resetContrasenaTemp.value = ''
  resetLoading.value = false
  copiadoReset.value = false
  mostrarModalReset.value = true
}

async function cerrarModalReset() {
  if (resetFase.value === 2) {
    const confirmado = await dialog.confirm({
      type: 'warning',
      title: '¿Ya guardó las credenciales?',
      message: 'Una vez que cierre este recuadro, la contraseña temporal <strong>no podrá verse nuevamente</strong>. Asegúrese de haberla anotado o copiado antes de continuar.',
      confirmText: 'Sí, cerrar',
      cancelText: 'Volver'
    })
    if (!confirmado) return
  }
  mostrarModalReset.value = false
}

async function confirmarReset() {
  resetLoading.value = true
  try {
    const { data } = await usuarioService.resetContrasena(resetTarget.value.correo)
    resetContrasenaTemp.value = data.contrasenaTemp
    resetFase.value = 2
  } catch (e) {
    await dialog.alert({
      type: 'danger',
      title: 'No se pudo restablecer la contraseña',
      message: e.response?.data?.mensaje || 'Ocurrió un error al restablecer la contraseña.'
    })
  } finally {
    resetLoading.value = false
  }
}

async function copiarReset() {
  const texto = `Correo: ${resetTarget.value.correo}\nContraseña temporal: ${resetContrasenaTemp.value}\n\nAl ingresar por primera vez, el sistema le pedirá establecer una nueva contraseña.`
  await navigator.clipboard.writeText(texto)
  copiadoReset.value = true
  setTimeout(() => { copiadoReset.value = false }, 3000)
}

const rolClases = { Administradora: 'bg-purple-100 text-purple-800', GTI: 'bg-blue-100 text-blue-800', JefaAdministrativa: 'bg-green-100 text-green-800', Invitado: 'bg-gray-100 text-gray-800' }
function rolClase(r) { return rolClases[r] || 'bg-gray-100 text-gray-800' }
function rolDisplay(r) { return r === 'JefaAdministrativa' ? 'Jefa Administrativa' : r }

const rolesInfo = [
  { nombre: 'Administradora', borde: 'border-purple-500', bg: 'bg-purple-100', icono: 'text-purple-600', check: 'text-purple-500', permisos: ['Ver, crear y editar activos del inventario', 'Aprobar cambios de placa', 'Crear usuarios y generar claves temporales', 'Aceptar/rechazar solicitudes de eliminación', 'Agregar y gestionar categorías'] },
  { nombre: 'GTI', borde: 'border-blue-500', bg: 'bg-blue-100', icono: 'text-blue-600', check: 'text-blue-500', permisos: ['Ver, crear y editar activos del inventario', 'Editar ubicación y responsable', 'Registrar activos para desecho'] },
  { nombre: 'Jefa Administrativa', borde: 'border-green-500', bg: 'bg-green-100', icono: 'text-green-600', check: 'text-green-500', permisos: ['Ver el inventario', 'Solicitar cambios en elementos del inventario', 'Aprobar procesos administrativos'] },
  { nombre: 'Invitado', borde: 'border-gray-500', bg: 'bg-gray-100', icono: 'text-gray-600', check: 'text-gray-500', permisos: ['Solo lectura del inventario'] }
]
</script>

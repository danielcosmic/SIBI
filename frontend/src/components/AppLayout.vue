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
          <component :is="item.icon" class="w-5 h-5 flex-shrink-0" />
          <span class="flex-1">{{ item.label }}</span>
          <span v-if="item.badge" class="bg-red-500 text-white text-xs font-bold rounded-full w-5 h-5 flex items-center justify-center flex-shrink-0">
            {{ item.badge > 9 ? '9+' : item.badge }}
          </span>
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
                name="search"
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
          <div class="flex items-center gap-2 ml-2">
            <!-- Notificaciones de solicitudes -->
            <div v-if="auth.esGTI" class="relative" ref="notifBtn">
              <button
                @click="toggleNotif"
                class="relative p-2 rounded-xl hover:bg-gray-100 transition text-gray-500 hover:text-[#003d7a]"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                </svg>
                <span v-if="notifCount > 0"
                  class="absolute -top-0.5 -right-0.5 bg-red-500 text-white text-xs font-bold rounded-full min-w-[18px] h-[18px] flex items-center justify-center px-1 leading-none">
                  {{ notifCount > 9 ? '9+' : notifCount }}
                </span>
              </button>
            </div>
            <div class="pl-2 border-l border-gray-300">
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
                  <p class="text-xs text-gray-500">{{ rolDisplay(auth.permisos) }}</p>
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
                {{ rolDisplay(auth.permisos) }}
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

          <!-- Cambiar contraseña -->
          <div class="px-4 pb-2">
            <button
              @click="abrirCambioContrasena"
              class="w-full flex items-center justify-center gap-2 px-4 py-2.5 bg-blue-50 hover:bg-blue-100 text-[#003d7a] rounded-xl transition font-medium text-sm"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
              </svg>
              Cambiar Contraseña
            </button>
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

        <!-- Panel de notificaciones -->
      <Transition
        enter-active-class="transition ease-out duration-150"
        enter-from-class="opacity-0 scale-95"
        enter-to-class="opacity-100 scale-100"
        leave-active-class="transition ease-in duration-100"
        leave-from-class="opacity-100 scale-100"
        leave-to-class="opacity-0 scale-95"
      >
        <div
          v-if="notifAbierto"
          class="fixed w-84 bg-white rounded-2xl shadow-2xl border border-gray-100 overflow-hidden"
          :style="notifDropdownStyle"
          style="z-index: 9999; width: 340px"
        >
          <!-- Cabecera -->
          <div class="bg-gradient-to-r from-[#003d7a] to-[#0066cc] px-5 py-4 flex items-center justify-between">
            <div class="flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
              </svg>
              <h4 class="text-white font-semibold text-sm">Notificaciones</h4>
            </div>
          </div>

          <div class="max-h-96 overflow-y-auto">

            <!-- Sección: Notificaciones en tiempo real -->
            <div v-if="notifStore.items.length > 0">
              <div class="px-4 py-1.5 text-xs font-semibold text-gray-500 uppercase tracking-wide bg-gray-50 border-b border-gray-100">
                Recientes
              </div>
              <div
                v-for="(n, idx) in notifStore.items.slice(0, 8)"
                :key="idx"
                class="px-4 py-3 border-b border-gray-50 hover:bg-gray-50/60 transition-colors"
              >
                <div class="flex items-start gap-3">
                  <!-- Ícono por tipo -->
                  <div class="mt-0.5 w-7 h-7 rounded-full flex items-center justify-center shrink-0"
                    :class="n.tipo === 'cuenta_bloqueada' ? 'bg-red-100' : 'bg-amber-100'">
                    <!-- solicitud_cambio -->
                    <svg v-if="n.tipo === 'solicitud_cambio'" xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-amber-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" /></svg>
                    <!-- cuenta_bloqueada -->
                    <svg v-else-if="n.tipo === 'cuenta_bloqueada'" xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" /></svg>
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-gray-800">{{ n.titulo }}</p>
                    <p class="text-xs text-gray-500 mt-0.5 leading-snug">{{ n.mensaje }}</p>
                    <p class="text-xs text-gray-400 mt-1">{{ formatNotifFecha(n.fecha) }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Sección: Solicitudes pendientes -->
            <div v-if="auth.esGTI">
              <div class="px-4 py-1.5 text-xs font-semibold text-gray-500 uppercase tracking-wide bg-gray-50 border-b border-gray-100 flex items-center justify-between">
                <span>Solicitudes pendientes</span>
                <span v-if="solicitudesPendientes > 0" class="bg-amber-100 text-amber-700 text-xs font-bold px-1.5 py-0.5 rounded-full">{{ solicitudesPendientes }}</span>
              </div>
              <div v-if="notifCargando" class="py-6 text-center text-sm text-gray-400">Cargando...</div>
              <div v-else-if="notifItems.length === 0" class="py-6 text-center">
                <p class="text-sm text-gray-400">No hay solicitudes pendientes</p>
              </div>
              <div
                v-for="item in notifItems"
                :key="item.id"
                @click="irASolicitudes"
                class="px-4 py-3 hover:bg-blue-50/50 cursor-pointer transition-colors border-b border-gray-50 last:border-0"
              >
                <div class="flex items-start gap-3">
                  <div class="w-2 h-2 rounded-full bg-amber-400 mt-1.5 flex-shrink-0"></div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-gray-800">
                      <span class="font-mono text-[#003d7a]">{{ item.activoPlaca }}</span>
                      <span class="text-gray-500 font-normal"> · {{ item.articuloActual }}</span>
                    </p>
                    <p class="text-xs text-gray-500 mt-0.5">Solicitado por {{ item.solicitanteNombre }}</p>
                    <p class="text-xs text-gray-400 mt-0.5">{{ formatNotifFecha(item.fechaSolicitud) }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Estado vacío total -->
            <div v-if="notifStore.items.length === 0 && notifItems.length === 0 && !notifCargando"
              class="py-10 text-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-10 h-10 text-gray-200 mx-auto mb-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
              </svg>
              <p class="text-sm text-gray-400">No hay notificaciones</p>
            </div>
          </div>

          <!-- Pie -->
          <div v-if="auth.esGTI" class="border-t border-gray-100 px-4 py-3 bg-gray-50/60">
            <button @click="irASolicitudes"
              class="w-full text-sm text-[#0066cc] hover:text-[#003d7a] font-medium transition text-center">
              Ver todas las solicitudes →
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

      <div
        v-if="notifAbierto"
        class="fixed inset-0"
        style="z-index: 9998"
        @click="notifAbierto = false"
      />

      <!-- Modal cambio de contraseña -->
      <Transition
        enter-active-class="transition ease-out duration-150"
        enter-from-class="opacity-0 scale-95"
        enter-to-class="opacity-100 scale-100"
        leave-active-class="transition ease-in duration-100"
        leave-from-class="opacity-100 scale-100"
        leave-to-class="opacity-0 scale-95"
      >
        <div v-if="cambioContrasenaAbierto" class="fixed inset-0 flex items-center justify-center" style="z-index: 10000">
          <div class="fixed inset-0 bg-black/40" @click="cerrarCambioContrasena" />
          <div class="relative bg-white rounded-2xl shadow-2xl border border-gray-100 w-full max-w-md mx-4 overflow-hidden" style="z-index: 10001">
            <!-- Cabecera -->
            <div class="bg-gradient-to-br from-[#003d7a] to-[#0066cc] px-6 py-4 flex items-center gap-3">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-white flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
              </svg>
              <h3 class="text-white font-semibold text-lg">Cambiar Contraseña</h3>
            </div>

            <!-- Cuerpo -->
            <div class="p-6 space-y-4">
              <div>
                <label class="text-xs text-gray-500 uppercase tracking-wide font-medium">Contraseña actual</label>
                <input
                  v-model="contrasenaActual"
                  type="password"
                  placeholder="Ingresa tu contraseña actual"
                  autocomplete="off"
                  readonly
                  @focus="$event.target.removeAttribute('readonly')"
                  class="mt-1 w-full px-3 py-2.5 border border-gray-200 rounded-xl text-sm outline-none focus:ring-2 focus:ring-[#0066cc]/20 focus:border-[#0066cc]/50 transition"
                  @keyup.enter="guardarContrasena"
                />
              </div>
              <div>
                <label class="text-xs text-gray-500 uppercase tracking-wide font-medium">Nueva contraseña</label>
                <input
                  v-model="nuevaContrasena"
                  type="password"
                  placeholder="Mínimo 6 caracteres, mayúscula, minúscula y número"
                  autocomplete="new-password"
                  readonly
                  @focus="$event.target.removeAttribute('readonly')"
                  class="mt-1 w-full px-3 py-2.5 border border-gray-200 rounded-xl text-sm outline-none focus:ring-2 focus:ring-[#0066cc]/20 focus:border-[#0066cc]/50 transition"
                  @keyup.enter="guardarContrasena"
                />
                <div v-if="nuevaContrasena.length > 0" class="mt-2 space-y-2">
                  <div class="h-1.5 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      class="h-full rounded-full transition-all duration-300"
                      :class="requisitosNueva.barColor"
                      :style="{ width: (requisitosNueva.cumplidos / 4 * 100) + '%' }"
                    />
                  </div>
                  <div class="grid grid-cols-2 gap-x-4 gap-y-1">
                    <div v-for="r in requisitosNueva.lista" :key="r.texto" class="flex items-center gap-1.5">
                      <svg v-if="r.ok" xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5 text-green-500 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
                      </svg>
                      <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5 text-gray-300 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                      </svg>
                      <span class="text-xs" :class="r.ok ? 'text-green-600' : 'text-gray-400'">{{ r.texto }}</span>
                    </div>
                  </div>
                </div>
              </div>
              <div>
                <label class="text-xs text-gray-500 uppercase tracking-wide font-medium">Confirmar contraseña</label>
                <input
                  v-model="confirmarContrasena"
                  type="password"
                  placeholder="Repite la nueva contraseña"
                  autocomplete="new-password"
                  readonly
                  @focus="$event.target.removeAttribute('readonly')"
                  class="mt-1 w-full px-3 py-2.5 border border-gray-200 rounded-xl text-sm outline-none focus:ring-2 focus:ring-[#0066cc]/20 focus:border-[#0066cc]/50 transition"
                  @keyup.enter="guardarContrasena"
                />
              </div>
              <p v-if="contrasenaError" class="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">{{ contrasenaError }}</p>
              <p v-if="contrasenOk" class="text-sm text-green-700 bg-green-50 px-3 py-2 rounded-lg">Contraseña actualizada correctamente.</p>
            </div>

            <!-- Pie -->
            <div class="px-6 pb-6 flex gap-3">
              <button
                @click="cerrarCambioContrasena"
                class="flex-1 px-4 py-2.5 border border-gray-200 text-gray-600 rounded-xl hover:bg-gray-50 transition font-medium text-sm"
              >
                Cancelar
              </button>
              <button
                @click="guardarContrasena"
                :disabled="guardandoContrasena"
                class="flex-1 px-4 py-2.5 bg-[#003d7a] text-white rounded-xl hover:bg-[#0066cc] transition font-medium text-sm disabled:opacity-50 flex items-center justify-center gap-2"
              >
                <svg v-if="guardandoContrasena" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
                </svg>
                Guardar
              </button>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>

    <AppDialog />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useNotificacionesStore } from '@/stores/notificaciones'
import AppDialog from '@/components/AppDialog.vue'
import sibiLogo from '@/assets/SIBI_logo_4096_fondo_oscuro.png'
import activoService from '@/services/activoService'
import solicitudService from '@/services/solicitudService'
import authService from '@/services/authService'

const auth = useAuthStore()
const notifStore = useNotificacionesStore()
const router = useRouter()
const route = useRoute()
const perfilAbierto = ref(false)
const solicitudesPendientes = ref(0)
const notifAbierto = ref(false)
const notifBtn = ref(null)
const notifDropdownStyle = ref({})
const notifItems = ref([])
const notifCargando = ref(false)

const notifCount = computed(() => notifStore.noLeidas + solicitudesPendientes.value)
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

async function cargarPendientes() {
  try {
    if (auth.esGTI) {
      const { data } = await solicitudService.contarPendientes()
      solicitudesPendientes.value = data.count
    }
  } catch { /* silent */ }
}

async function toggleNotif() {
  if (notifAbierto.value) { notifAbierto.value = false; return }
  const rect = notifBtn.value.getBoundingClientRect()
  notifDropdownStyle.value = {
    top: `${rect.bottom + 8}px`,
    right: `${window.innerWidth - rect.right}px`
  }
  notifAbierto.value = true
  notifStore.marcarLeidas()
  notifCargando.value = true
  notifItems.value = []
  try {
    if (auth.esGTI) {
      const { data } = await solicitudService.listar('Pendiente')
      notifItems.value = data.slice(0, 5)
    }
  } catch { /* silent */ }
  finally { notifCargando.value = false }
}

function irASolicitudes() {
  notifAbierto.value = false
  router.push(auth.esGTI ? '/solicitudes' : '/mis-solicitudes')
}

function formatNotifFecha(iso) {
  if (!iso) return ''
  const d = new Date(iso)
  const ahora = new Date()
  const mins = Math.floor((ahora - d) / 60000)
  if (mins < 60) return `Hace ${mins || 1} min`
  const hrs = Math.floor(mins / 60)
  if (hrs < 24) return `Hace ${hrs}h`
  return d.toLocaleDateString('es-CR', { day: '2-digit', month: 'short' })
}

onMounted(() => {
  document.addEventListener('mousedown', onClickFuera)
  cargarPendientes()
  if (auth.esGTI) notifStore.conectar(auth.token)
})
onBeforeUnmount(() => {
  document.removeEventListener('mousedown', onClickFuera)
  notifStore.desconectar()
})
watch(() => route.path, cargarPendientes)

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
  if (auth.esGTI) {
    items.push({
      to: '/solicitudes',
      label: 'Solicitudes',
      icon: IconClipboard,
      badge: solicitudesPendientes.value || null
    })
  }
  if (auth.esJefaAdministrativa) {
    items.push({ to: '/mis-solicitudes', label: 'Mis Solicitudes', icon: IconClipboard })
  }
  return items
})

const rolClases = {
  Administradora: 'bg-purple-100 text-purple-800',
  GTI: 'bg-blue-100 text-blue-800',
  JefaAdministrativa: 'bg-green-100 text-green-800',
  Invitado: 'bg-gray-100 text-gray-800'
}
function rolClase(r) { return rolClases[r] || 'bg-gray-100 text-gray-800' }
function rolDisplay(r) { return r === 'JefaAdministrativa' ? 'Jefa Administrativa' : r }

async function cerrarSesion() {
  await notifStore.desconectar()
  auth.logout()
  router.push('/')
}

// ── Cambio de contraseña ──────────────────────────────────────────────────────
const cambioContrasenaAbierto = ref(false)
const contrasenaActual = ref('')
const nuevaContrasena = ref('')
const confirmarContrasena = ref('')
const contrasenaError = ref('')
const contrasenOk = ref(false)
const guardandoContrasena = ref(false)

const requisitosNueva = computed(() => {
  const p = nuevaContrasena.value
  const lista = [
    { texto: 'Mínimo 6 caracteres', ok: p.length >= 6 },
    { texto: 'Una mayúscula (A-Z)',  ok: /[A-Z]/.test(p) },
    { texto: 'Una minúscula (a-z)',  ok: /[a-z]/.test(p) },
    { texto: 'Un número (0-9)',      ok: /[0-9]/.test(p) }
  ]
  const cumplidos = lista.filter(r => r.ok).length
  const barColor = cumplidos <= 1 ? 'bg-red-500' : cumplidos <= 2 ? 'bg-yellow-500' : cumplidos === 3 ? 'bg-blue-500' : 'bg-green-500'
  return { lista, cumplidos, barColor }
})

function abrirCambioContrasena() {
  perfilAbierto.value = false
  contrasenaActual.value = ''
  nuevaContrasena.value = ''
  confirmarContrasena.value = ''
  contrasenaError.value = ''
  contrasenOk.value = false
  cambioContrasenaAbierto.value = true
}

function cerrarCambioContrasena() {
  cambioContrasenaAbierto.value = false
}

async function guardarContrasena() {
  contrasenaError.value = ''
  contrasenOk.value = false
  if (!contrasenaActual.value) {
    contrasenaError.value = 'Ingresa tu contraseña actual.'
    return
  }
  if (!nuevaContrasena.value) {
    contrasenaError.value = 'Ingresa la nueva contraseña.'
    return
  }
  if (nuevaContrasena.value !== confirmarContrasena.value) {
    contrasenaError.value = 'Las contraseñas no coinciden.'
    return
  }
  guardandoContrasena.value = true
  try {
    await authService.cambiarContrasena(contrasenaActual.value, nuevaContrasena.value)
    contrasenOk.value = true
    contrasenaActual.value = ''
    nuevaContrasena.value = ''
    confirmarContrasena.value = ''
    setTimeout(() => { cambioContrasenaAbierto.value = false }, 1500)
  } catch (e) {
    contrasenaError.value = e.response?.data?.mensaje || 'Error al cambiar la contraseña.'
  } finally {
    guardandoContrasena.value = false
  }
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
const IconClipboard = {
  template: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" /></svg>`
}
</script>

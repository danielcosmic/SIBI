<template>
  <Teleport to="body">
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[90vh] flex flex-col">

      <!-- Header -->
      <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl flex-shrink-0">
        <div>
          <h2 class="text-xl font-bold">
            {{ mode === 'create' ? 'Nuevo Activo' : mode === 'edit' ? 'Editar Activo' : mode === 'solicitud' ? 'Proponer Cambios' : 'Detalle del Activo' }}
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
        <div class="overflow-y-auto flex-1 p-4 space-y-3">

          <!-- Banner de solicitud pendiente (solo JefaAdministrativa) -->
          <div v-if="solicitudPendiente" class="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 space-y-2">
            <div class="flex items-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-amber-600 flex-shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <p class="text-sm font-semibold text-amber-800">Cambio en espera de aprobación</p>
              <span class="ml-auto text-xs text-amber-500">{{ formatFechaSolicitud(solicitudPendiente.fechaSolicitud) }}</span>
            </div>
            <ul class="space-y-1 pl-6">
              <template v-for="campo in camposCambiados" :key="campo.label">
                <li class="text-xs text-amber-700">
                  <span class="font-medium">{{ campo.label }}:</span>
                  <span class="line-through text-amber-400 mx-1">{{ campo.actual }}</span>
                  <span class="italic font-medium">→ {{ campo.propuesto }}</span>
                </li>
              </template>
            </ul>
          </div>

          <!-- Identificación -->
          <div class="rounded-xl border border-gray-200 overflow-hidden">
            <p class="text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest bg-blue-100 border-b border-blue-200 px-4 py-2">Identificación</p>
            <div class="grid grid-cols-2 gap-3 p-4">
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
          <div class="rounded-xl border border-gray-200 overflow-hidden">
            <p class="text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest bg-blue-100 border-b border-blue-200 px-4 py-2">Clasificación</p>
            <div class="grid grid-cols-2 gap-3 p-4">
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
              <div v-if="props.activo?.fechaDesecho && !auth.esInvitado" class="bg-red-50 rounded-lg p-3 col-span-2">
                <p class="text-xs text-red-400 mb-0.5">Fecha de Desecho</p>
                <p class="font-medium text-red-700">{{ props.activo?.fechaDesecho }}</p>
              </div>
            </div>
          </div>

          <!-- Ubicación -->
          <div class="rounded-xl border border-gray-200 overflow-hidden">
            <p class="text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest bg-blue-100 border-b border-blue-200 px-4 py-2">Ubicación</p>
            <div class="grid gap-3 p-4" :class="auth.esInvitado ? 'grid-cols-1' : 'grid-cols-2'">
              <div class="bg-blue-50 rounded-lg p-3 border border-blue-200">
                <p class="text-xs text-blue-500 mb-0.5">Ubicación Actual</p>
                <p class="font-semibold text-blue-900">{{ props.activo?.ubicacionActual }}</p>
              </div>
              <div v-if="!auth.esInvitado" class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Ubicación Anterior</p>
                <p class="font-medium text-gray-600">{{ props.activo?.ubicacionAnterior || '—' }}</p>
              </div>
            </div>
          </div>

          <!-- Encargados -->
          <div class="rounded-xl border border-gray-200 overflow-hidden">
            <p class="text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest bg-blue-100 border-b border-blue-200 px-4 py-2">Encargados</p>
            <div class="grid gap-3 p-4" :class="auth.esInvitado ? 'grid-cols-1' : 'grid-cols-2'">
              <div class="bg-blue-50 rounded-lg p-3 border border-blue-200">
                <p class="text-xs text-blue-500 mb-0.5">Encargado Actual</p>
                <p class="font-semibold text-blue-900">{{ props.activo?.encargadoActual }}</p>
              </div>
              <div v-if="!auth.esInvitado" class="bg-gray-50 rounded-lg p-3">
                <p class="text-xs text-gray-400 mb-0.5">Encargado Anterior</p>
                <p class="font-medium text-gray-600">{{ props.activo?.encargadoAnterior || '—' }}</p>
              </div>
            </div>
          </div>

          <!-- Observaciones -->
          <div v-if="props.activo?.observaciones" class="rounded-xl border border-gray-200 overflow-hidden">
            <p class="text-[11px] font-semibold text-[#003d7a] uppercase tracking-widest bg-blue-100 border-b border-blue-200 px-4 py-2">Observaciones</p>
            <div class="p-4 bg-gray-50">
              <p class="text-sm text-gray-700">{{ props.activo?.observaciones }}</p>
            </div>
          </div>
        </div>

        <!-- Footer view -->
        <div class="flex gap-3 px-6 py-4 border-t border-gray-200 flex-shrink-0">
          <button type="button" @click="$emit('close')"
            class="px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
            Cerrar
          </button>
          <button v-if="!auth.esInvitado" type="button" @click="router.push('/activo/' + props.activo?.placa); $emit('close')"
            class="flex items-center gap-1.5 px-4 py-2.5 text-[#0066cc] border border-[#0066cc]/30 rounded-lg hover:bg-blue-50 transition font-medium text-sm">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
            </svg>
            Ver Detalle
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
            v-if="auth.esJefaAdministrativa && props.activo?.estado !== 'Desecho' && !solicitudPendiente"
            type="button"
            @click="$emit('solicitar')"
            class="flex-1 px-4 py-2.5 bg-amber-500 text-white rounded-lg hover:bg-amber-600 transition font-medium flex items-center justify-center gap-2"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" />
            </svg>
            Solicitar Cambio
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

      <!-- ── MODO CREATE / EDIT / SOLICITUD: formulario ── -->
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
                  <div v-if="showNuevaCategoria" class="mt-2 bg-white border border-blue-200 rounded-lg p-3 space-y-2">
                    <div class="flex gap-2">
                      <input v-model="nuevaCategoria.nombre" type="text" placeholder="Nombre de la categoría"
                        class="flex-1 px-2 py-1.5 text-sm border border-gray-200 rounded-lg outline-none focus:ring-1 focus:ring-[#003d7a]"
                        @keydown.enter.prevent="nuevaCategoria.nombre.trim() && !creandoCategoria && crearCategoria()" />
                      <input v-model="nuevaCategoria.emoji" type="text" maxlength="2" placeholder="🗂️"
                        class="w-12 text-center px-2 py-1.5 text-sm border border-gray-200 rounded-lg outline-none focus:ring-1 focus:ring-[#003d7a]" />
                    </div>
                    <p v-if="errorCategoria" class="text-xs text-red-600">{{ errorCategoria }}</p>
                    <div class="flex gap-2">
                      <button type="button" @click="crearCategoria"
                        :disabled="!nuevaCategoria.nombre.trim() || creandoCategoria"
                        class="flex-1 py-1.5 text-xs bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition disabled:bg-gray-300">
                        {{ creandoCategoria ? 'Creando...' : 'Crear categoría' }}
                      </button>
                      <button type="button" @click="showNuevaCategoria = false; errorCategoria = ''"
                        class="px-3 py-1.5 text-xs border border-gray-200 rounded-lg hover:bg-gray-100 transition text-gray-600">
                        Cancelar
                      </button>
                    </div>
                  </div>
                  <button v-else-if="auth.esAdministradora" type="button" @click="showNuevaCategoria = true"
                    class="mt-1.5 flex items-center gap-1 text-xs text-[#0066cc] hover:text-[#003d7a] transition">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                    </svg>
                    Nueva categoría
                  </button>
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
                  <div v-if="showNuevoEncargado" class="mt-2 bg-white border border-blue-200 rounded-lg p-3 space-y-2">
                    <div class="flex flex-col gap-2">
                      <input v-model="nuevoEncargado.nombre" type="text" placeholder="Nombre del encargado"
                        class="w-full px-2 py-1.5 text-sm border border-gray-200 rounded-lg outline-none focus:ring-1 focus:ring-[#003d7a]" />
                      <input v-model="nuevoEncargado.rol" type="text" placeholder="Cargo o rol"
                        class="w-full px-2 py-1.5 text-sm border border-gray-200 rounded-lg outline-none focus:ring-1 focus:ring-[#003d7a]"
                        @keydown.enter.prevent="nuevoEncargado.nombre.trim() && nuevoEncargado.rol.trim() && !creandoEncargado && crearEncargado()" />
                    </div>
                    <p v-if="errorEncargado" class="text-xs text-red-600">{{ errorEncargado }}</p>
                    <div class="flex gap-2">
                      <button type="button" @click="crearEncargado"
                        :disabled="!nuevoEncargado.nombre.trim() || !nuevoEncargado.rol.trim() || creandoEncargado"
                        class="flex-1 py-1.5 text-xs bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition disabled:bg-gray-300">
                        {{ creandoEncargado ? 'Creando...' : 'Crear encargado' }}
                      </button>
                      <button type="button" @click="showNuevoEncargado = false; errorEncargado = ''"
                        class="px-3 py-1.5 text-xs border border-gray-200 rounded-lg hover:bg-gray-100 transition text-gray-600">
                        Cancelar
                      </button>
                    </div>
                  </div>
                  <button v-else-if="auth.esGTI || auth.esAdministradora" type="button" @click="showNuevoEncargado = true"
                    class="mt-1.5 flex items-center gap-1 text-xs text-[#0066cc] hover:text-[#003d7a] transition">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                    </svg>
                    Nuevo encargado
                  </button>
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

        <!-- Aviso modo solicitud -->
        <div v-if="mode === 'solicitud'" class="mx-6 mb-2 px-3 py-2 bg-amber-50 border border-amber-200 rounded-lg text-xs text-amber-700">
          Los cambios propuestos serán enviados para revisión y no se aplicarán hasta ser aprobados por Administradora o GTI.
        </div>

        <!-- Footer create/edit/solicitud -->
        <div class="flex gap-3 px-6 py-4 border-t border-gray-100 flex-shrink-0">
          <button type="button" @click="mode === 'edit' || mode === 'solicitud' ? $emit('cancelEdit') : $emit('close')"
            class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-100 transition font-medium">
            Cancelar
          </button>
          <button @click="handleSubmit" :disabled="loading"
            :class="mode === 'solicitud'
              ? 'bg-amber-500 hover:bg-amber-600'
              : 'bg-[#003d7a] hover:bg-[#002d5a]'"
            class="flex-1 px-4 py-2.5 text-white rounded-lg transition font-medium disabled:bg-gray-400 flex items-center justify-center gap-2">
            <svg v-if="loading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8H4z"/>
            </svg>
            {{ loading ? (mode === 'solicitud' ? 'Enviando...' : 'Guardando...') : mode === 'create' ? 'Crear Activo' : mode === 'solicitud' ? 'Enviar Solicitud' : 'Guardar Cambios' }}
          </button>
        </div>
      </template>

    </div>
  </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { useRouter } from 'vue-router'
import activoService from '@/services/activoService'
import solicitudService from '@/services/solicitudService'
import categoriaService from '@/services/categoriaService'
import encargadoService from '@/services/encargadoService'
import SearchableSelect from '@/components/SearchableSelect.vue'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const auth = useAuthStore()
const solicitudPendiente = ref(null)

async function cargarSolicitudPendiente(placa) {
  if (!auth.esJefaAdministrativa || !placa) { solicitudPendiente.value = null; return }
  try {
    const res = await solicitudService.obtenerPendienteDeActivo(placa)
    solicitudPendiente.value = res.status === 204 ? null : res.data
  } catch {
    solicitudPendiente.value = null
  }
}

function formatFechaSolicitud(iso) {
  if (!iso) return ''
  return new Date(iso).toLocaleDateString('es-CR', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' })
}

const props = defineProps({
  mode: { type: String, required: true }, // 'create' | 'edit' | 'view' | 'solicitud'
  activo: { type: Object, default: null },
  categorias: { type: Array, default: () => [] },
  encargados: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'saved', 'edit', 'enviarDesecho', 'cancelEdit', 'solicitar', 'entityCreated'])

// Entities created inline during this modal session
const categoriasLocales = ref([])
const encargadosLocales = ref([])

const categoriasOptions = computed(() =>
  [...props.categorias, ...categoriasLocales.value]
    .map(c => ({ value: c.id, label: `${c.icono ?? ''} ${c.nombre}`.trim() }))
)
const encargadosOptions = computed(() =>
  [...props.encargados, ...encargadosLocales.value]
    .map(e => ({ value: e.id, label: `${e.nombre} — ${e.rol}` }))
)

// Inline category creation
const showNuevaCategoria = ref(false)
const nuevaCategoria = ref({ nombre: '', emoji: '' })
const creandoCategoria = ref(false)
const errorCategoria = ref('')

// Inline encargado creation
const showNuevoEncargado = ref(false)
const nuevoEncargado = ref({ nombre: '', rol: '' })
const creandoEncargado = ref(false)
const errorEncargado = ref('')

async function crearCategoria() {
  errorCategoria.value = ''
  creandoCategoria.value = true
  try {
    const { data } = await categoriaService.crear({
      nombre: nuevaCategoria.value.nombre.trim(),
      icono: nuevaCategoria.value.emoji.trim() || null
    })
    categoriasLocales.value.push(data)
    form.value.categoriaId = data.id
    showNuevaCategoria.value = false
    nuevaCategoria.value = { nombre: '', emoji: '' }
    emit('entityCreated')
  } catch (e) {
    errorCategoria.value = e.response?.data?.mensaje || 'No se pudo crear la categoría.'
  } finally {
    creandoCategoria.value = false
  }
}

async function crearEncargado() {
  errorEncargado.value = ''
  creandoEncargado.value = true
  try {
    const { data } = await encargadoService.crear({
      nombre: nuevoEncargado.value.nombre.trim(),
      rol: nuevoEncargado.value.rol.trim()
    })
    encargadosLocales.value.push(data)
    form.value.encargadoId = data.id
    showNuevoEncargado.value = false
    nuevoEncargado.value = { nombre: '', rol: '' }
    emit('entityCreated')
  } catch (e) {
    errorEncargado.value = e.response?.data?.mensaje || 'No se pudo crear el encargado.'
  } finally {
    creandoEncargado.value = false
  }
}

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
    cargarSolicitudPendiente(activo.placa)
  } else {
    solicitudPendiente.value = null
  }
}, { immediate: true })

const camposCambiados = computed(() => {
  if (!solicitudPendiente.value || !props.activo) return []
  const d = solicitudPendiente.value.datosNuevos
  const a = props.activo
  const cambios = []
  if (d.articulo !== a.articulo)           cambios.push({ label: 'Artículo',   actual: a.articulo,         propuesto: d.articulo })
  if (d.marca !== a.marca)                 cambios.push({ label: 'Marca',      actual: a.marca,            propuesto: d.marca })
  if (d.modelo !== a.modelo)               cambios.push({ label: 'Modelo',     actual: a.modelo,           propuesto: d.modelo })
  if (d.numSerial !== a.numSerial)         cambios.push({ label: 'N° Serial',  actual: a.numSerial,        propuesto: d.numSerial })
  if (d.categoriaNombre !== a.categoriaNombre) cambios.push({ label: 'Categoría', actual: a.categoriaNombre, propuesto: d.categoriaNombre })
  if (d.ubicacionActual !== a.ubicacionActual) cambios.push({ label: 'Ubicación', actual: a.ubicacionActual,  propuesto: d.ubicacionActual })
  if (d.encargadoNombre !== a.encargadoActual) cambios.push({ label: 'Encargado', actual: a.encargadoActual,  propuesto: d.encargadoNombre })
  if (d.estado !== a.estado)               cambios.push({ label: 'Estado',     actual: a.estado,           propuesto: d.estado })
  return cambios
})

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    if (props.mode === 'solicitud') {
      await solicitudService.crear({
        activoPlaca: form.value.placa,
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
    } else if (props.mode === 'create') {
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
    error.value = e.response?.data?.mensaje || (props.mode === 'solicitud' ? 'Error al enviar la solicitud.' : 'Error al guardar el activo.')
  } finally {
    loading.value = false
  }
}
</script>

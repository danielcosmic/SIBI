<template>
  <Teleport to="body">
  <div class="fixed inset-0 bg-black/50 flex items-center justify-center p-4" style="z-index: 9999" @click.self="$emit('close')">
    <div class="bg-white rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] flex flex-col">

      <!-- Header -->
      <div class="bg-[#003d7a] text-white px-6 py-4 flex items-center justify-between rounded-t-2xl shrink-0">
        <div class="flex items-center gap-3">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
          </svg>
          <h2 class="text-xl font-bold">Importar Activos desde Excel o CSV</h2>
        </div>
        <button @click="$emit('close')" class="p-1 hover:bg-white/10 rounded">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Step indicators -->
      <div class="flex border-b border-gray-100 shrink-0">
        <div v-for="(step, idx) in stepsDisplay" :key="idx"
          class="flex-1 py-3 text-sm font-medium text-center transition-colors"
          :class="step.isCurrent
            ? 'text-[#003d7a] border-b-2 border-[#003d7a] bg-blue-50/40'
            : step.isDone ? 'text-green-600' : 'text-gray-400'"
        >
          {{ step.label }}
        </div>
      </div>

      <!-- Scrollable content -->
      <div class="flex-1 overflow-y-auto p-6">

        <!-- PASO 1: Instructions + upload -->
        <div v-if="paso === 1" class="space-y-5">
          <div class="bg-blue-50 border border-blue-200 rounded-xl p-4 space-y-2">
            <p class="text-sm font-semibold text-[#003d7a]">Instrucciones</p>
            <ol class="text-sm text-gray-700 space-y-1 list-decimal list-inside">
              <li>Descarga la plantilla de Excel.</li>
              <li>Completa los datos de cada activo (una fila por activo).</li>
              <li>El campo <strong>Tipo Placa</strong> debe ser <code class="bg-blue-100 px-1 rounded">Institucional</code> o <code class="bg-blue-100 px-1 rounded">Interno</code> / <code class="bg-blue-100 px-1 rounded">Interna</code>.</li>
              <li>No modifiques los encabezados de la plantilla.</li>
            </ol>
            <p class="text-xs text-blue-700 bg-blue-100 rounded-lg px-3 py-2 mt-2">
              Si una categoría o encargado no existe en el sistema, podrás agregarlos durante la importación sin necesidad de crearlos manualmente antes.
            </p>
          </div>

          <!-- Categorías y encargados disponibles -->
          <div class="grid grid-cols-2 gap-4">
            <div class="border border-gray-200 rounded-xl overflow-hidden">
              <div class="bg-gray-50 px-4 py-2 text-xs font-semibold text-gray-600 uppercase tracking-wide">Categorías disponibles</div>
              <ul class="divide-y divide-gray-100 max-h-36 overflow-y-auto">
                <li v-for="c in categorias" :key="c.id" class="px-4 py-1.5 text-sm text-gray-700">{{ c.nombre }}</li>
              </ul>
            </div>
            <div class="border border-gray-200 rounded-xl overflow-hidden">
              <div class="bg-gray-50 px-4 py-2 text-xs font-semibold text-gray-600 uppercase tracking-wide">Encargados disponibles</div>
              <ul class="divide-y divide-gray-100 max-h-36 overflow-y-auto">
                <li v-for="e in encargados" :key="e.id" class="px-4 py-1.5 text-sm text-gray-700">{{ e.nombre }}</li>
              </ul>
            </div>
          </div>

          <!-- Download template -->
          <button @click="descargarPlantilla"
            class="flex items-center gap-2 px-4 py-2.5 border border-green-600 text-green-700 rounded-lg hover:bg-green-50 transition text-sm font-medium">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
            </svg>
            Descargar plantilla (.xlsx)
          </button>

          <!-- Upload zone -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Seleccionar archivo</label>
            <div
              class="border-2 border-dashed rounded-xl p-8 text-center transition-all"
              :class="archivoError ? 'border-red-300 bg-red-50' : archivoNombre ? 'border-green-400 bg-green-50' : 'border-gray-300 hover:border-[#003d7a] hover:bg-blue-50/30'"
              @dragover.prevent @drop.prevent="onDrop"
            >
              <template v-if="archivoNombre">
                <div class="w-12 h-12 mx-auto mb-3 bg-green-100 rounded-full flex items-center justify-center">
                  <svg xmlns="http://www.w3.org/2000/svg" class="w-7 h-7 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <p class="text-sm font-semibold text-green-700 mb-0.5">Archivo cargado</p>
                <p class="text-sm text-green-600 mb-3 truncate max-w-xs mx-auto">{{ archivoNombre }}</p>
              </template>
              <template v-else>
                <svg xmlns="http://www.w3.org/2000/svg" class="w-10 h-10 mx-auto mb-3 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                </svg>
                <p class="text-sm text-gray-600 mb-1">Arrastra tu archivo aquí o haz clic para seleccionar</p>
                <p class="text-xs text-gray-400 mb-3">.xlsx, .xls o .csv</p>
              </template>
              <input ref="fileInput" type="file" accept=".xlsx,.xls,.csv" class="hidden" @change="onFileChange" />
              <button type="button" @click="fileInput.click()"
                class="px-4 py-1.5 text-sm rounded-lg transition"
                :class="archivoNombre ? 'border border-green-500 text-green-700 hover:bg-green-100' : 'bg-[#003d7a] text-white hover:bg-[#002d5a]'">
                {{ archivoNombre ? 'Cambiar archivo' : 'Seleccionar archivo' }}
              </button>
            </div>
            <p v-if="archivoError" class="mt-1.5 text-sm text-red-600">{{ archivoError }}</p>
          </div>
        </div>

        <!-- PASO 2: Preview table -->
        <div v-else-if="paso === 2" class="space-y-4">
          <div class="flex items-center justify-between flex-wrap gap-2">
            <p class="text-sm text-gray-600">
              <span class="font-semibold text-gray-900">{{ filas.length }}</span> filas ·
              <span class="text-red-600 font-medium">{{ filasConError.length }} con errores</span> ·
              <span class="text-green-700 font-medium">{{ filasValidas.length }} válidas</span>
              <span v-if="filasConEntidadesNuevas.length" class="text-amber-600 font-medium"> · {{ filasConEntidadesNuevas.length }} con entidades nuevas</span>
              <span v-if="encargadosAmbiguos.length" class="text-orange-600 font-medium"> · encargados duplicados en sistema</span>
            </p>
            <span v-if="filasConError.length" class="text-xs bg-red-50 text-red-700 border border-red-200 px-2 py-1 rounded-full">
              Corrige los errores en el Excel y vuelve a subir el archivo
            </span>
            <span v-else-if="tieneEntidadesNuevas" class="text-xs bg-amber-50 text-amber-700 border border-amber-200 px-2 py-1 rounded-full">
              Se solicitará aprobación de entidades nuevas en el paso siguiente
            </span>
          </div>

          <div class="border border-gray-200 rounded-xl overflow-hidden">
            <div class="overflow-x-auto max-h-96">
              <table class="w-full text-sm">
                <thead class="bg-gray-50 sticky top-0">
                  <tr>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">#</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Placa</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Artículo</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Marca / Modelo</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Categoría</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Encargado</th>
                    <th class="px-3 py-2 text-left text-xs font-semibold text-gray-600 uppercase">Estado</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-gray-100">
                  <tr v-for="fila in filas" :key="fila._idx" :class="fila._errores.length ? 'bg-red-50' : 'hover:bg-gray-50'">
                    <td class="px-3 py-2 text-gray-400 text-xs">{{ fila._idx }}</td>
                    <td class="px-3 py-2 font-mono font-medium text-gray-900">{{ fila.placa || '—' }}</td>
                    <td class="px-3 py-2 text-gray-700">{{ fila.articulo || '—' }}</td>
                    <td class="px-3 py-2 text-gray-700">{{ fila.marca }} {{ fila.modelo }}</td>
                    <td class="px-3 py-2">
                      <span class="text-gray-700">{{ fila.categoriaNombre || '—' }}</span>
                      <span v-if="fila.categoriaNombre && !categoriaValida(fila.categoriaNombre)"
                        class="ml-1.5 text-xs bg-amber-100 text-amber-700 border border-amber-200 px-1.5 py-0.5 rounded-full font-medium">Nuevo</span>
                    </td>
                    <td class="px-3 py-2">
                      <span class="text-gray-700">{{ fila.encargadoNombre || '—' }}</span>
                      <span v-if="fila.encargadoNombre && !encargadoValido(fila.encargadoNombre)"
                        class="ml-1.5 text-xs bg-amber-100 text-amber-700 border border-amber-200 px-1.5 py-0.5 rounded-full font-medium">Nuevo</span>
                      <span v-else-if="fila.encargadoNombre && encargadoEsAmbiguo(fila.encargadoNombre)"
                        class="ml-1.5 text-xs bg-orange-100 text-orange-700 border border-orange-200 px-1.5 py-0.5 rounded-full font-medium">Duplicado</span>
                    </td>
                    <td class="px-3 py-2">
                      <span v-if="fila._errores.length" class="text-red-600 text-xs">{{ fila._errores.join(' · ') }}</span>
                      <span v-else class="text-green-700 text-xs font-medium">OK</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- PASO 3: Validate new entities + resolve duplicates -->
        <div v-else-if="paso === 3" class="space-y-5">
          <div class="bg-amber-50 border border-amber-200 rounded-xl p-4 flex items-start gap-3">
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-amber-600 mt-0.5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
            <div>
              <p class="text-sm font-semibold text-amber-800">Revisión de entidades</p>
              <p class="text-sm text-amber-700 mt-0.5">Aprueba las nuevas y resuelve encargados con nombre duplicado en el sistema.</p>
            </div>
          </div>

          <!-- Categories similar to DB entries -->
          <div v-if="categoriasSimilaresADB.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-yellow-100 text-yellow-700 rounded flex items-center justify-center text-xs">~</span>
              Categorías similares a existentes ({{ categoriasSimilaresADB.length }})
            </h4>
            <p class="text-xs text-gray-500">El sistema detectó que estas categorías del archivo podrían ser la misma que una ya registrada. Indica si usar la existente o crear una nueva.</p>
            <div v-for="sim in categoriasSimilaresADB" :key="sim.nombreExcel"
              class="p-3 rounded-lg border border-yellow-200 bg-yellow-50 space-y-2.5">
              <div class="flex items-start justify-between gap-4 flex-wrap">
                <div>
                  <span class="text-[10px] text-gray-400 uppercase tracking-wide block">En el archivo</span>
                  <span class="font-mono font-medium text-gray-800">"{{ sim.nombreExcel }}"</span>
                </div>
                <div class="text-right">
                  <span class="text-[10px] text-gray-400 uppercase tracking-wide block">Ya existe en sistema</span>
                  <span class="font-medium text-[#003d7a]">"{{ sim.categoriaDB.nombre }}"</span>
                </div>
              </div>
              <div class="flex flex-wrap gap-4">
                <label class="flex items-center gap-2 cursor-pointer text-sm">
                  <input type="radio" :name="'catSim_' + sim.nombreExcel" value="db" v-model="sim.decision" class="accent-[#003d7a]" />
                  <span :class="sim.decision === 'db' ? 'text-[#003d7a] font-medium' : 'text-gray-600'">
                    Usar "{{ sim.categoriaDB.nombre }}" (existente)
                  </span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer text-sm">
                  <input type="radio" :name="'catSim_' + sim.nombreExcel" value="nueva" v-model="sim.decision" class="accent-[#003d7a]" />
                  <span :class="sim.decision === 'nueva' ? 'text-gray-900 font-medium' : 'text-gray-600'">
                    Crear "{{ sim.nombreExcel }}" como nueva
                  </span>
                </label>
              </div>
              <div v-if="sim.decision === 'nueva'" class="flex items-center gap-2 pl-1">
                <label class="text-xs text-gray-500 shrink-0">Ícono:</label>
                <input v-model="sim.emoji" type="text" maxlength="2" placeholder="🗂️"
                  class="w-12 text-center border border-gray-300 rounded px-1 py-0.5 text-lg outline-none focus:ring-1 focus:ring-[#003d7a]" />
              </div>
            </div>
          </div>

          <!-- Category groups (similar among themselves, all new to DB) -->
          <div v-if="gruposCategoriasSimilares.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-yellow-100 text-yellow-700 rounded flex items-center justify-center text-xs">~</span>
              Posibles duplicados de categorías en el archivo ({{ gruposCategoriasSimilares.length }})
            </h4>
            <p class="text-xs text-gray-500">Estas categorías del archivo parecen ser la misma con diferente escritura. Elige el nombre que se usará para todas.</p>
            <div v-for="grupo in gruposCategoriasSimilares" :key="grupo.nombres[0]"
              class="p-3 rounded-lg border border-yellow-200 bg-yellow-50 space-y-2">
              <span class="text-xs text-gray-500">{{ filasDependientesGrupoCat(grupo).length }} activo(s) afectados</span>
              <div class="flex flex-col gap-1.5 pl-1">
                <label v-for="nombre in grupo.nombres" :key="nombre"
                  class="flex items-center gap-2 cursor-pointer text-sm"
                  :class="grupo.nombreElegido === nombre ? 'text-gray-900 font-medium' : 'text-gray-600'">
                  <input type="radio" :name="'grupoCat_' + grupo.nombres[0]" :value="nombre" v-model="grupo.nombreElegido" class="accent-[#003d7a]" />
                  {{ nombre }}
                </label>
              </div>
              <div class="flex items-center gap-2 pt-0.5">
                <label class="text-xs text-gray-500 shrink-0">Ícono:</label>
                <input v-model="grupo.emoji" type="text" maxlength="2" placeholder="🗂️"
                  class="w-12 text-center border border-gray-300 rounded px-1 py-0.5 text-lg outline-none focus:ring-1 focus:ring-[#003d7a]" />
              </div>
            </div>
          </div>

          <!-- New categories -->
          <div v-if="categoriasNuevas.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-purple-100 text-purple-700 rounded flex items-center justify-center text-xs font-bold">C</span>
              Categorías nuevas ({{ categoriasNuevas.length }})
            </h4>
            <div v-for="cat in categoriasNuevas" :key="cat.nombre"
              class="flex items-center gap-3 p-3 rounded-lg border transition-colors"
              :class="cat.aprobada ? 'bg-green-50 border-green-200' : 'bg-gray-50 border-gray-200'">
              <input type="checkbox" v-model="cat.aprobada" class="w-4 h-4 cursor-pointer accent-[#003d7a]" />
              <span class="font-medium text-gray-800 flex-1">{{ cat.nombre }}</span>
              <template v-if="cat.aprobada">
                <label class="text-xs text-gray-500 shrink-0">Ícono:</label>
                <input v-model="cat.emoji" type="text" maxlength="2" placeholder="🗂️"
                  class="w-12 text-center border border-gray-300 rounded px-1 py-0.5 text-lg outline-none focus:ring-1 focus:ring-[#003d7a]" />
              </template>
              <span v-else class="text-xs text-gray-400 italic">
                {{ filasDependientes('cat', cat.nombre).length }} activo(s) no se importarán
              </span>
            </div>
          </div>

          <!-- Encargados similar to DB entries -->
          <div v-if="encargadosSimilaresADB.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-yellow-100 text-yellow-700 rounded flex items-center justify-center text-xs">~</span>
              Encargados similares a existentes ({{ encargadosSimilaresADB.length }})
            </h4>
            <p class="text-xs text-gray-500">El sistema detectó que estos encargados del archivo podrían ser la misma persona que una ya registrada.</p>
            <div v-for="sim in encargadosSimilaresADB" :key="sim.nombreExcel"
              class="p-3 rounded-lg border border-yellow-200 bg-yellow-50 space-y-2.5">
              <div class="flex items-start justify-between gap-4 flex-wrap">
                <div>
                  <span class="text-[10px] text-gray-400 uppercase tracking-wide block">En el archivo</span>
                  <span class="font-medium text-gray-800">"{{ sim.nombreExcel }}"</span>
                </div>
                <div class="text-right">
                  <span class="text-[10px] text-gray-400 uppercase tracking-wide block">Ya existe en sistema</span>
                  <span class="font-medium text-[#003d7a]">"{{ sim.encargadoDB.nombre }}"</span>
                  <span class="text-xs text-gray-400 block">{{ sim.encargadoDB.rol || 'Sin rol' }}</span>
                </div>
              </div>
              <div class="flex flex-wrap gap-4">
                <label class="flex items-center gap-2 cursor-pointer text-sm">
                  <input type="radio" :name="'encSim_' + sim.nombreExcel" value="db" v-model="sim.decision" class="accent-[#003d7a]" />
                  <span :class="sim.decision === 'db' ? 'text-[#003d7a] font-medium' : 'text-gray-600'">
                    Usar "{{ sim.encargadoDB.nombre }}" (existente)
                  </span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer text-sm">
                  <input type="radio" :name="'encSim_' + sim.nombreExcel" value="nueva" v-model="sim.decision" class="accent-[#003d7a]" />
                  <span :class="sim.decision === 'nueva' ? 'text-gray-900 font-medium' : 'text-gray-600'">
                    Crear "{{ sim.nombreExcel }}" como nuevo
                  </span>
                </label>
              </div>
              <div v-if="sim.decision === 'nueva'" class="pl-1">
                <input v-model="sim.rol" type="text" placeholder="Cargo o rol (ej: Docente, Administrativo...)"
                  class="w-full border rounded px-3 py-1.5 text-sm outline-none focus:ring-1 transition-colors"
                  :class="!sim.rol?.trim() ? 'border-red-300 bg-red-50 focus:ring-red-400' : 'border-gray-300 focus:ring-[#003d7a]'" />
                <span v-if="!sim.rol?.trim()" class="text-xs text-red-500">Requerido</span>
              </div>
            </div>
          </div>

          <!-- Encargado groups (similar among themselves, all new to DB) -->
          <div v-if="gruposEncargadosSimilares.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-yellow-100 text-yellow-700 rounded flex items-center justify-center text-xs">~</span>
              Posibles duplicados de encargados en el archivo ({{ gruposEncargadosSimilares.length }})
            </h4>
            <p class="text-xs text-gray-500">Estos encargados del archivo parecen ser la misma persona con diferente escritura. Elige el nombre que se usará para todos.</p>
            <div v-for="grupo in gruposEncargadosSimilares" :key="grupo.nombres[0]"
              class="p-3 rounded-lg border border-yellow-200 bg-yellow-50 space-y-2">
              <span class="text-xs text-gray-500">{{ filasDependientesGrupoEnc(grupo).length }} activo(s) afectados</span>
              <div class="flex flex-col gap-1.5 pl-1">
                <label v-for="nombre in grupo.nombres" :key="nombre"
                  class="flex items-center gap-2 cursor-pointer text-sm"
                  :class="grupo.nombreElegido === nombre ? 'text-gray-900 font-medium' : 'text-gray-600'">
                  <input type="radio" :name="'grupoEnc_' + grupo.nombres[0]" :value="nombre" v-model="grupo.nombreElegido" class="accent-[#003d7a]" />
                  {{ nombre }}
                </label>
              </div>
              <input v-model="grupo.rol" type="text" placeholder="Cargo o rol (ej: Docente, Administrativo...)"
                class="w-full border rounded px-3 py-1.5 text-sm outline-none focus:ring-1 transition-colors"
                :class="!grupo.rol?.trim() ? 'border-red-300 bg-red-50 focus:ring-red-400' : 'border-gray-300 focus:ring-[#003d7a]'" />
              <span v-if="!grupo.rol?.trim()" class="text-xs text-red-500">El rol es requerido</span>
            </div>
          </div>

          <!-- New encargados -->
          <div v-if="encargadosNuevos.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-blue-100 text-blue-700 rounded flex items-center justify-center text-xs font-bold">E</span>
              Encargados nuevos ({{ encargadosNuevos.length }})
            </h4>
            <div v-for="enc in encargadosNuevos" :key="enc.nombre"
              class="flex items-center gap-3 p-3 rounded-lg border transition-colors"
              :class="enc.aprobado ? 'bg-green-50 border-green-200' : 'bg-gray-50 border-gray-200'">
              <input type="checkbox" v-model="enc.aprobado" class="w-4 h-4 cursor-pointer accent-[#003d7a]" />
              <span class="font-medium text-gray-800 w-44 shrink-0">{{ enc.nombre }}</span>
              <template v-if="enc.aprobado">
                <input v-model="enc.rol" type="text" placeholder="Cargo o rol (ej: Docente, Administrativo...)"
                  class="flex-1 border rounded px-3 py-1.5 text-sm outline-none focus:ring-1 transition-colors"
                  :class="!enc.rol.trim() ? 'border-red-300 bg-red-50 focus:ring-red-400' : 'border-gray-300 focus:ring-[#003d7a]'" />
                <span v-if="!enc.rol.trim()" class="text-xs text-red-500 shrink-0">Requerido</span>
              </template>
              <span v-else class="text-xs text-gray-400 italic">
                {{ filasDependientes('enc', enc.nombre).length }} activo(s) no se importarán
              </span>
            </div>
          </div>

          <!-- Ambiguous encargados -->
          <div v-if="encargadosAmbiguos.length" class="space-y-2">
            <h4 class="text-sm font-semibold text-gray-700 flex items-center gap-2">
              <span class="w-5 h-5 bg-orange-100 text-orange-700 rounded flex items-center justify-center text-xs font-bold">!</span>
              Encargados con nombre duplicado en el sistema ({{ encargadosAmbiguos.length }})
            </h4>
            <p class="text-xs text-gray-500">Hay varios encargados registrados con el mismo nombre. Selecciona cuál usar para cada caso.</p>
            <div v-for="amb in encargadosAmbiguos" :key="amb.nombre"
              class="p-3 rounded-lg border border-orange-200 bg-orange-50 space-y-2">
              <div class="flex items-center justify-between">
                <span class="font-medium text-gray-800">{{ amb.nombre }}</span>
                <span class="text-xs text-orange-600">{{ filasDependientes('enc', amb.nombre).length }} activo(s) afectados</span>
              </div>
              <div class="flex flex-col gap-1.5 pl-1">
                <label v-for="op in amb.opciones" :key="op.id"
                  class="flex items-center gap-2 cursor-pointer text-sm"
                  :class="amb.seleccionadoId === op.id ? 'text-gray-900 font-medium' : 'text-gray-600'">
                  <input type="radio" :name="'amb_' + amb.nombre" :value="op.id" v-model="amb.seleccionadoId"
                    class="accent-[#003d7a]" />
                  <span>{{ op.nombre }}</span>
                  <span class="text-gray-400 text-xs">— {{ op.rol || 'Sin rol' }}</span>
                </label>
              </div>
              <p v-if="!amb.seleccionadoId" class="text-xs text-orange-700 font-medium">
                ⚠ Selecciona uno para poder importar estos activos
              </p>
            </div>
          </div>

          <!-- Summary -->
          <div class="bg-blue-50 border border-blue-200 rounded-xl p-4">
            <p class="text-sm font-semibold text-[#003d7a] mb-1">Resumen</p>
            <p class="text-sm text-gray-700">
              <span class="font-semibold text-green-700">{{ filasAImportar.length }} activo(s)</span> se importarán
              <template v-if="filasValidas.length - filasAImportar.length > 0">
                · <span class="font-semibold text-amber-600">{{ filasValidas.length - filasAImportar.length }}</span> omitidos por entidades rechazadas o sin resolver
              </template>
              <template v-if="filasConError.length">
                · <span class="font-semibold text-red-600">{{ filasConError.length }}</span> con errores de datos (ignorados)
              </template>
            </p>
          </div>
        </div>

        <!-- PASO 4: Results -->
        <div v-else-if="paso === 4" class="space-y-4">
          <div :class="resultadosExistentes.length ? 'grid grid-cols-3 gap-4' : 'grid grid-cols-2 gap-4'">
            <div class="bg-green-50 border border-green-200 rounded-xl p-4 text-center">
              <p class="text-3xl font-bold text-green-700">{{ resultadosExitosos.length }}</p>
              <p class="text-sm text-green-600 mt-1">Activos creados</p>
            </div>
            <div v-if="resultadosExistentes.length" class="bg-gray-50 border border-gray-200 rounded-xl p-4 text-center">
              <p class="text-3xl font-bold text-gray-500">{{ resultadosExistentes.length }}</p>
              <p class="text-sm text-gray-400 mt-1">Ya existían</p>
            </div>
            <div class="bg-red-50 border border-red-200 rounded-xl p-4 text-center">
              <p class="text-3xl font-bold text-red-600">{{ resultadosError.length }}</p>
              <p class="text-sm text-red-500 mt-1">Con error</p>
            </div>
          </div>

          <!-- Placas que ya existían — neutral -->
          <div v-if="resultadosExistentes.length" class="border border-gray-200 rounded-xl overflow-hidden">
            <div class="bg-gray-50 px-4 py-2 text-xs font-semibold text-gray-500 uppercase tracking-wide">Placas ya registradas (omitidas)</div>
            <ul class="divide-y divide-gray-100 max-h-40 overflow-y-auto">
              <li v-for="r in resultadosExistentes" :key="r.fila" class="px-4 py-2 flex items-center gap-3 text-sm">
                <span class="font-mono font-medium text-gray-600">{{ r.placa }}</span>
              </li>
            </ul>
          </div>

          <!-- Errores reales -->
          <div v-if="resultadosError.length" class="border border-red-200 rounded-xl overflow-hidden">
            <div class="bg-red-50 px-4 py-2 text-xs font-semibold text-red-700 uppercase tracking-wide">Errores</div>
            <ul class="divide-y divide-red-100 max-h-64 overflow-y-auto">
              <li v-for="r in resultadosError" :key="r.fila" class="px-4 py-2.5 flex items-start gap-3 text-sm">
                <div class="shrink-0 min-w-[6rem]">
                  <span class="text-[10px] text-gray-400 uppercase tracking-wide block">Placa</span>
                  <span class="font-mono font-semibold text-gray-800">{{ r.placa }}</span>
                </div>
                <span class="text-red-600 flex-1">{{ r.error }}</span>
              </li>
            </ul>
          </div>

          <p v-if="resultadosExitosos.length > 0" class="text-sm text-gray-600 bg-blue-50 border border-blue-100 rounded-lg px-4 py-3">
            El inventario fue actualizado. Los activos creados ya aparecen en la tabla de inventario.
          </p>
        </div>

      </div>

      <!-- Footer -->
      <div class="border-t border-gray-100 px-6 py-4 flex justify-between items-center shrink-0">
        <button v-if="paso === 2 || paso === 3" @click="paso--"
          class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition text-sm font-medium">
          Atrás
        </button>
        <span v-else></span>

        <div class="flex gap-3">
          <button @click="$emit('close')"
            class="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 transition text-sm font-medium">
            {{ paso === 4 ? 'Cerrar' : 'Cancelar' }}
          </button>

          <!-- Paso 1 -->
          <button v-if="paso === 1" @click="parsearArchivo" :disabled="!archivoNombre"
            class="px-5 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition text-sm font-medium disabled:bg-gray-300">
            Previsualizar
          </button>

          <!-- Paso 2: ir a validación si hay entidades nuevas o ambiguas -->
          <button v-else-if="paso === 2 && tieneEntidadesNuevas" @click="irAPaso3" :disabled="filasValidas.length === 0"
            class="px-5 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition text-sm font-medium disabled:bg-gray-300">
            Siguiente →
          </button>

          <!-- Paso 2: importar directo si no hay entidades nuevas ni ambiguas -->
          <button v-else-if="paso === 2 && !tieneEntidadesNuevas" @click="confirmarImportacion"
            :disabled="filasValidas.length === 0 || importando"
            class="px-5 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition text-sm font-medium disabled:bg-gray-300">
            {{ importando ? 'Importando...' : `Importar ${filasValidas.length} activo${filasValidas.length !== 1 ? 's' : ''}` }}
          </button>

          <!-- Paso 3: confirmar e importar -->
          <button v-else-if="paso === 3" @click="confirmarImportacion"
            :disabled="importando || encargadosConRolFaltante.length > 0 || encargadosAmbiguosSinResolver.length > 0 || filasAImportar.length === 0"
            class="px-5 py-2 bg-[#003d7a] text-white rounded-lg hover:bg-[#002d5a] transition text-sm font-medium disabled:bg-gray-300">
            {{ importando ? 'Importando...' : `Confirmar e importar (${filasAImportar.length})` }}
          </button>
        </div>
      </div>

    </div>
  </div>
  </Teleport>
</template>

<script setup>
import { ref, computed } from 'vue'
import activoService from '@/services/activoService'
import categoriaService from '@/services/categoriaService'
import encargadoService from '@/services/encargadoService'
import { useDialog } from '@/composables/useDialog'

const props = defineProps({
  categorias: { type: Array, default: () => [] },
  encargados: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'importado'])
const dialog = useDialog()

const paso = ref(1)
const fueValidacion = ref(false)
const fileInput = ref(null)
const archivoNombre = ref('')
const archivoError = ref('')
const filas = ref([])
const resultados = ref([])
const importando = ref(false)

// Truly new entities (no similarity to anything)
const categoriasNuevas = ref([])   // [{ nombre, emoji, aprobada }]
const encargadosNuevos = ref([])   // [{ nombre, rol, aprobado }]
// Exact-name duplicates in DB (multiple DB records for same name)
const encargadosAmbiguos = ref([]) // [{ nombre, opciones:[{id,nombre,rol}], seleccionadoId }]
// Similar to an existing DB entry (typo/accent difference)
const categoriasSimilaresADB = ref([])    // [{ nombreExcel, categoriaDB:{id,nombre}, decision:'db'|'nueva', emoji:'' }]
const encargadosSimilaresADB = ref([])    // [{ nombreExcel, encargadoDB:{id,nombre,rol}, decision:'db'|'nueva', rol:'' }]
// Groups of similar names among themselves (all new to DB)
const gruposCategoriasSimilares = ref([]) // [{ nombres:[], nombreElegido, emoji:'' }]
const gruposEncargadosSimilares = ref([]) // [{ nombres:[], nombreElegido, rol:'' }]

// ── Similarity helpers ─────────────────────────────────────────────────────
function normalizarTexto(s) {
  return s.normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase().trim()
}

function levenshtein(a, b) {
  const m = a.length, n = b.length
  const dp = Array.from({ length: m + 1 }, (_, i) =>
    Array.from({ length: n + 1 }, (_, j) => i === 0 ? j : j === 0 ? i : 0))
  for (let i = 1; i <= m; i++)
    for (let j = 1; j <= n; j++)
      dp[i][j] = a[i - 1] === b[j - 1]
        ? dp[i - 1][j - 1]
        : 1 + Math.min(dp[i - 1][j], dp[i][j - 1], dp[i - 1][j - 1])
  return dp[m][n]
}

const UMBRAL = 0.78

function esSimilar(a, b) {
  const na = normalizarTexto(a), nb = normalizarTexto(b)
  if (na === nb) return false
  const maxLen = Math.max(na.length, nb.length)
  return maxLen > 0 && (1 - levenshtein(na, nb) / maxLen) >= UMBRAL
}

// ── Canonical name resolvers (apply user decisions to Excel values) ─────────
function nombreCanonicoCategoria(nombre) {
  const nl = nombre.toLowerCase()
  const simDB = categoriasSimilaresADB.value.find(s => s.nombreExcel.toLowerCase() === nl)
  if (simDB?.decision === 'db') return simDB.categoriaDB.nombre
  const grupo = gruposCategoriasSimilares.value.find(g => g.nombres.some(n => n.toLowerCase() === nl))
  if (grupo) return grupo.nombreElegido
  return nombre
}

function nombreCanonicoEncargado(nombre) {
  const nl = nombre.toLowerCase()
  const simDB = encargadosSimilaresADB.value.find(s => s.nombreExcel.toLowerCase() === nl)
  if (simDB?.decision === 'db') return simDB.encargadoDB.nombre
  const grupo = gruposEncargadosSimilares.value.find(g => g.nombres.some(n => n.toLowerCase() === nl))
  if (grupo) return grupo.nombreElegido
  return nombre
}
// ──────────────────────────────────────────────────────────────────────────

const COLUMNAS = ['Placa', 'Tipo Placa', 'Artículo', 'Marca', 'Modelo', 'N° Serial', 'Categoría', 'Ubicación Actual', 'Encargado', 'Observaciones']

const categoriasPorNombre = computed(() =>
  new Set(props.categorias.map(c => c.nombre.toLowerCase()))
)
const encargadosPorNombre = computed(() =>
  new Set(props.encargados.map(e => e.nombre.toLowerCase()))
)

const encargadosConteo = computed(() => {
  const m = {}
  props.encargados.forEach(e => {
    const k = e.nombre.toLowerCase()
    m[k] = (m[k] || 0) + 1
  })
  return m
})

function categoriaValida(nombre) {
  return nombre && categoriasPorNombre.value.has(nombre.toLowerCase())
}
function encargadoValido(nombre) {
  return nombre && encargadosPorNombre.value.has(nombre.toLowerCase())
}
function encargadoEsAmbiguo(nombre) {
  return nombre && (encargadosConteo.value[nombre.toLowerCase()] || 0) > 1
}

const filasConError = computed(() => filas.value.filter(f => f._errores.length > 0))
const filasValidas = computed(() => filas.value.filter(f => f._errores.length === 0))

const filasConEntidadesNuevas = computed(() =>
  filas.value.filter(f =>
    f._errores.length === 0 && (
      (f.categoriaNombre && !categoriaValida(f.categoriaNombre)) ||
      (f.encargadoNombre && !encargadoValido(f.encargadoNombre))
    )
  )
)

const tieneEntidadesNuevas = computed(() =>
  categoriasNuevas.value.length > 0 ||
  encargadosNuevos.value.length > 0 ||
  encargadosAmbiguos.value.length > 0 ||
  categoriasSimilaresADB.value.length > 0 ||
  encargadosSimilaresADB.value.length > 0 ||
  gruposCategoriasSimilares.value.length > 0 ||
  gruposEncargadosSimilares.value.length > 0
)

const encargadosConRolFaltante = computed(() => [
  ...encargadosNuevos.value.filter(e => e.aprobado && !e.rol.trim()),
  ...encargadosSimilaresADB.value.filter(s => s.decision === 'nueva' && !s.rol?.trim()),
  ...gruposEncargadosSimilares.value.filter(g => !g.rol?.trim())
])

const encargadosAmbiguosSinResolver = computed(() =>
  encargadosAmbiguos.value.filter(e => !e.seleccionadoId)
)

const filasAImportar = computed(() => {
  const rechCats = new Set(
    categoriasNuevas.value.filter(c => !c.aprobada).map(c => c.nombre.toLowerCase())
  )
  const rechEncs = new Set(
    encargadosNuevos.value.filter(e => !e.aprobado).map(e => e.nombre.toLowerCase())
  )
  const sinResolver = new Set(
    encargadosAmbiguos.value.filter(e => !e.seleccionadoId).map(e => e.nombre.toLowerCase())
  )
  return filasValidas.value.filter(f => {
    const catCanon = nombreCanonicoCategoria(f.categoriaNombre ?? '').toLowerCase()
    const encCanon = nombreCanonicoEncargado(f.encargadoNombre ?? '').toLowerCase()
    return !rechCats.has(catCanon) && !rechEncs.has(encCanon) && !sinResolver.has(encCanon)
  })
})

const stepsDisplay = computed(() => {
  if (fueValidacion.value) {
    return [
      { label: '1. Preparar archivo', isCurrent: paso.value === 1, isDone: paso.value > 1 },
      { label: '2. Vista previa',     isCurrent: paso.value === 2, isDone: paso.value > 2 },
      { label: '3. Entidades',        isCurrent: paso.value === 3, isDone: paso.value > 3 },
      { label: '4. Resultados',       isCurrent: paso.value === 4, isDone: false }
    ]
  }
  return [
    { label: '1. Preparar archivo', isCurrent: paso.value === 1, isDone: paso.value > 1 },
    { label: '2. Vista previa',     isCurrent: paso.value === 2, isDone: paso.value > 2 },
    { label: '3. Resultados',       isCurrent: paso.value === 4, isDone: false }
  ]
})

const resultadosExitosos   = computed(() => resultados.value.filter(r => r.exitoso))
const resultadosExistentes = computed(() => resultados.value.filter(r => !r.exitoso && r.error?.toLowerCase().includes('ya existe')))
const resultadosError      = computed(() => resultados.value.filter(r => !r.exitoso && !r.error?.toLowerCase().includes('ya existe')))

function filasDependientes(tipo, nombre) {
  const nl = nombre.toLowerCase()
  if (tipo === 'cat') return filasValidas.value.filter(f => f.categoriaNombre?.toLowerCase() === nl)
  return filasValidas.value.filter(f => f.encargadoNombre?.toLowerCase() === nl)
}

function filasDependientesGrupoCat(grupo) {
  return filasValidas.value.filter(f =>
    grupo.nombres.some(n => n.toLowerCase() === f.categoriaNombre?.toLowerCase())
  )
}

function filasDependientesGrupoEnc(grupo) {
  return filasValidas.value.filter(f =>
    grupo.nombres.some(n => n.toLowerCase() === f.encargadoNombre?.toLowerCase())
  )
}

function irAPaso3() {
  fueValidacion.value = true
  paso.value = 3
}

async function descargarPlantilla() {
  const XLSX = await import('xlsx')
  const ws = XLSX.utils.aoa_to_sheet([COLUMNAS])
  ws['!cols'] = [12, 12, 25, 15, 15, 18, 18, 20, 20, 20].map(w => ({ wch: w }))
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, 'Activos')
  XLSX.writeFile(wb, 'Plantilla_Importacion_SIBI.xlsx')
}

function onDrop(e) {
  const file = e.dataTransfer.files[0]
  if (file) procesarArchivo(file)
}

function onFileChange(e) {
  const file = e.target.files[0]
  if (file) procesarArchivo(file)
}

function procesarArchivo(file) {
  archivoError.value = ''
  if (!file.name.match(/\.(xlsx|xls|csv)$/i)) {
    archivoError.value = 'El archivo debe ser .xlsx, .xls o .csv'
    return
  }
  archivoNombre.value = file.name
  fileInput.value._file = file
}

async function parsearArchivo() {
  archivoError.value = ''
  const file = fileInput.value._file
  if (!file) { archivoError.value = 'Selecciona un archivo primero.'; return }

  const XLSX = await import('xlsx')
  let wb
  if (file.name.match(/\.csv$/i)) {
    const text = await file.text()
    wb = XLSX.read(text, { type: 'string' })
  } else {
    const buffer = await file.arrayBuffer()
    wb = XLSX.read(buffer, { type: 'array' })
  }
  const ws = wb.Sheets[wb.SheetNames[0]]
  const rows = XLSX.utils.sheet_to_json(ws, { header: 1, defval: '' })

  if (rows.length < 2) {
    archivoError.value = 'El archivo no contiene datos (solo el encabezado o está vacío).'
    return
  }

  const dataRows = rows.slice(1).filter(r => r.some(c => c !== ''))

  filas.value = dataRows.map((r, i) => {
    const fila = {
      _idx: i + 2,
      placa:           String(r[0] ?? '').trim(),
      tipoPlaca:       String(r[1] ?? '').trim().replace(/^Interna$/i, 'Interno'),
      articulo:        String(r[2] ?? '').trim(),
      marca:           String(r[3] ?? '').trim(),
      modelo:          String(r[4] ?? '').trim(),
      numSerial:       String(r[5] ?? '').trim(),
      categoriaNombre: String(r[6] ?? '').trim(),
      ubicacionActual: String(r[7] ?? '').trim(),
      encargadoNombre: String(r[8] ?? '').trim(),
      observaciones:   String(r[9] ?? '').trim(),
      _errores: []
    }
    if (!fila.placa)           fila._errores.push('Placa vacía')
    if (!fila.articulo)        fila._errores.push('Artículo vacío')
    if (!fila.marca)           fila._errores.push('Marca vacía')
    if (!fila.modelo)          fila._errores.push('Modelo vacío')
    if (!fila.numSerial)       fila._errores.push('N° Serial vacío')
    if (!fila.ubicacionActual) fila._errores.push('Ubicación vacía')
    if (!fila.categoriaNombre) fila._errores.push('Categoría vacía')
    if (!fila.encargadoNombre) fila._errores.push('Encargado vacío')
    if (!['Institucional', 'Interno'].includes(fila.tipoPlaca))
      fila._errores.push('Tipo Placa inválido (Institucional / Interno)')
    return fila
  })

  const knownCats = categoriasPorNombre.value
  const knownEncs = encargadosPorNombre.value

  // ── Category similarity analysis ──────────────────────────────────────
  const newCatNames = [...new Set(
    filas.value
      .filter(f => f._errores.length === 0 && f.categoriaNombre && !knownCats.has(f.categoriaNombre.toLowerCase()))
      .map(f => f.categoriaNombre)
  )]

  const catSimADB = []
  const catEnSimDB = new Set()
  for (const nombre of newCatNames) {
    const match = props.categorias.find(c => esSimilar(nombre, c.nombre))
    if (match) {
      catSimADB.push({ nombreExcel: nombre, categoriaDB: match, decision: 'db', emoji: '' })
      catEnSimDB.add(nombre.toLowerCase())
    }
  }

  const catSinMatchDB = newCatNames.filter(n => !catEnSimDB.has(n.toLowerCase()))
  const gruposCats = []
  const asigCat = new Set()
  for (const nombre of catSinMatchDB) {
    if (asigCat.has(nombre.toLowerCase())) continue
    const grupo = [nombre]
    asigCat.add(nombre.toLowerCase())
    for (const otro of catSinMatchDB) {
      if (!asigCat.has(otro.toLowerCase()) && esSimilar(nombre, otro)) {
        grupo.push(otro)
        asigCat.add(otro.toLowerCase())
      }
    }
    if (grupo.length > 1) gruposCats.push({ nombres: grupo, nombreElegido: grupo[0], emoji: '' })
  }

  const enGrupoCat = new Set(gruposCats.flatMap(g => g.nombres.map(n => n.toLowerCase())))
  const catNuevasRestantes = catSinMatchDB.filter(n => !enGrupoCat.has(n.toLowerCase()))

  categoriasSimilaresADB.value = catSimADB
  gruposCategoriasSimilares.value = gruposCats
  categoriasNuevas.value = catNuevasRestantes.map(n => ({ nombre: n, emoji: '', aprobada: true }))

  // ── Encargado similarity analysis ──────────────────────────────────────
  const newEncNames = [...new Set(
    filas.value
      .filter(f => f._errores.length === 0 && f.encargadoNombre && !knownEncs.has(f.encargadoNombre.toLowerCase()))
      .map(f => f.encargadoNombre)
  )]

  const encSimADB = []
  const encEnSimDB = new Set()
  for (const nombre of newEncNames) {
    const match = props.encargados.find(e => esSimilar(nombre, e.nombre))
    if (match) {
      encSimADB.push({ nombreExcel: nombre, encargadoDB: match, decision: 'db', rol: '' })
      encEnSimDB.add(nombre.toLowerCase())
    }
  }

  const encSinMatchDB = newEncNames.filter(n => !encEnSimDB.has(n.toLowerCase()))
  const gruposEncs = []
  const asigEnc = new Set()
  for (const nombre of encSinMatchDB) {
    if (asigEnc.has(nombre.toLowerCase())) continue
    const grupo = [nombre]
    asigEnc.add(nombre.toLowerCase())
    for (const otro of encSinMatchDB) {
      if (!asigEnc.has(otro.toLowerCase()) && esSimilar(nombre, otro)) {
        grupo.push(otro)
        asigEnc.add(otro.toLowerCase())
      }
    }
    if (grupo.length > 1) gruposEncs.push({ nombres: grupo, nombreElegido: grupo[0], rol: '' })
  }

  const enGrupoEnc = new Set(gruposEncs.flatMap(g => g.nombres.map(n => n.toLowerCase())))
  const encNuevosRestantes = encSinMatchDB.filter(n => !enGrupoEnc.has(n.toLowerCase()))

  const ambiguousEncNames = [...new Set(
    filas.value
      .filter(f => f._errores.length === 0 && f.encargadoNombre && encargadoEsAmbiguo(f.encargadoNombre))
      .map(f => f.encargadoNombre)
  )]

  encargadosSimilaresADB.value = encSimADB
  gruposEncargadosSimilares.value = gruposEncs
  encargadosNuevos.value = encNuevosRestantes.map(n => ({ nombre: n, rol: '', aprobado: true }))
  encargadosAmbiguos.value = ambiguousEncNames.map(nombre => {
    const opciones = props.encargados.filter(e => e.nombre.toLowerCase() === nombre.toLowerCase())
    return { nombre, opciones, seleccionadoId: null }
  })

  fueValidacion.value = false
  paso.value = 2
}

async function confirmarImportacion() {
  if (filasConError.value.length > 0) {
    const count = filasAImportar.value.length
    const ok = await dialog.confirm({
      type: 'warning',
      title: 'Hay filas con errores',
      message: `Solo se importarán los <strong>${count} activo${count !== 1 ? 's' : ''}</strong> sin errores de datos. Las <strong>${filasConError.value.length} fila${filasConError.value.length !== 1 ? 's' : ''}</strong> marcadas en rojo serán ignoradas. ¿Desea continuar?`,
      confirmText: 'Sí, importar los válidos',
      cancelText: 'Cancelar'
    })
    if (!ok) return
  }

  importando.value = true
  try {
    // Truly new categories (no similarity matches)
    for (const cat of categoriasNuevas.value.filter(c => c.aprobada))
      await categoriaService.crear({ nombre: cat.nombre, icono: cat.emoji.trim() || null }).catch(() => {})
    // Similar-to-DB but user chose to create new
    for (const sim of categoriasSimilaresADB.value.filter(s => s.decision === 'nueva'))
      await categoriaService.crear({ nombre: sim.nombreExcel, icono: sim.emoji?.trim() || null }).catch(() => {})
    // One category per similarity group (canonical name)
    for (const grupo of gruposCategoriasSimilares.value)
      await categoriaService.crear({ nombre: grupo.nombreElegido, icono: grupo.emoji?.trim() || null }).catch(() => {})

    // Truly new encargados
    for (const enc of encargadosNuevos.value.filter(e => e.aprobado))
      await encargadoService.crear({ nombre: enc.nombre, rol: enc.rol.trim() }).catch(() => {})
    // Similar-to-DB but user chose to create new
    for (const sim of encargadosSimilaresADB.value.filter(s => s.decision === 'nueva'))
      await encargadoService.crear({ nombre: sim.nombreExcel, rol: sim.rol?.trim() || '' }).catch(() => {})
    // One encargado per similarity group
    for (const grupo of gruposEncargadosSimilares.value)
      await encargadoService.crear({ nombre: grupo.nombreElegido, rol: grupo.rol?.trim() || '' }).catch(() => {})

    // Resolved IDs: ambiguous encargados + similar-to-DB with decision='db'
    const resolvedIds = Object.fromEntries(
      encargadosAmbiguos.value
        .filter(e => e.seleccionadoId)
        .map(e => [e.nombre.toLowerCase(), e.seleccionadoId])
    )
    const dbEncIds = Object.fromEntries(
      encargadosSimilaresADB.value
        .filter(s => s.decision === 'db')
        .map(s => [s.nombreExcel.toLowerCase(), s.encargadoDB.id])
    )

    const payload = filasAImportar.value.map(f => {
      const catCanon = nombreCanonicoCategoria(f.categoriaNombre)
      const encCanon = nombreCanonicoEncargado(f.encargadoNombre)
      const encId = resolvedIds[f.encargadoNombre?.toLowerCase()]
        ?? dbEncIds[f.encargadoNombre?.toLowerCase()]
        ?? null
      return {
        placa:           f.placa,
        tipoPlaca:       f.tipoPlaca,
        articulo:        f.articulo,
        marca:           f.marca,
        modelo:          f.modelo,
        numSerial:       f.numSerial,
        categoriaNombre: catCanon,
        ubicacionActual: f.ubicacionActual,
        encargadoNombre: encCanon,
        observaciones:   f.observaciones || null,
        encargadoId:     encId
      }
    })

    const { data } = await activoService.importar(payload)
    resultados.value = data
    paso.value = 4
    if (resultadosExitosos.value.length > 0) emit('importado')
  } finally {
    importando.value = false
  }
}
</script>

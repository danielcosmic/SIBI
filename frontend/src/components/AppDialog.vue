<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition ease-out duration-200"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition ease-in duration-150"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="state.open"
        class="fixed inset-0 bg-black/50 flex items-center justify-center p-4"
        style="z-index: 99999"
        @keydown.esc="respond(false)"
      >
        <Transition
          enter-active-class="transition ease-out duration-200"
          enter-from-class="opacity-0 scale-95 translate-y-2"
          enter-to-class="opacity-100 scale-100 translate-y-0"
          leave-active-class="transition ease-in duration-150"
          leave-from-class="opacity-100 scale-100 translate-y-0"
          leave-to-class="opacity-0 scale-95 translate-y-2"
        >
          <div v-if="state.open" class="bg-white rounded-2xl shadow-2xl max-w-md w-full overflow-hidden">

            <!-- Header -->
            <div class="px-6 py-5 flex items-center gap-4" :class="headerClass">
              <div class="w-12 h-12 rounded-full flex items-center justify-center flex-shrink-0 bg-white/20">
                <!-- Danger icon -->
                <svg v-if="state.type === 'danger'" xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
                <!-- Warning icon -->
                <svg v-else-if="state.type === 'warning'" xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                <!-- Info icon -->
                <svg v-else xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              <h3 class="text-lg font-bold text-white leading-tight">{{ state.title }}</h3>
            </div>

            <!-- Body -->
            <div class="px-6 py-5">
              <p class="text-gray-600 leading-relaxed">{{ state.message }}</p>
            </div>

            <!-- Buttons -->
            <div class="px-6 pb-6 flex gap-3">
              <button
                v-if="state.showCancel"
                @click="respond(false)"
                class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg hover:bg-gray-50 transition font-medium text-gray-700"
              >
                {{ state.cancelText }}
              </button>
              <button
                @click="respond(true)"
                class="flex-1 px-4 py-2.5 text-white rounded-lg transition font-medium"
                :class="confirmClass"
              >
                {{ state.confirmText }}
              </button>
            </div>

          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup>
import { computed } from 'vue'
import { useDialog } from '@/composables/useDialog'

const { state, respond } = useDialog()

const headerClass = computed(() => ({
  'bg-gradient-to-r from-red-600 to-red-700':       state.type === 'danger',
  'bg-gradient-to-r from-amber-500 to-orange-500':   state.type === 'warning',
  'bg-gradient-to-r from-[#003d7a] to-[#0066cc]':   state.type === 'info'
}))

const confirmClass = computed(() => ({
  'bg-red-600 hover:bg-red-700':       state.type === 'danger',
  'bg-amber-500 hover:bg-amber-600':   state.type === 'warning',
  'bg-[#003d7a] hover:bg-[#002d5a]':  state.type === 'info'
}))
</script>

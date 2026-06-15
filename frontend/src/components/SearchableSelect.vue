<template>
  <div ref="wrapperEl" class="relative">
    <div class="relative">
      <input
        ref="inputEl"
        :value="abierto ? query : selectedLabel"
        :placeholder="!modelValue ? placeholder : ''"
        :disabled="disabled"
        autocomplete="off"
        @focus="onFocus"
        @blur="onBlur"
        @input="onInput"
        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[#0066cc] focus:border-transparent outline-none pr-8"
        :class="disabled ? 'bg-gray-100 cursor-not-allowed text-gray-500' : 'bg-white'"
      />
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="absolute right-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none transition-transform duration-200"
        :class="abierto ? 'rotate-180' : ''"
        fill="none" viewBox="0 0 24 24" stroke="currentColor"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </div>

    <Teleport to="body">
      <div
        v-if="abierto"
        :style="dropdownStyle"
        class="fixed bg-white border border-gray-200 rounded-lg shadow-xl overflow-y-auto"
        style="z-index: 9999; max-height: 200px"
      >
        <div v-if="filteredOptions.length === 0" class="px-4 py-3 text-sm text-gray-400 text-center">
          Sin resultados
        </div>
        <button
          v-for="opt in filteredOptions"
          :key="opt.value"
          type="button"
          @mousedown.prevent="seleccionar(opt)"
          class="w-full px-4 py-2.5 text-left text-sm transition-colors hover:bg-blue-50"
          :class="String(opt.value) === String(modelValue)
            ? 'bg-blue-50 text-[#003d7a] font-medium'
            : 'text-gray-700'"
        >
          {{ opt.label }}
        </button>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  modelValue: { default: '' },
  options: { type: Array, default: () => [] }, // [{ value, label }]
  placeholder: { type: String, default: 'Seleccione...' },
  disabled: { type: Boolean, default: false }
})

const emit = defineEmits(['update:modelValue'])

const wrapperEl = ref(null)
const abierto = ref(false)
const query = ref('')
const dropdownStyle = ref({})

const selectedLabel = computed(() => {
  const opt = props.options.find(o => String(o.value) === String(props.modelValue))
  return opt ? opt.label : ''
})

const filteredOptions = computed(() => {
  if (!query.value.trim()) return props.options
  const q = query.value.toLowerCase()
  return props.options.filter(o => o.label.toLowerCase().includes(q))
})

function updatePosition() {
  if (!wrapperEl.value) return
  const rect = wrapperEl.value.getBoundingClientRect()
  dropdownStyle.value = {
    top: `${rect.bottom + 2}px`,
    left: `${rect.left}px`,
    width: `${rect.width}px`
  }
}

function onFocus() {
  if (props.disabled) return
  updatePosition()
  query.value = ''
  abierto.value = true
}

function onInput(e) {
  query.value = e.target.value
  updatePosition()
  abierto.value = true
}

function onBlur() {
  setTimeout(() => {
    abierto.value = false
    query.value = ''
  }, 150)
}

function seleccionar(opt) {
  emit('update:modelValue', opt.value)
  abierto.value = false
  query.value = ''
}
</script>

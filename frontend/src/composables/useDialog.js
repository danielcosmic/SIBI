import { reactive } from 'vue'

// Estado singleton compartido por todos los imports
const state = reactive({
  open: false,
  type: 'danger',
  title: '',
  message: '',
  confirmText: 'Confirmar',
  cancelText: 'Cancelar',
  showCancel: true,
  resolve: null
})

function openDialog(options) {
  return new Promise((resolve) => {
    Object.assign(state, {
      open: true,
      type: options.type ?? 'danger',
      title: options.title ?? '¿Está seguro?',
      message: options.message ?? '',
      confirmText: options.confirmText ?? 'Confirmar',
      cancelText: options.cancelText ?? 'Cancelar',
      showCancel: options.showCancel !== false,
      resolve
    })
  })
}

export function useDialog() {
  return {
    confirm: (opts) => openDialog({ confirmText: 'Confirmar', showCancel: true, ...opts }),
    alert:   (opts) => openDialog({ confirmText: 'Aceptar',   showCancel: false, ...opts }),
    respond(value) {
      state.open = false
      if (state.resolve) {
        state.resolve(value)
        state.resolve = null
      }
    },
    state
  }
}

import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('sibi_token') || null,
    usuario: JSON.parse(localStorage.getItem('sibi_usuario') || 'null')
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    permisos: (state) => state.usuario?.permisos || null,
    esAdministradora: (state) => state.usuario?.permisos === 'Administradora',
    esGTI: (state) => ['GTI', 'Administradora'].includes(state.usuario?.permisos || ''),
    nombre: (state) => state.usuario?.nombre || '',
    correo: (state) => state.usuario?.correo || ''
  },
  actions: {
    setAuth(token, usuario) {
      this.token = token
      this.usuario = usuario
      localStorage.setItem('sibi_token', token)
      localStorage.setItem('sibi_usuario', JSON.stringify(usuario))
    },
    logout() {
      this.token = null
      this.usuario = null
      localStorage.removeItem('sibi_token')
      localStorage.removeItem('sibi_usuario')
    }
  }
})

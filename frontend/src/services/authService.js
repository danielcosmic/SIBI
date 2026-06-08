import api from './api'

export default {
  login: (correo, contrasena) =>
    api.post('/auth/login', { correo, contrasena }),

  recuperar: (correo) =>
    api.post('/auth/recuperar', { correo }),

  cambiarContrasena: (nuevaContrasena) =>
    api.post('/auth/cambiar-contrasena', { nuevaContrasena })
}

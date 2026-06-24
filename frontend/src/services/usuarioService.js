import api from './api'

export default {
  listar: () =>
    api.get('/usuario'),

  crear: (data) =>
    api.post('/usuario', data),

  editar: (correo, data) =>
    api.put(`/usuario/${encodeURIComponent(correo)}`, data),

  eliminar: (correo) =>
    api.delete(`/usuario/${encodeURIComponent(correo)}`),

  desbloquear: (correo) =>
    api.post(`/usuario/${encodeURIComponent(correo)}/desbloquear`)
}

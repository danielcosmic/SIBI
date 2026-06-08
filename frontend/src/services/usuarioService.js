import api from './api'

export default {
  listar: () =>
    api.get('/usuario'),

  crear: (data) =>
    api.post('/usuario', data),

  editar: (correo, data) =>
    api.put(`/usuario/${encodeURIComponent(correo)}`, data)
}

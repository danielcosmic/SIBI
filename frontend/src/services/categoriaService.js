import api from './api'

export default {
  listar: () =>
    api.get('/categoria'),

  crear: (data) =>
    api.post('/categoria', data),

  eliminar: (id) =>
    api.delete(`/categoria/${id}`)
}

import api from './api'

export default {
  listar: () => api.get('/encargado'),
  crear: (data) => api.post('/encargado', data),
  editar: (id, data) => api.put(`/encargado/${id}`, data),
  eliminar: (id) => api.delete(`/encargado/${id}`)
}

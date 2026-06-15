import api from './api'

export default {
  listar: (params) =>
    api.get('/activo', { params }),

  stats: () =>
    api.get('/activo/stats'),

  obtener: (placa) =>
    api.get(`/activo/${placa}`),

  crear: (data) =>
    api.post('/activo', data),

  editar: (placa, data) =>
    api.put(`/activo/${placa}`, data),

  eliminar: (placa) =>
    api.delete(`/activo/${placa}`),

  recientes: (categoriaId, tamano = 5) =>
    api.get('/activo/recientes', { params: { categoriaId, tamano } })
}

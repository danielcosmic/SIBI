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

  cambiarPlaca: (placa, nuevaPlaca) =>
    api.patch(`/activo/${placa}/cambiar-placa`, { nuevaPlaca }),

  eliminar: (placa) =>
    api.delete(`/activo/${placa}`),

  eliminarReciente: (placa) =>
    api.delete(`/activo/${placa}/reciente`),

  recientes: (categoriaId, tamano = 5) =>
    api.get('/activo/recientes', { params: { categoriaId, tamano } }),

  importar: (filas) =>
    api.post('/activo/importar', filas)
}

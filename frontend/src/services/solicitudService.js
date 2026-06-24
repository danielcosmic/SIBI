import api from './api'

export default {
  listar: (estado) =>
    api.get('/solicitudcambio', { params: estado ? { estado } : {} }),

  listarMias: (estado) =>
    api.get('/solicitudcambio/mis', { params: estado ? { estado } : {} }),

  obtenerPendienteDeActivo: (placa) =>
    api.get(`/solicitudcambio/activo/${encodeURIComponent(placa)}/pendiente`),

  contarPendientes: () =>
    api.get('/solicitudcambio/pendientes/count'),

  crear: (data) =>
    api.post('/solicitudcambio', data),

  aprobar: (id) =>
    api.post(`/solicitudcambio/${id}/aprobar`),

  rechazar: (id, comentario) =>
    api.post(`/solicitudcambio/${id}/rechazar`, { comentario })
}

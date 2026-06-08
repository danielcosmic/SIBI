import api from './api'

export default {
  listar: () =>
    api.get('/desecho'),

  aprobar: (placa) =>
    api.post(`/desecho/${placa}/aprobar`),

  rechazar: (placa) =>
    api.post(`/desecho/${placa}/rechazar`)
}

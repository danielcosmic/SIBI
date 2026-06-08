import api from './api'

export default {
  listar: (params) =>
    api.get('/historial', { params })
}

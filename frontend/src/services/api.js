import axios from 'axios'

const api = axios.create({
  baseURL: process.env.VUE_APP_API_URL ? `${process.env.VUE_APP_API_URL}/api` : '/api',
  paramsSerializer: (params) => {
    const parts = []
    for (const [key, val] of Object.entries(params)) {
      if (Array.isArray(val)) {
        val.forEach(v => parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(v)}`))
      } else if (val !== null && val !== undefined && val !== '') {
        parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(val)}`)
      }
    }
    return parts.join('&')
  }
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('sibi_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    const isAuthEndpoint = error.config?.url?.startsWith('/auth/')
    if (error.response?.status === 401 && !isAuthEndpoint) {
      localStorage.removeItem('sibi_token')
      localStorage.removeItem('sibi_usuario')
      window.location.href = '/'
    }
    return Promise.reject(error)
  }
)

export default api

import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes = [
  {
    path: '/',
    name: 'Login',
    component: () => import('@/views/LoginView.vue')
  },
  {
    path: '/recuperar-contrasena',
    name: 'RecuperarContrasena',
    component: () => import('@/views/RecuperarContrasenaView.vue')
  },
  {
    path: '/',
    component: () => import('@/components/AppLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/DashboardView.vue')
      },
      {
        path: 'inventario',
        name: 'Inventario',
        component: () => import('@/views/InventarioView.vue')
      },
      {
        path: 'historial',
        name: 'Historial',
        component: () => import('@/views/HistorialView.vue')
      },
      {
        path: 'usuarios',
        name: 'Usuarios',
        component: () => import('@/views/UsuariosView.vue'),
        meta: { requiresAdmin: true }
      },
      {
        path: 'categorias',
        name: 'Categorias',
        component: () => import('@/views/CategoriasView.vue')
      },
      {
        path: 'desecho',
        name: 'Desecho',
        component: () => import('@/views/DesechoView.vue')
      }
    ]
  },
  { path: '/:pathMatch(.*)*', redirect: '/' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'Login' }
  }

  if (to.meta.requiresAdmin && !auth.esAdministradora) {
    return { name: 'Dashboard' }
  }

  if ((to.name === 'Login' || to.name === 'RecuperarContrasena') && auth.isAuthenticated) {
    return { name: 'Dashboard' }
  }
})

export default router

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
    path: '/cambiar-contrasena',
    name: 'CambiarContrasena',
    component: () => import('@/views/CambiarContrasenaView.vue'),
    meta: { requiresAuth: true }
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

  // Sin sesión → solo puede ir a Login o RecuperarContrasena
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'Login' }
  }

  // Con contraseña temporal → solo puede ir a CambiarContrasena
  if (auth.isAuthenticated && auth.esContrasenaTemporal && to.name !== 'CambiarContrasena') {
    return { name: 'CambiarContrasena' }
  }

  // Ya autenticado sin contraseña temp → no puede volver a Login/Recuperar
  if ((to.name === 'Login' || to.name === 'RecuperarContrasena') && auth.isAuthenticated && !auth.esContrasenaTemporal) {
    return { name: 'Dashboard' }
  }

  if (to.meta.requiresAdmin && !auth.esAdministradora) {
    return { name: 'Dashboard' }
  }
})

export default router

import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// 路由懒加载
const LoginView    = () => import('@/views/LoginView.vue')
const MainLayout   = () => import('@/layouts/MainLayout.vue')
const DashboardView = () => import('@/views/DashboardView.vue')
const JobListView   = () => import('@/views/jobs/JobListView.vue')
const CandidateListView = () => import('@/views/candidates/CandidateListView.vue')

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { public: true }
  },
  {
    path: '/',
    component: MainLayout,
    children: [
      {
        path: '',
        redirect: '/dashboard'
      },
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: DashboardView,
        meta: { title: '工作台' }
      },
      {
        path: 'jobs',
        name: 'Jobs',
        component: JobListView,
        meta: { title: '职位管理' }
      },
      {
        path: 'candidates',
        name: 'Candidates',
        component: CandidateListView,
        meta: { title: '候选人库' }
      }
    ]
  },
  // 404 捕获
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// ─── 全局导航守卫：JWT 未登录跳转至 /login ────────────────────
router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.public) return true          // 公开页面直接放行

  if (!auth.token) {
    return { name: 'Login', query: { redirect: to.fullPath } }
  }

  return true
})

export default router

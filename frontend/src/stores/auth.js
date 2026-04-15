import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  // ─── State ─────────────────────────────────────────────────
  const token    = ref(localStorage.getItem('ats_token') || '')
  const realName = ref(localStorage.getItem('ats_real_name') || '')
  const roles    = ref(JSON.parse(localStorage.getItem('ats_roles') || '[]'))

  // ─── Getters ───────────────────────────────────────────────
  const isLoggedIn  = computed(() => !!token.value)
  const isAdmin     = computed(() => roles.value.includes('ROLE_ADMIN'))
  const isHR        = computed(() => roles.value.includes('ROLE_HR') || isAdmin.value)

  // ─── Actions ───────────────────────────────────────────────
  function setAuth(data) {
    token.value    = data.token
    realName.value = data.realName
    roles.value    = data.roles || []

    localStorage.setItem('ats_token',     data.token)
    localStorage.setItem('ats_real_name', data.realName)
    localStorage.setItem('ats_roles',     JSON.stringify(data.roles || []))
  }

  function clearAuth() {
    token.value    = ''
    realName.value = ''
    roles.value    = []
    localStorage.removeItem('ats_token')
    localStorage.removeItem('ats_real_name')
    localStorage.removeItem('ats_roles')
  }

  return { token, realName, roles, isLoggedIn, isAdmin, isHR, setAuth, clearAuth }
})

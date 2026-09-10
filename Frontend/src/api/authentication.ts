import type { AuthenticatedUser, AuthenticationStatus } from '../types/authentication'

export async function getAuthenticationStatus() {
  const response = await fetch('/api/auth/status')

  if (!response.ok) {
    throw new Error('Giriş yapılandırması alınamadı.')
  }

  return response.json() as Promise<AuthenticationStatus>
}

export async function getCurrentUser() {
  const response = await fetch('/api/auth/me', { credentials: 'include' })

  if (response.status === 401) {
    return null
  }

  if (!response.ok) {
    throw new Error('Oturum bilgisi alınamadı.')
  }

  return response.json() as Promise<AuthenticatedUser>
}

export function loginWithGoogle() {
  window.location.assign('/api/auth/google-login?returnPath=/')
}

export async function logout() {
  const response = await fetch('/api/auth/logout', {
    method: 'POST',
    credentials: 'include',
  })

  if (!response.ok) {
    throw new Error('Oturum kapatılamadı.')
  }
}

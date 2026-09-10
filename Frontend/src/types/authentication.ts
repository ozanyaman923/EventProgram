export interface AuthenticationStatus {
  googleLoginConfigured: boolean
}

export interface AuthenticatedUser {
  id: string
  displayName: string
  email: string
  isBlocked: boolean
}

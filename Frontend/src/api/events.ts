import type { CreateEventRequest, EventSummary } from '../types/event'

async function request<T>(path: string): Promise<T> {
  const response = await fetch(path)

  if (!response.ok) {
    throw new Error('Etkinlik bilgisi şu anda alınamadı.')
  }

  return response.json() as Promise<T>
}

export function getPublicEvents() {
  return request<EventSummary[]>('/api/events')
}

export function getEventByShareCode(shareCode: string) {
  return request<EventSummary>(`/api/events/${shareCode}`)
}

export async function createEvent(event: CreateEventRequest) {
  const response = await fetch('/api/events', {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(event),
  })

  if (response.status === 401) {
    throw new Error('Etkinlik oluşturmak için Google ile giriş yapmalısın.')
  }

  if (!response.ok) {
    throw new Error('Etkinlik oluşturulamadı. Alanları kontrol edip tekrar dene.')
  }

  return response.json() as Promise<EventSummary>
}

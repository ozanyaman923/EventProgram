import type { EventSummary } from '../types/event'

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

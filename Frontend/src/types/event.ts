export type EventVisibility = 'Public' | 'Private'

export interface EventSummary {
  id: string
  title: string
  description: string
  startsAtUtc: string
  endsAtUtc: string
  capacity: number | null
  visibility: EventVisibility
  shareCode: string
}

export interface CreateEventRequest {
  title: string
  description: string
  startsAtUtc: string
  endsAtUtc: string
  capacity: number | null
  visibility: EventVisibility
}

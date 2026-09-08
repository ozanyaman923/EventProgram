import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { getEventByShareCode, getPublicEvents } from './api/events'
import type { EventSummary } from './types/event'
import './event-program.css'

const demoPrivateShareCode = 'c8395aa8141c438d9c740627e8c493c3'

function formatDate(value: string) {
  return new Intl.DateTimeFormat('tr-TR', {
    dateStyle: 'long',
    timeStyle: 'short',
    timeZone: 'Europe/Istanbul',
  }).format(new Date(value))
}

export default function EventProgramApp() {
  const [events, setEvents] = useState<EventSummary[]>([])
  const [selectedEvent, setSelectedEvent] = useState<EventSummary | null>(null)
  const [shareCode, setShareCode] = useState('')
  const [loading, setLoading] = useState(true)
  const [opening, setOpening] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    void loadEvents()
  }, [])

  async function loadEvents() {
    try {
      setLoading(true)
      setError(null)
      setEvents(await getPublicEvents())
    } catch {
      setError('Etkinlikler yüklenemedi. .NET API çalışıyor mu kontrol et.')
    } finally {
      setLoading(false)
    }
  }

  async function openEvent(code: string) {
    try {
      setOpening(true)
      setError(null)
      setSelectedEvent(await getEventByShareCode(code.trim()))
    } catch {
      setError('Bu paylaşım koduna ait etkinlik bulunamadı.')
    } finally {
      setOpening(false)
    }
  }

  function handlePrivateAccess(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (shareCode.trim()) {
      void openEvent(shareCode)
    }
  }

  return (
    <main className="app-shell">
      <section className="hero-panel">
        <p className="eyebrow inverse">EVENTPROGRAM</p>
        <h1>Etkinlikleri görünür, katılımı anlamlı kıl.</h1>
        <p>Public etkinlikleri keşfet. Private etkinliklere davet bağlantınla eriş.</p>
        <button className="outline-button" type="button" onClick={() => void openEvent(demoPrivateShareCode)}>
          Private demo etkinliğini aç
        </button>
      </section>

      <section className="content-section" aria-labelledby="public-events-heading">
        <div className="section-heading">
          <div>
            <p className="eyebrow">KEŞFET</p>
            <h2 id="public-events-heading">Public etkinlikler</h2>
          </div>
          <button className="text-button" type="button" onClick={() => void loadEvents()}>Yenile</button>
        </div>

        {loading && <p className="status-message">Etkinlikler yükleniyor…</p>}
        {error && <p className="status-message error-message">{error}</p>}
        {!loading && !error && (
          <div className="event-grid">
            {events.map((event) => (
              <article className="event-card" key={event.id}>
                <span className="visibility-badge">{event.visibility}</span>
                <h3>{event.title}</h3>
                <p>{event.description}</p>
                <dl>
                  <div><dt>Tarih</dt><dd>{formatDate(event.startsAtUtc)}</dd></div>
                  <div><dt>Kapasite</dt><dd>{event.capacity ?? 'Sınırsız'} kişi</dd></div>
                </dl>
                <button className="primary-button" type="button" onClick={() => void openEvent(event.shareCode)}>
                  Etkinliği görüntüle
                </button>
              </article>
            ))}
          </div>
        )}
      </section>

      <section className="content-section private-card" aria-labelledby="private-access-heading">
        <p className="eyebrow">DAVETLE ERİŞİM</p>
        <h2 id="private-access-heading">Private etkinlik bağlantın mı var?</h2>
        <p>Paylaşım kodunu girerek listelenmeyen etkinliği görüntüleyebilirsin.</p>
        <form onSubmit={handlePrivateAccess}>
          <label htmlFor="share-code">Paylaşım kodu</label>
          <div className="access-row">
            <input id="share-code" value={shareCode} onChange={(event) => setShareCode(event.target.value)} placeholder="Örn. c8395aa..." />
            <button className="primary-button" disabled={opening} type="submit">
              {opening ? 'Açılıyor…' : 'Etkinliği aç'}
            </button>
          </div>
        </form>
      </section>

      {selectedEvent && (
        <section className="content-section detail-card" aria-live="polite">
          <div className="section-heading">
            <div><p className="eyebrow">{selectedEvent.visibility} ETKİNLİK</p><h2>{selectedEvent.title}</h2></div>
            <button className="text-button" type="button" onClick={() => setSelectedEvent(null)}>Kapat</button>
          </div>
          <p>{selectedEvent.description}</p>
          <p className="event-date">{formatDate(selectedEvent.startsAtUtc)} — {formatDate(selectedEvent.endsAtUtc)}</p>
          <div className="readonly-callout"><strong>Salt okunur moddasın.</strong><span>Oy vermek, soru sormak ve katılım bırakmak için Google ile giriş yakında eklenecek.</span></div>
        </section>
      )}
    </main>
  )
}

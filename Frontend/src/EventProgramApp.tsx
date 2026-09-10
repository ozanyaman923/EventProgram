import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import {
  getAuthenticationStatus,
  getCurrentUser,
  loginWithGoogle,
  logout,
} from './api/authentication'
import { createEvent, getEventByShareCode, getPublicEvents } from './api/events'
import type { AuthenticatedUser } from './types/authentication'
import type { EventSummary, EventVisibility } from './types/event'
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
  const [currentUser, setCurrentUser] = useState<AuthenticatedUser | null>(null)
  const [googleLoginConfigured, setGoogleLoginConfigured] = useState(false)
  const [shareCode, setShareCode] = useState('')
  const [loading, setLoading] = useState(true)
  const [opening, setOpening] = useState(false)
  const [creating, setCreating] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    void initialize()
  }, [])

  async function initialize() {
    await Promise.all([loadEvents(), loadAuthentication()])
  }

  async function loadAuthentication() {
    try {
      const [status, user] = await Promise.all([
        getAuthenticationStatus(),
        getCurrentUser(),
      ])

      setGoogleLoginConfigured(status.googleLoginConfigured)
      setCurrentUser(user)
    } catch {
      setError('Oturum bilgisi yüklenemedi.')
    }
  }

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

  async function handleLogout() {
    try {
      await logout()
      setCurrentUser(null)
    } catch (logoutError) {
      setError(logoutError instanceof Error ? logoutError.message : 'Oturum kapatılamadı.')
    }
  }

  async function handleCreateEvent(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const formElement = event.currentTarget
    const form = new FormData(formElement)

    try {
      setCreating(true)
      setError(null)
      const capacityText = String(form.get('capacity') ?? '').trim()
      const createdEvent = await createEvent({
        title: String(form.get('title') ?? ''),
        description: String(form.get('description') ?? ''),
        startsAtUtc: new Date(String(form.get('startsAt') ?? '')).toISOString(),
        endsAtUtc: new Date(String(form.get('endsAt') ?? '')).toISOString(),
        capacity: capacityText ? Number(capacityText) : null,
        visibility: String(form.get('visibility')) as EventVisibility,
      })

      formElement.reset()
      setSelectedEvent(createdEvent)
      await loadEvents()
    } catch (creationError) {
      setError(creationError instanceof Error ? creationError.message : 'Etkinlik oluşturulamadı.')
    } finally {
      setCreating(false)
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
        <div className="hero-account">
          <p className="eyebrow inverse">EVENTPROGRAM</p>
          {currentUser ? (
            <div className="account-actions">
              <span>{currentUser.displayName}</span>
              <button className="outline-button compact-button" type="button" onClick={() => void handleLogout()}>
                Çıkış yap
              </button>
            </div>
          ) : (
            <button
              className="outline-button compact-button"
              disabled={!googleLoginConfigured}
              type="button"
              onClick={loginWithGoogle}
            >
              Google ile giriş
            </button>
          )}
        </div>
        <h1>Etkinlikleri görünür, katılımı anlamlı kıl.</h1>
        <p>Public etkinlikleri keşfet. Private etkinliklere davet bağlantınla eriş.</p>
        {!googleLoginConfigured && (
          <p className="configuration-note">Google girişi için geliştirme anahtarları henüz eklenmedi.</p>
        )}
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
        {!loading && (
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

      <section className="content-section organizer-card" aria-labelledby="create-event-heading">
        <p className="eyebrow">ORGANİZATÖR</p>
        <h2 id="create-event-heading">Kendi etkinliğini oluştur</h2>
        {!currentUser ? (
          <p>Etkinlik oluşturmak için Google ile giriş yapmalısın. Etkinlikleri görüntülemek için giriş gerekmez.</p>
        ) : (
          <form className="event-form" onSubmit={(event) => void handleCreateEvent(event)}>
            <label>
              Etkinlik adı
              <input name="title" required maxLength={150} />
            </label>
            <label className="full-field">
              Açıklama
              <textarea name="description" required maxLength={5000} rows={4} />
            </label>
            <label>
              Başlangıç
              <input name="startsAt" type="datetime-local" required />
            </label>
            <label>
              Bitiş
              <input name="endsAt" type="datetime-local" required />
            </label>
            <label>
              Kapasite (isteğe bağlı)
              <input name="capacity" type="number" min={1} />
            </label>
            <label>
              Görünürlük
              <select name="visibility" defaultValue="Public">
                <option value="Public">Public — herkes görebilir</option>
                <option value="Private">Private — bağlantısı olan görebilir</option>
              </select>
            </label>
            <button className="primary-button full-field" disabled={creating} type="submit">
              {creating ? 'Oluşturuluyor…' : 'Etkinliği oluştur ve yayınla'}
            </button>
          </form>
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
          <div className="readonly-callout">
            <strong>{currentUser ? 'Oturumun açık.' : 'Salt okunur moddasın.'}</strong>
            <span>Oy verme, soru sorma ve yorum özellikleri sonraki geliştirme paketlerinde eklenecek.</span>
          </div>
        </section>
      )}
    </main>
  )
}

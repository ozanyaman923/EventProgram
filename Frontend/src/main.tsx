import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './global.css'
import EventProgramApp from './EventProgramApp.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <EventProgramApp />
  </StrictMode>,
)

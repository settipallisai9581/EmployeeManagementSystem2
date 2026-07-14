import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { clientLogger } from './services/logger'

clientLogger.info('Client bootstrap started')

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)

clientLogger.info('Client bootstrap completed')

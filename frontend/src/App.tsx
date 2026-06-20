import { Navigate, Route, Routes } from 'react-router-dom';
import NuevoTorneoPage from '@/routes/torneos/nuevo';
import TorneosPage from '@/routes/torneos';

function App() {
  return (
    <div className="flex min-h-screen flex-col">
      <header className="bg-secondary text-secondary-foreground">
        <nav className="mx-auto flex max-w-5xl items-center px-4 py-4">
          <span className="text-lg leading-relaxed font-bold">TorneoApp</span>
        </nav>
      </header>

      <main className="mx-auto w-full max-w-5xl flex-1 px-4 py-8">
        <Routes>
          <Route path="/" element={<Navigate to="/torneos" replace />} />
          <Route path="/torneos" element={<TorneosPage />} />
          <Route path="/torneos/nuevo" element={<NuevoTorneoPage />} />
        </Routes>
      </main>
    </div>
  );
}

export default App;

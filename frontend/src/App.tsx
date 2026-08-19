import type { ReactNode } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { RoleGuard } from '@/components/layout/RoleGuard';
import LoginPage from '@/routes/login';
import CategoriasPage from '@/routes/torneos/$torneoId/categorias';
import EditarCategoriaPage from '@/routes/torneos/$torneoId/categorias/$categoriaId/editar';
import CompetidoresPage from '@/routes/torneos/$torneoId/competidores';
import EditarCompetidorPage from '@/routes/torneos/$torneoId/competidores/$competidorId/editar';
import EditarTorneoPage from '@/routes/torneos/$torneoId/editar';
import NuevoTorneoPage from '@/routes/torneos/nuevo';
import TorneosPage from '@/routes/torneos';
import { useAuthStore } from '@/store/authStore';

function AppShell({ children }: { children: ReactNode }) {
  const usuario = useAuthStore((state) => state.usuario);
  const logout = useAuthStore((state) => state.logout);

  return (
    <div className="flex min-h-screen flex-col">
      <header className="bg-secondary text-secondary-foreground">
        <nav className="mx-auto flex max-w-5xl items-center justify-between px-4 py-4">
          <span className="text-lg leading-relaxed font-bold">TorneoApp</span>
          {usuario && (
            <div className="flex items-center gap-4">
              <span className="text-sm leading-relaxed">
                {usuario.nombre} · {usuario.rol}
              </span>
              <button
                type="button"
                onClick={logout}
                className="flex min-h-11 items-center rounded-md px-3 text-sm font-medium underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-white"
              >
                Cerrar sesión
              </button>
            </div>
          )}
        </nav>
      </header>

      <main className="mx-auto w-full max-w-5xl flex-1 px-4 py-8">{children}</main>
    </div>
  );
}

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/*"
        element={
          <RoleGuard>
            <AppShell>
              <Routes>
                <Route path="/" element={<Navigate to="/torneos" replace />} />
                <Route path="/torneos" element={<TorneosPage />} />
                <Route
                  path="/torneos/nuevo"
                  element={
                    <RoleGuard roles={['Coordinador']}>
                      <NuevoTorneoPage />
                    </RoleGuard>
                  }
                />
                <Route
                  path="/torneos/:torneoId/editar"
                  element={
                    <RoleGuard roles={['Coordinador']}>
                      <EditarTorneoPage />
                    </RoleGuard>
                  }
                />
                <Route
                  path="/torneos/:torneoId/competidores"
                  element={
                    <RoleGuard roles={['Coordinador']}>
                      <CompetidoresPage />
                    </RoleGuard>
                  }
                />
                <Route
                  path="/torneos/:torneoId/competidores/:competidorId/editar"
                  element={
                    <RoleGuard roles={['Coordinador']}>
                      <EditarCompetidorPage />
                    </RoleGuard>
                  }
                />
                <Route path="/torneos/:torneoId/categorias" element={<CategoriasPage />} />
                <Route
                  path="/torneos/:torneoId/categorias/:categoriaId/editar"
                  element={
                    <RoleGuard roles={['Coordinador']}>
                      <EditarCategoriaPage />
                    </RoleGuard>
                  }
                />
              </Routes>
            </AppShell>
          </RoleGuard>
        }
      />
    </Routes>
  );
}

export default App;

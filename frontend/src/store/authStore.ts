import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { LoginResult, Usuario } from '@/types';

interface AuthState {
  token: string | null;
  usuario: Usuario | null;
  login: (result: LoginResult) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      usuario: null,
      login: (result) =>
        set({
          token: result.token,
          usuario: { nombre: result.nombre, email: result.email, rol: result.rol },
        }),
      logout: () => set({ token: null, usuario: null }),
    }),
    { name: 'torneo-auth' },
  ),
);

import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import type { RolUsuario } from '@/types';

interface RoleGuardProps {
  children: ReactNode;
  roles?: RolUsuario[];
}

export function RoleGuard({ children, roles }: RoleGuardProps) {
  const { token, usuario } = useAuthStore();

  if (!token || !usuario) {
    return <Navigate to="/login" replace />;
  }

  if (roles && !roles.includes(usuario.rol)) {
    return <Navigate to="/torneos" replace />;
  }

  return children;
}

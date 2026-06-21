import { LoginForm } from '@/components/auth/LoginForm';

export default function LoginPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-muted/30 px-4">
      <main className="w-full max-w-sm rounded-lg border bg-card p-8 shadow-sm">
        <h1 className="mb-6 text-2xl leading-relaxed font-bold text-left">TorneoApp</h1>
        <LoginForm />
      </main>
    </div>
  );
}

import { AlertDialog } from '@base-ui/react/alert-dialog';
import type { ReactNode } from 'react';
import { buttonVariants } from '@/components/ui/button';
import { cn } from '@/lib/utils';

interface ConfirmDialogProps {
  /** Contenido del disparador (por ej. el texto "Eliminar"). */
  trigger: ReactNode;
  title: string;
  description: string;
  confirmLabel?: string;
  cancelLabel?: string;
  /** Acción a ejecutar al confirmar (típicamente una mutación). */
  onConfirm: () => void;
  /** Clase del disparador; por defecto un botón de texto destructivo. */
  triggerClassName?: string;
}

const defaultTriggerClass =
  'inline-flex min-h-11 items-center font-medium text-destructive underline-offset-4 hover:underline focus-visible:outline focus-visible:outline-3 focus-visible:outline-destructive';

/**
 * Diálogo de confirmación reutilizable para acciones irreversibles (WCAG 2.2: confirmación previa).
 * Muestra un título, una descripción y botones Cancelar / Confirmar.
 */
export function ConfirmDialog({
  trigger,
  title,
  description,
  confirmLabel = 'Eliminar',
  cancelLabel = 'Cancelar',
  onConfirm,
  triggerClassName,
}: ConfirmDialogProps) {
  return (
    <AlertDialog.Root>
      <AlertDialog.Trigger className={triggerClassName ?? defaultTriggerClass}>{trigger}</AlertDialog.Trigger>
      <AlertDialog.Portal>
        <AlertDialog.Backdrop className="fixed inset-0 z-50 bg-black/40 transition-opacity data-ending-style:opacity-0 data-starting-style:opacity-0" />
        <AlertDialog.Popup className="fixed top-1/2 left-1/2 z-50 flex w-[min(28rem,calc(100vw-2rem))] -translate-x-1/2 -translate-y-1/2 flex-col gap-6 rounded-lg border bg-card p-6 text-card-foreground shadow-lg transition-[opacity,scale] data-ending-style:scale-95 data-ending-style:opacity-0 data-starting-style:scale-95 data-starting-style:opacity-0">
          <div className="flex flex-col gap-2">
            <AlertDialog.Title className="text-base leading-relaxed font-semibold text-left">
              {title}
            </AlertDialog.Title>
            <AlertDialog.Description className="text-sm leading-relaxed text-muted-foreground text-left">
              {description}
            </AlertDialog.Description>
          </div>
          <div className="flex flex-wrap justify-end gap-4">
            <AlertDialog.Close className={cn(buttonVariants({ variant: 'outline' }), 'min-h-11 px-4')}>
              {cancelLabel}
            </AlertDialog.Close>
            <AlertDialog.Close
              className={cn(buttonVariants({ variant: 'destructive' }), 'min-h-11 px-4')}
              onClick={onConfirm}
            >
              {confirmLabel}
            </AlertDialog.Close>
          </div>
        </AlertDialog.Popup>
      </AlertDialog.Portal>
    </AlertDialog.Root>
  );
}

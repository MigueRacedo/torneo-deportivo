import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useCrearTorneo, useEditarTorneo } from '@/hooks/useTorneos';
import { ApiError } from '@/lib/api';
import type { Torneo } from '@/types';

const hoyIso = new Date().toISOString().slice(0, 10);

const torneoSchema = z.object({
  nombre: z
    .string()
    .trim()
    .min(1, 'El nombre del torneo es obligatorio.')
    .max(200, 'El nombre no puede superar los 200 caracteres.'),
  fecha: z
    .string()
    .min(1, 'Elegí una fecha para el torneo.')
    .refine((value) => value > hoyIso, 'La fecha debe ser posterior a hoy.'),
  lugar: z
    .string()
    .trim()
    .min(1, 'El lugar es obligatorio.')
    .max(300, 'El lugar no puede superar los 300 caracteres.'),
  imagenFlyer: z.string().trim().optional(),
});

type TorneoFormValues = z.infer<typeof torneoSchema>;

/**
 * Formulario de torneo reutilizable. Sin `torneo` crea uno nuevo (H0001); con `torneo`
 * precarga los datos y edita el existente (H0003).
 */
export function TorneoForm({ torneo }: { torneo?: Torneo }) {
  const navigate = useNavigate();
  const esEdicion = Boolean(torneo);

  const crear = useCrearTorneo();
  const editar = useEditarTorneo(torneo?.id ?? '');
  const { mutate, isPending } = esEdicion ? editar : crear;

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<TorneoFormValues>({
    resolver: zodResolver(torneoSchema),
    defaultValues: torneo
      ? { nombre: torneo.nombre, fecha: torneo.fecha, lugar: torneo.lugar, imagenFlyer: torneo.imagenFlyer ?? '' }
      : undefined,
  });

  const onSubmit = (values: TorneoFormValues) => {
    mutate(
      { ...values, imagenFlyer: values.imagenFlyer || undefined },
      {
        onSuccess: (guardado) => {
          toast.success(
            esEdicion
              ? `Torneo "${guardado.nombre}" actualizado correctamente.`
              : `Torneo "${guardado.nombre}" creado correctamente.`,
          );
          navigate('/torneos');
        },
        onError: (error) => {
          const fallback = esEdicion ? 'No se pudo actualizar el torneo.' : 'No se pudo crear el torneo.';
          toast.error(error instanceof ApiError ? error.message : fallback);
        },
      },
    );
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6" noValidate>
      <div className="flex flex-col gap-2">
        <Label htmlFor="nombre">Nombre del torneo</Label>
        <Input
          id="nombre"
          className="min-h-11"
          placeholder="Ej: Copa Apertura Taekwondo"
          aria-invalid={Boolean(errors.nombre)}
          aria-describedby={errors.nombre ? 'nombre-error' : undefined}
          {...register('nombre')}
        />
        {errors.nombre && (
          <p id="nombre-error" className="text-sm text-destructive">
            {errors.nombre.message}
          </p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="fecha">Fecha</Label>
        <Input
          id="fecha"
          type="date"
          className="min-h-11"
          aria-invalid={Boolean(errors.fecha)}
          aria-describedby={errors.fecha ? 'fecha-error' : undefined}
          {...register('fecha')}
        />
        {errors.fecha && (
          <p id="fecha-error" className="text-sm text-destructive">
            {errors.fecha.message}
          </p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="lugar">Lugar</Label>
        <Input
          id="lugar"
          className="min-h-11"
          placeholder="Ej: Club San Martín, Buenos Aires"
          aria-invalid={Boolean(errors.lugar)}
          aria-describedby={errors.lugar ? 'lugar-error' : undefined}
          {...register('lugar')}
        />
        {errors.lugar && (
          <p id="lugar-error" className="text-sm text-destructive">
            {errors.lugar.message}
          </p>
        )}
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="imagenFlyer">Flyer del torneo (URL, opcional)</Label>
        <Input id="imagenFlyer" className="min-h-11" placeholder="https://..." {...register('imagenFlyer')} />
      </div>

      <Button type="submit" disabled={isPending} className="min-h-11 self-start px-6">
        {isPending
          ? esEdicion
            ? 'Guardando cambios…'
            : 'Creando torneo…'
          : esEdicion
            ? 'Guardar cambios'
            : 'Crear torneo'}
      </Button>
    </form>
  );
}

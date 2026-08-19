import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { z } from 'zod';
import { formatearGraduacion } from '@/components/categorias/graduacion';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useCargarCompetidor } from '@/hooks/useCompetidores';
import { ApiError } from '@/lib/api';
import { GRADUACIONES } from '@/types';

// Estilo compartido para los <select> nativos (coincide con el <Input> de shadcn).
const selectClass =
  'min-h-11 w-full rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 md:text-sm';

const competidorSchema = z.object({
  nombre: z.string().trim().min(1, 'El nombre es obligatorio.').max(150, 'Máximo 150 caracteres.'),
  apellido: z.string().trim().min(1, 'El apellido es obligatorio.').max(150, 'Máximo 150 caracteres.'),
  sexo: z.enum(['M', 'F'], { message: 'Elegí el sexo.' }),
  edad: z
    .number({ message: 'Ingresá la edad.' })
    .int('La edad debe ser un número entero.')
    .min(1, 'La edad debe estar entre 1 y 120.')
    .max(120, 'La edad debe estar entre 1 y 120.'),
  graduacion: z.enum(GRADUACIONES, { message: 'Elegí la graduación.' }),
  peso: z
    .number({ message: 'Ingresá el peso.' })
    .positive('El peso debe ser mayor a 0.')
    .max(500, 'El peso debe ser hasta 500 kg.'),
  altura: z
    .number({ message: 'Ingresá la altura.' })
    .positive('La altura debe ser mayor a 0.')
    .max(3, 'La altura debe ser hasta 3 metros.'),
  escuela: z.string().trim().min(1, 'La escuela es obligatoria.').max(200, 'Máximo 200 caracteres.'),
  responsable: z.string().trim().min(1, 'El responsable es obligatorio.').max(200, 'Máximo 200 caracteres.'),
  telefono: z.string().trim().max(50, 'Máximo 50 caracteres.').optional(),
});

type CompetidorFormValues = z.infer<typeof competidorSchema>;

export function CompetidorForm({ torneoId }: { torneoId: string }) {
  const { mutate, isPending } = useCargarCompetidor(torneoId);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CompetidorFormValues>({
    resolver: zodResolver(competidorSchema),
    defaultValues: { sexo: 'M', graduacion: 'CinturonBlanco' },
  });

  const onSubmit = (values: CompetidorFormValues) => {
    mutate(
      { ...values, telefono: values.telefono || undefined },
      {
        onSuccess: (competidor) => {
          toast.success(`Competidor "${competidor.nombreCompleto}" cargado correctamente.`);
          reset({ sexo: 'M', graduacion: 'CinturonBlanco' });
        },
        onError: (error) => {
          toast.error(error instanceof ApiError ? error.message : 'No se pudo cargar el competidor.');
        },
      },
    );
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6" noValidate>
      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="flex flex-col gap-2">
          <Label htmlFor="nombre">Nombre</Label>
          <Input
            id="nombre"
            className="min-h-11"
            aria-invalid={Boolean(errors.nombre)}
            aria-describedby={errors.nombre ? 'nombre-error' : undefined}
            {...register('nombre')}
          />
          {errors.nombre && (
            <p id="nombre-error" className="text-sm leading-relaxed text-destructive">
              {errors.nombre.message}
            </p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="apellido">Apellido</Label>
          <Input
            id="apellido"
            className="min-h-11"
            aria-invalid={Boolean(errors.apellido)}
            aria-describedby={errors.apellido ? 'apellido-error' : undefined}
            {...register('apellido')}
          />
          {errors.apellido && (
            <p id="apellido-error" className="text-sm leading-relaxed text-destructive">
              {errors.apellido.message}
            </p>
          )}
        </div>
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="sexo">Sexo</Label>
        <select id="sexo" className={selectClass} aria-invalid={Boolean(errors.sexo)} {...register('sexo')}>
          <option value="M">Masculino</option>
          <option value="F">Femenino</option>
        </select>
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
        <div className="flex flex-col gap-2">
          <Label htmlFor="edad">Edad</Label>
          <Input
            id="edad"
            type="number"
            min={1}
            className="min-h-11"
            aria-invalid={Boolean(errors.edad)}
            aria-describedby={errors.edad ? 'edad-error' : undefined}
            {...register('edad', { valueAsNumber: true })}
          />
          {errors.edad && (
            <p id="edad-error" className="text-sm leading-relaxed text-destructive">
              {errors.edad.message}
            </p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="peso">Peso (kg)</Label>
          <Input
            id="peso"
            type="number"
            min={0}
            step="0.01"
            className="min-h-11"
            aria-invalid={Boolean(errors.peso)}
            aria-describedby={errors.peso ? 'peso-error' : undefined}
            {...register('peso', { valueAsNumber: true })}
          />
          {errors.peso && (
            <p id="peso-error" className="text-sm leading-relaxed text-destructive">
              {errors.peso.message}
            </p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="altura">Altura (m)</Label>
          <Input
            id="altura"
            type="number"
            min={0}
            step="0.01"
            className="min-h-11"
            aria-invalid={Boolean(errors.altura)}
            aria-describedby={errors.altura ? 'altura-error' : undefined}
            {...register('altura', { valueAsNumber: true })}
          />
          {errors.altura && (
            <p id="altura-error" className="text-sm leading-relaxed text-destructive">
              {errors.altura.message}
            </p>
          )}
        </div>
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="graduacion">Graduación</Label>
        <select
          id="graduacion"
          className={selectClass}
          aria-invalid={Boolean(errors.graduacion)}
          {...register('graduacion')}
        >
          {GRADUACIONES.map((g) => (
            <option key={g} value={g}>
              {formatearGraduacion(g)}
            </option>
          ))}
        </select>
      </div>

      <div className="flex flex-col gap-2">
        <Label htmlFor="escuela">Escuela</Label>
        <Input
          id="escuela"
          className="min-h-11"
          aria-invalid={Boolean(errors.escuela)}
          aria-describedby={errors.escuela ? 'escuela-error' : undefined}
          {...register('escuela')}
        />
        {errors.escuela && (
          <p id="escuela-error" className="text-sm leading-relaxed text-destructive">
            {errors.escuela.message}
          </p>
        )}
      </div>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="flex flex-col gap-2">
          <Label htmlFor="responsable">Responsable</Label>
          <Input
            id="responsable"
            className="min-h-11"
            aria-invalid={Boolean(errors.responsable)}
            aria-describedby={errors.responsable ? 'responsable-error' : undefined}
            {...register('responsable')}
          />
          {errors.responsable && (
            <p id="responsable-error" className="text-sm leading-relaxed text-destructive">
              {errors.responsable.message}
            </p>
          )}
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="telefono">Teléfono (opcional)</Label>
          <Input
            id="telefono"
            type="tel"
            className="min-h-11"
            aria-invalid={Boolean(errors.telefono)}
            aria-describedby={errors.telefono ? 'telefono-error' : undefined}
            {...register('telefono')}
          />
          {errors.telefono && (
            <p id="telefono-error" className="text-sm leading-relaxed text-destructive">
              {errors.telefono.message}
            </p>
          )}
        </div>
      </div>

      <Button type="submit" disabled={isPending} className="min-h-11 self-start px-6">
        {isPending ? 'Cargando competidor…' : 'Cargar competidor'}
      </Button>
    </form>
  );
}

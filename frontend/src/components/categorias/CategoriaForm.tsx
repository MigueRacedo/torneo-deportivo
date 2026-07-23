import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { toast } from 'sonner';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useCrearCategoria } from '@/hooks/useCategorias';
import { ApiError } from '@/lib/api';
import { GRADUACIONES } from '@/types';
import { formatearGraduacion } from './graduacion';

// Estilo compartido para los <select> nativos (coincide con el <Input> de shadcn).
const selectClass =
  'min-h-11 w-full rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 md:text-sm';

const indiceGraduacion = (g: string) => GRADUACIONES.indexOf(g as (typeof GRADUACIONES)[number]);

const categoriaSchema = z
  .object({
    nombre: z
      .string()
      .trim()
      .min(1, 'El nombre de la categoría es obligatorio.')
      .max(200, 'El nombre no puede superar los 200 caracteres.'),
    tipoCompetencia: z.enum(['Combate', 'Formas'], {
      message: 'Elegí el tipo de competencia.',
    }),
    sexo: z.enum(['M', 'F'], { message: 'Elegí el sexo de la categoría.' }),
    rangoEdadMin: z
      .number({ message: 'Ingresá la edad mínima.' })
      .int()
      .min(0, 'La edad mínima no puede ser negativa.'),
    rangoEdadMax: z
      .number({ message: 'Ingresá la edad máxima.' })
      .int()
      .min(0, 'La edad máxima no puede ser negativa.'),
    rangoPesoMin: z
      .number({ message: 'Ingresá el peso mínimo.' })
      .positive('El peso mínimo debe ser mayor a 0.'),
    rangoPesoMax: z
      .number({ message: 'Ingresá el peso máximo.' })
      .positive('El peso máximo debe ser mayor a 0.'),
    rangoGraduacionMin: z.enum(GRADUACIONES, { message: 'Elegí la graduación mínima.' }),
    rangoGraduacionMax: z.enum(GRADUACIONES, { message: 'Elegí la graduación máxima.' }),
  })
  .refine((v) => v.rangoEdadMax >= v.rangoEdadMin, {
    path: ['rangoEdadMax'],
    message: 'La edad máxima no puede ser menor que la mínima.',
  })
  .refine((v) => v.rangoPesoMax >= v.rangoPesoMin, {
    path: ['rangoPesoMax'],
    message: 'El peso máximo no puede ser menor que el mínimo.',
  })
  .refine((v) => indiceGraduacion(v.rangoGraduacionMax) >= indiceGraduacion(v.rangoGraduacionMin), {
    path: ['rangoGraduacionMax'],
    message: 'La graduación máxima no puede ser menor que la mínima.',
  });

type CategoriaFormValues = z.infer<typeof categoriaSchema>;

export function CategoriaForm({ torneoId }: { torneoId: string }) {
  const { mutate, isPending } = useCrearCategoria(torneoId);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CategoriaFormValues>({
    resolver: zodResolver(categoriaSchema),
    defaultValues: {
      tipoCompetencia: 'Combate',
      sexo: 'M',
      rangoGraduacionMin: 'CinturonBlanco',
      rangoGraduacionMax: 'CinturonBlanco',
    },
  });

  const onSubmit = (values: CategoriaFormValues) => {
    mutate(values, {
      onSuccess: (categoria) => {
        toast.success(`Categoría "${categoria.nombre}" creada correctamente.`);
        reset();
      },
      onError: (error) => {
        const message = error instanceof ApiError ? error.message : 'No se pudo crear la categoría.';
        toast.error(message);
      },
    });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-6" noValidate>
      <div className="flex flex-col gap-2">
        <Label htmlFor="nombre">Nombre de la categoría</Label>
        <Input
          id="nombre"
          className="min-h-11"
          placeholder="Ej: Adultos A - Combate Femenino"
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

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <div className="flex flex-col gap-2">
          <Label htmlFor="tipoCompetencia">Tipo de competencia</Label>
          <select
            id="tipoCompetencia"
            className={selectClass}
            aria-invalid={Boolean(errors.tipoCompetencia)}
            {...register('tipoCompetencia')}
          >
            <option value="Combate">Combate</option>
            <option value="Formas">Formas</option>
          </select>
        </div>

        <div className="flex flex-col gap-2">
          <Label htmlFor="sexo">Sexo</Label>
          <select
            id="sexo"
            className={selectClass}
            aria-invalid={Boolean(errors.sexo)}
            {...register('sexo')}
          >
            <option value="M">Masculino</option>
            <option value="F">Femenino</option>
          </select>
        </div>
      </div>

      <fieldset className="flex flex-col gap-2">
        <legend className="mb-2 text-sm leading-relaxed font-medium">Rango de edad (años)</legend>
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoEdadMin">Edad mínima</Label>
            <Input
              id="rangoEdadMin"
              type="number"
              min={0}
              className="min-h-11"
              aria-invalid={Boolean(errors.rangoEdadMin)}
              aria-describedby={errors.rangoEdadMin ? 'edad-min-error' : undefined}
              {...register('rangoEdadMin', { valueAsNumber: true })}
            />
            {errors.rangoEdadMin && (
              <p id="edad-min-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoEdadMin.message}
              </p>
            )}
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoEdadMax">Edad máxima</Label>
            <Input
              id="rangoEdadMax"
              type="number"
              min={0}
              className="min-h-11"
              aria-invalid={Boolean(errors.rangoEdadMax)}
              aria-describedby={errors.rangoEdadMax ? 'edad-max-error' : undefined}
              {...register('rangoEdadMax', { valueAsNumber: true })}
            />
            {errors.rangoEdadMax && (
              <p id="edad-max-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoEdadMax.message}
              </p>
            )}
          </div>
        </div>
      </fieldset>

      <fieldset className="flex flex-col gap-2">
        <legend className="mb-2 text-sm leading-relaxed font-medium">Rango de peso (kg)</legend>
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoPesoMin">Peso mínimo</Label>
            <Input
              id="rangoPesoMin"
              type="number"
              min={0}
              step="0.01"
              className="min-h-11"
              aria-invalid={Boolean(errors.rangoPesoMin)}
              aria-describedby={errors.rangoPesoMin ? 'peso-min-error' : undefined}
              {...register('rangoPesoMin', { valueAsNumber: true })}
            />
            {errors.rangoPesoMin && (
              <p id="peso-min-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoPesoMin.message}
              </p>
            )}
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoPesoMax">Peso máximo</Label>
            <Input
              id="rangoPesoMax"
              type="number"
              min={0}
              step="0.01"
              className="min-h-11"
              aria-invalid={Boolean(errors.rangoPesoMax)}
              aria-describedby={errors.rangoPesoMax ? 'peso-max-error' : undefined}
              {...register('rangoPesoMax', { valueAsNumber: true })}
            />
            {errors.rangoPesoMax && (
              <p id="peso-max-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoPesoMax.message}
              </p>
            )}
          </div>
        </div>
      </fieldset>

      <fieldset className="flex flex-col gap-2">
        <legend className="mb-2 text-sm leading-relaxed font-medium">Rango de graduación</legend>
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoGraduacionMin">Graduación mínima</Label>
            <select id="rangoGraduacionMin" className={selectClass} {...register('rangoGraduacionMin')}>
              {GRADUACIONES.map((g) => (
                <option key={g} value={g}>
                  {formatearGraduacion(g)}
                </option>
              ))}
            </select>
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoGraduacionMax">Graduación máxima</Label>
            <select
              id="rangoGraduacionMax"
              className={selectClass}
              aria-invalid={Boolean(errors.rangoGraduacionMax)}
              aria-describedby={errors.rangoGraduacionMax ? 'grad-max-error' : undefined}
              {...register('rangoGraduacionMax')}
            >
              {GRADUACIONES.map((g) => (
                <option key={g} value={g}>
                  {formatearGraduacion(g)}
                </option>
              ))}
            </select>
            {errors.rangoGraduacionMax && (
              <p id="grad-max-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoGraduacionMax.message}
              </p>
            )}
          </div>
        </div>
      </fieldset>

      <Button type="submit" disabled={isPending} className="min-h-11 self-start px-6">
        {isPending ? 'Creando categoría…' : 'Crear categoría'}
      </Button>
    </form>
  );
}

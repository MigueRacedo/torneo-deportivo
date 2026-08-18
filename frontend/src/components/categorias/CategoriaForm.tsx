import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useCrearCategoria, useEditarCategoria } from '@/hooks/useCategorias';
import { ApiError } from '@/lib/api';
import { GRADUACIONES, type Categoria } from '@/types';
import { formatearGraduacion } from './graduacion';

// Estilo compartido para los <select> nativos (coincide con el <Input> de shadcn).
const selectClass =
  'min-h-11 w-full rounded-lg border border-input bg-transparent px-2.5 py-1 text-base transition-colors outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 aria-invalid:border-destructive aria-invalid:ring-3 aria-invalid:ring-destructive/20 md:text-sm';

const indiceGraduacion = (g: string) => GRADUACIONES.indexOf(g as (typeof GRADUACIONES)[number]);

// Convierte el valor del input a número opcional: vacío => undefined (rango abierto).
const aNumeroOpcional = (v: unknown) => {
  if (v === '' || v === null || v === undefined) return undefined;
  const n = Number(v);
  return Number.isNaN(n) ? undefined : n;
};

const categoriaSchema = z
  .object({
    nombre: z
      .string()
      .trim()
      .min(1, 'El nombre de la categoría es obligatorio.')
      .max(200, 'El nombre no puede superar los 200 caracteres.'),
    tipoCompetencia: z.enum(['Combate', 'Formas'], { message: 'Elegí el tipo de competencia.' }),
    sexo: z.enum(['M', 'F'], { message: 'Elegí el sexo de la categoría.' }),
    rangoEdadMin: z.number().int().min(0, 'La edad mínima no puede ser negativa.').optional(),
    rangoEdadMax: z.number().int().min(0, 'La edad máxima no puede ser negativa.').optional(),
    rangoPesoMin: z.number().positive('El peso mínimo debe ser mayor a 0.').optional(),
    rangoPesoMax: z.number().positive('El peso máximo debe ser mayor a 0.').optional(),
    rangoGraduacionMin: z.enum(GRADUACIONES, { message: 'Elegí la graduación mínima.' }),
    rangoGraduacionMax: z.enum(GRADUACIONES, { message: 'Elegí la graduación máxima.' }),
  })
  // Edad: si ambos extremos existen, el máximo debe ser mayor (no iguales).
  .refine((v) => v.rangoEdadMin == null || v.rangoEdadMax == null || v.rangoEdadMax > v.rangoEdadMin, {
    path: ['rangoEdadMax'],
    message: 'La edad máxima debe ser mayor que la mínima (no pueden ser iguales).',
  })
  // Peso: solo se valida en Combate; si ambos existen, el máximo debe ser mayor (no iguales).
  .refine(
    (v) =>
      v.tipoCompetencia !== 'Combate' ||
      v.rangoPesoMin == null ||
      v.rangoPesoMax == null ||
      v.rangoPesoMax > v.rangoPesoMin,
    {
      path: ['rangoPesoMax'],
      message: 'El peso máximo debe ser mayor que el mínimo (no pueden ser iguales).',
    },
  )
  .refine((v) => indiceGraduacion(v.rangoGraduacionMax) >= indiceGraduacion(v.rangoGraduacionMin), {
    path: ['rangoGraduacionMax'],
    message: 'La graduación máxima no puede ser menor que la mínima.',
  });

type CategoriaFormValues = z.infer<typeof categoriaSchema>;

// Valores iniciales del formulario a partir de una categoría existente (modo edición).
function valoresDesdeCategoria(categoria: Categoria): CategoriaFormValues {
  return {
    nombre: categoria.nombre,
    tipoCompetencia: categoria.tipoCompetencia,
    sexo: categoria.sexo,
    rangoEdadMin: categoria.rangoEdadMin ?? undefined,
    rangoEdadMax: categoria.rangoEdadMax ?? undefined,
    rangoPesoMin: categoria.rangoPesoMin ?? undefined,
    rangoPesoMax: categoria.rangoPesoMax ?? undefined,
    rangoGraduacionMin: categoria.rangoGraduacionMin as CategoriaFormValues['rangoGraduacionMin'],
    rangoGraduacionMax: categoria.rangoGraduacionMax as CategoriaFormValues['rangoGraduacionMax'],
  };
}

/**
 * Formulario de categoría reutilizable. Sin `categoria` crea una nueva; con `categoria`
 * precarga los datos y edita la existente.
 */
export function CategoriaForm({ torneoId, categoria }: { torneoId: string; categoria?: Categoria }) {
  const navigate = useNavigate();
  const esEdicion = Boolean(categoria);

  const crear = useCrearCategoria(torneoId);
  const editar = useEditarCategoria(torneoId, categoria?.id ?? '');
  const { mutate, isPending } = esEdicion ? editar : crear;

  const {
    register,
    handleSubmit,
    reset,
    resetField,
    watch,
    formState: { errors },
  } = useForm<CategoriaFormValues>({
    resolver: zodResolver(categoriaSchema),
    defaultValues: categoria
      ? valoresDesdeCategoria(categoria)
      : {
          tipoCompetencia: 'Combate',
          sexo: 'M',
          rangoGraduacionMin: 'CinturonBlanco',
          rangoGraduacionMax: 'CinturonBlanco',
        },
  });

  const esFormas = watch('tipoCompetencia') === 'Formas';

  // En Formas el peso no aplica: limpiamos los campos para no arrastrar valores
  // ocultos (que podrían bloquear el submit con un error invisible).
  useEffect(() => {
    if (esFormas) {
      resetField('rangoPesoMin');
      resetField('rangoPesoMax');
    }
  }, [esFormas, resetField]);

  const onSubmit = (values: CategoriaFormValues) => {
    // En Formas el peso no aplica: se descarta antes de enviar.
    const payload = {
      ...values,
      rangoPesoMin: esFormas ? undefined : values.rangoPesoMin,
      rangoPesoMax: esFormas ? undefined : values.rangoPesoMax,
    };
    mutate(payload, {
      onSuccess: (guardada) => {
        if (esEdicion) {
          toast.success(`Categoría "${guardada.nombre}" actualizada correctamente.`);
          navigate(`/torneos/${torneoId}/categorias`);
        } else {
          toast.success(`Categoría "${guardada.nombre}" creada correctamente.`);
          reset();
        }
      },
      onError: (error) => {
        const fallback = esEdicion ? 'No se pudo actualizar la categoría.' : 'No se pudo crear la categoría.';
        toast.error(error instanceof ApiError ? error.message : fallback);
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
          <select id="sexo" className={selectClass} aria-invalid={Boolean(errors.sexo)} {...register('sexo')}>
            <option value="M">Masculino</option>
            <option value="F">Femenino</option>
          </select>
        </div>
      </div>

      <fieldset className="flex flex-col gap-2">
        <legend className="mb-1 text-sm leading-relaxed font-medium">Rango de edad (años)</legend>
        <p className="mb-2 text-xs leading-relaxed text-muted-foreground">
          Dejá un campo vacío para rangos abiertos (ej. solo máximo = “hasta X”; solo mínimo = “X en adelante”).
        </p>
        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoEdadMin">Edad mínima (desde)</Label>
            <Input
              id="rangoEdadMin"
              type="number"
              min={0}
              className="min-h-11"
              placeholder="Sin mínimo"
              aria-invalid={Boolean(errors.rangoEdadMin)}
              aria-describedby={errors.rangoEdadMin ? 'edad-min-error' : undefined}
              {...register('rangoEdadMin', { setValueAs: aNumeroOpcional })}
            />
            {errors.rangoEdadMin && (
              <p id="edad-min-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoEdadMin.message}
              </p>
            )}
          </div>
          <div className="flex flex-col gap-2">
            <Label htmlFor="rangoEdadMax">Edad máxima (hasta)</Label>
            <Input
              id="rangoEdadMax"
              type="number"
              min={0}
              className="min-h-11"
              placeholder="Sin máximo"
              aria-invalid={Boolean(errors.rangoEdadMax)}
              aria-describedby={errors.rangoEdadMax ? 'edad-max-error' : undefined}
              {...register('rangoEdadMax', { setValueAs: aNumeroOpcional })}
            />
            {errors.rangoEdadMax && (
              <p id="edad-max-error" className="text-sm leading-relaxed text-destructive">
                {errors.rangoEdadMax.message}
              </p>
            )}
          </div>
        </div>
      </fieldset>

      {!esFormas && (
        <fieldset className="flex flex-col gap-2">
          <legend className="mb-1 text-sm leading-relaxed font-medium">Rango de peso (kg)</legend>
          <p className="mb-2 text-xs leading-relaxed text-muted-foreground">
            Dejá un campo vacío para rangos abiertos (ej. “hasta 50 kg” o “80 kg en adelante”).
          </p>
          <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
            <div className="flex flex-col gap-2">
              <Label htmlFor="rangoPesoMin">Peso mínimo (desde)</Label>
              <Input
                id="rangoPesoMin"
                type="number"
                min={0}
                step="0.01"
                className="min-h-11"
                placeholder="Sin mínimo"
                aria-invalid={Boolean(errors.rangoPesoMin)}
                aria-describedby={errors.rangoPesoMin ? 'peso-min-error' : undefined}
                {...register('rangoPesoMin', { setValueAs: aNumeroOpcional })}
              />
              {errors.rangoPesoMin && (
                <p id="peso-min-error" className="text-sm leading-relaxed text-destructive">
                  {errors.rangoPesoMin.message}
                </p>
              )}
            </div>
            <div className="flex flex-col gap-2">
              <Label htmlFor="rangoPesoMax">Peso máximo (hasta)</Label>
              <Input
                id="rangoPesoMax"
                type="number"
                min={0}
                step="0.01"
                className="min-h-11"
                placeholder="Sin máximo"
                aria-invalid={Boolean(errors.rangoPesoMax)}
                aria-describedby={errors.rangoPesoMax ? 'peso-max-error' : undefined}
                {...register('rangoPesoMax', { setValueAs: aNumeroOpcional })}
              />
              {errors.rangoPesoMax && (
                <p id="peso-max-error" className="text-sm leading-relaxed text-destructive">
                  {errors.rangoPesoMax.message}
                </p>
              )}
            </div>
          </div>
        </fieldset>
      )}

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
        {isPending
          ? esEdicion
            ? 'Guardando cambios…'
            : 'Creando categoría…'
          : esEdicion
            ? 'Guardar cambios'
            : 'Crear categoría'}
      </Button>
    </form>
  );
}

# Skill: Schema de Base de Datos

> Leer antes de crear o modificar migraciones de EF Core.

## Schema Completo (PostgreSQL)

```sql
-- ============================
-- TORNEOS
-- ============================
CREATE TABLE torneos (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nombre          VARCHAR(200) NOT NULL,
    fecha           DATE NOT NULL,
    lugar           VARCHAR(300) NOT NULL,
    imagen_flyer    VARCHAR(500),           -- URL o path del flyer
    estado          VARCHAR(20) NOT NULL DEFAULT 'Borrador',  -- Borrador | Activo | Finalizado
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================
-- CATEGORIAS
-- ============================
CREATE TABLE categorias (
    id                      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    torneo_id               UUID NOT NULL REFERENCES torneos(id) ON DELETE CASCADE,
    nombre                  VARCHAR(200) NOT NULL,
    tipo_competencia        VARCHAR(20) NOT NULL,  -- Combate | Formas
    sexo                    CHAR(1) NOT NULL,       -- M | F
    rango_edad_min          INT NOT NULL,
    rango_edad_max          INT NOT NULL,
    rango_peso_min          DECIMAL(5,2) NOT NULL,
    rango_peso_max          DECIMAL(5,2) NOT NULL,
    rango_graduacion_min    VARCHAR(50) NOT NULL,   -- e.g. "CinturonAmarillo"
    rango_graduacion_max    VARCHAR(50) NOT NULL,
    llaves_generadas        BOOLEAN NOT NULL DEFAULT FALSE,
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at              TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================
-- COMPETIDORES
-- ============================
CREATE TABLE competidores (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    torneo_id       UUID NOT NULL REFERENCES torneos(id) ON DELETE CASCADE,
    -- categoria_id es NULLABLE: el competidor se carga sin categoría (H0004) y se le asigna
    -- una al armar las llaves (H0005), clasificándolo por sus atributos. ON DELETE SET NULL.
    categoria_id    UUID REFERENCES categorias(id) ON DELETE SET NULL,
    nombre          VARCHAR(100) NOT NULL,
    apellido        VARCHAR(100) NOT NULL,
    sexo            CHAR(1) NOT NULL,       -- M | F (se cruza con categorias.sexo al clasificar)
    edad            INT NOT NULL,
    graduacion      VARCHAR(50) NOT NULL,   -- e.g. "CinturonNegro1Dan"
    peso            DECIMAL(5,2) NOT NULL,  -- kg
    altura          DECIMAL(4,2) NOT NULL,  -- metros
    escuela         VARCHAR(200) NOT NULL,
    responsable     VARCHAR(200) NOT NULL,
    telefono        VARCHAR(20),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================
-- MATCHES (LLAVES)
-- ============================
CREATE TABLE matches (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    categoria_id        UUID NOT NULL REFERENCES categorias(id) ON DELETE CASCADE,
    ronda               INT NOT NULL,       -- 1 = primera ronda, aumenta hacia la final
    posicion            INT NOT NULL,       -- posición dentro de la ronda (0-indexed)
    competidor1_id      UUID REFERENCES competidores(id),
    competidor2_id      UUID REFERENCES competidores(id),
    ganador_id          UUID REFERENCES competidores(id),
    estado              VARCHAR(20) NOT NULL DEFAULT 'Pendiente',  -- Pendiente | Bye | EnCurso | Finalizado
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),

    UNIQUE(categoria_id, ronda, posicion)   -- no puede haber dos matches en misma posición
);

-- ============================
-- USUARIOS (Auth básico)
-- ============================
CREATE TABLE usuarios (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email           VARCHAR(300) NOT NULL UNIQUE,
    password_hash   VARCHAR(500) NOT NULL,
    nombre          VARCHAR(200) NOT NULL,
    rol             VARCHAR(20) NOT NULL,   -- Coordinador | Profesor | Director
    escuela         VARCHAR(200),           -- solo para Profesores
    activo          BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ============================
-- ÍNDICES
-- ============================
CREATE INDEX idx_categorias_torneo_id ON categorias(torneo_id);
CREATE INDEX idx_competidores_torneo_id ON competidores(torneo_id);
CREATE INDEX idx_competidores_categoria_id ON competidores(categoria_id);
CREATE INDEX idx_competidores_escuela ON competidores(escuela);
CREATE INDEX idx_matches_categoria_id ON matches(categoria_id);
CREATE INDEX idx_matches_ronda ON matches(categoria_id, ronda);
```

## Enumeraciones (Graduaciones de Taekwondo)
```csharp
public enum Graduacion
{
    // Cinturones de color (Geup)
    CinturonBlanco = 0,
    CinturonBlancoPtaAmarilla = 1,
    CinturonAmarillo = 2,
    CinturonAmarilloPtaVerde = 3,
    CinturonVerde = 4,
    CinturonVerdePtaAzul = 5,
    CinturonAzul = 6,
    CinturonAzulPtaRoja = 7,
    CinturonRojo = 8,
    CinturonRojoPtaNegra = 9,
    // Cinturones negros (Dan)
    CinturonNegro1Dan = 10,
    CinturonNegro2Dan = 11,
    CinturonNegro3Dan = 12,
    CinturonNegro4Dan = 13,
    CinturonNegro5Dan = 14,
    CinturonNegro6Dan = 15
}
```

## Consideraciones del Schema

### Integridad Referencial
- Al eliminar un Torneo → eliminación en cascada de Categorías, Competidores y Matches
- Al eliminar una Categoría → eliminación en cascada de Competidores y Matches
- Un Competidor puede aparecer en múltiples Matches (como competidor1, competidor2, o ganador)

### Estados del Torneo
```
Borrador → Activo → Finalizado
```
- En **Borrador**: se pueden agregar/editar categorías y competidores
- En **Activo**: se generan llaves y se registran resultados
- En **Finalizado**: solo lectura, se pueden generar reportes

### El flag `llaves_generadas` en Categoría
- Previene regenerar llaves una vez generadas (regla de negocio)
- Si se necesita regenerar, debe eliminarse el bracket existente (operación administrativa)

## Seed Data (Datos de Prueba)
```sql
-- Torneo de prueba
INSERT INTO torneos (id, nombre, fecha, lugar, estado)
VALUES (
    'a1b2c3d4-0000-0000-0000-000000000001',
    'Torneo Nacional AATEE 2025',
    '2025-11-30',
    'Microestadio San José Maristas de Morón',
    'Activo'
);

-- Categoría de prueba
INSERT INTO categorias (torneo_id, nombre, tipo_competencia, sexo,
    rango_edad_min, rango_edad_max, rango_peso_min, rango_peso_max,
    rango_graduacion_min, rango_graduacion_max)
VALUES (
    'a1b2c3d4-0000-0000-0000-000000000001',
    'Adultos A - Combate Femenino',
    'Combate', 'F',
    18, 35,
    50.00, 55.00,
    'CinturonAmarillo', 'CinturonVerde'
);
```

## EF Core — Naming Conventions
```csharp
// En ApplicationDbContext usar Underscore naming convention:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.UseSnakeCaseNamingConvention(); // Paquete: EFCore.NamingConventions
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TorneoConfiguration).Assembly);
}
```

namespace TorneoDeportivo.Domain.Enums;

/// <summary>
/// Graduaciones (cinturones) de Taekwondo ordenadas de menor a mayor. El valor numérico refleja el orden
/// jerárquico y se usa para validar que un rango de categoría tenga la graduación mínima antes que la máxima.
/// </summary>
public enum Graduacion
{
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
    CinturonNegro1Dan = 10,
    CinturonNegro2Dan = 11,
    CinturonNegro3Dan = 12,
    CinturonNegro4Dan = 13,
    CinturonNegro5Dan = 14,
    CinturonNegro6Dan = 15
}

namespace TorneoDeportivo.Infrastructure;

/// <summary>
/// Opciones de configuración para la conexión al servidor de almacenamiento de objetos MinIO/S3
/// (usado para las imágenes de flyers de los torneos).
/// </summary>
public class MinioOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = false;
}

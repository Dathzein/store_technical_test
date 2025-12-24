namespace ServerCloudStore.Transversal.Common;

/// <summary>
/// Clase genérica de respuesta estándar para todas las operaciones de la API
/// </summary>
/// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
public class Response<T>
{
    /// <summary>
    /// Datos de la respuesta
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Código de respuesta HTTP o código de estado personalizado
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Mensaje descriptivo de la respuesta
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Crea una respuesta exitosa
    /// </summary>
    public static Response<T> Success(T data, string message = "Operación exitosa", int code = 200)
    {
        return new Response<T>
        {
            Data = data,
            Code = code,
            Message = message,
            IsSuccess = true
        };
    }

    /// <summary>
    /// Crea una respuesta de error
    /// </summary>
    public static Response<T> Error(string message, int code = 400)
    {
        return new Response<T>
        {
            Data = default,
            Code = code,
            Message = message,
            IsSuccess = false
        };
    }
}


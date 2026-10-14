using System.Globalization;
using PilasCompartidas;
using CalculadoraPilas;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
var builder = WebApplication.CreateBuilder(args);
// Loopback only by default: no network service or external account required.
if (string.IsNullOrEmpty(builder.Configuration["urls"]))
    builder.WebHost.UseUrls("http://127.0.0.1:5222");
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/pila", (PeticionPila r) =>
{
    if (r.Tipo is not ("arreglo" or "enlazada") || r.Elementos is null
        || r.Elementos.Length > 512 || r.Elementos.Any(v => v is null || v.Length > 80))
        return Results.BadRequest(new { error = "Estado de pila inválido (máximo 512 elementos y 80 caracteres por dato)." });
    IPila<string> pila = r.Tipo == "arreglo" ? new PilaArreglo<string>(5) : new PilaEnlazada<string>();
    try
    {
        // Browser sends top first; rebuilding in reverse preserves LIFO.
        foreach (var value in r.Elementos.Reverse()) pila.Push(value);
        string message;
        switch (r.Operacion)
        {
            case "push":
                if (string.IsNullOrWhiteSpace(r.Dato) || r.Dato.Length > 80)
                    return Results.BadRequest(new { error = "Escribe un dato de 1 a 80 caracteres." });
                if (pila.Count == 512) return Results.BadRequest(new { error = "Esta demostración admite hasta 512 elementos." });
                pila.Push(r.Dato.Trim()); message = $"Push: se insertó «{r.Dato.Trim()}» en el tope."; break;
            case "pop": message = $"Pop: se retiró «{pila.Pop()}», el último dato que entró."; break;
            case "peek": message = $"Peek: el tope es «{pila.Peek()}». La pila no cambió."; break;
            case "clear": pila.Clear(); message = "Clear: la pila quedó vacía."; break;
            case "estado": message = pila.IsEmpty ? "La pila está vacía." : $"La pila contiene {pila.Count} elementos."; break;
            default: return Results.BadRequest(new { error = "Operación desconocida." });
        }
        return Results.Ok(new { elementos = pila.ToArray(), count = pila.Count, capacidad = pila.Capacity,
            vacia = pila.IsEmpty, llena = pila.Capacity is int c && pila.Count == c, mensaje = message });
    }
    catch (InvalidOperationException e) { return Results.BadRequest(new { error = e.Message }); }
});
app.MapPost("/api/calcular", (PeticionExpresion r) =>
{
    if (string.IsNullOrWhiteSpace(r.Expresion) || r.Expresion.Length > 512)
        return Results.BadRequest(new { error = "Escribe una expresión de 1 a 512 caracteres." });
    try
    {
        var result = Calculadora.Resolver(r.Expresion);
        return Results.Ok(new { infija = result.Infija, postfija = result.Postfija,
            valor = result.Valor.ToString(CultureInfo.InvariantCulture), conversion = result.Conversion,
            evaluacion = result.Evaluacion });
    }
    catch (Exception e) when (e is FormatException or DivideByZeroException or OverflowException)
    { return Results.BadRequest(new { error = e.Message }); }
});
app.MapGet("/api/salud", () => Results.Ok(new { estado = "listo" }));
app.Run();
record PeticionPila(string Tipo, string Operacion, string[] Elementos, string? Dato);
record PeticionExpresion(string Expresion);

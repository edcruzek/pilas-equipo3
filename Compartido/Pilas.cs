namespace PilasCompartidas;
public interface IPila<T>
{
    int Count { get; }
    bool IsEmpty { get; }
    int? Capacity { get; }
    void Push(T value);
    T Pop();
    T Peek();
    void Clear();
    T[] ToArray(); // Orden de cima a fondo.
}
public sealed class PilaArreglo<T> : IPila<T>
{
    private readonly T[] datos;
    public int Count { get; private set; }
    public int? Capacity => datos.Length;
    public bool IsEmpty => Count == 0;
    public bool IsFull => Count == datos.Length;
    public PilaArreglo(int capacidad)
    {
        if (capacidad <= 0) throw new ArgumentOutOfRangeException(nameof(capacidad));
        datos = new T[capacidad];
    }
    public void Push(T value)
    {
        if (IsFull) throw new InvalidOperationException("Pila llena: overflow.");
        datos[Count++] = value;
    }
    public T Pop()
    {
        if (IsEmpty) throw new InvalidOperationException("Pila vacía: underflow.");
        T value = datos[--Count];
        datos[Count] = default!; // Liberar la referencia al elemento retirado.
        return value;
    }
    public T Peek()
    {
        if (IsEmpty) throw new InvalidOperationException("Pila vacía: underflow.");
        return datos[Count - 1];
    }
    public void Clear() { Array.Clear(datos, 0, Count); Count = 0; }
    public T[] ToArray()
    {
        T[] result = new T[Count];
        for (int i = 0; i < Count; i++) result[i] = datos[Count - 1 - i];
        return result;
    }
}
public sealed class PilaEnlazada<T> : IPila<T>
{
    private sealed class Nodo(T dato, Nodo? siguiente)
    { public T Dato = dato; public Nodo? Siguiente = siguiente; }
    private Nodo? cima;
    public int Count { get; private set; }
    public int? Capacity => null; // Sin límite fijo; depende de la memoria disponible.
    public bool IsEmpty => cima is null;
    public void Push(T value) { cima = new Nodo(value, cima); Count++; }
    public T Pop()
    {
        if (cima is null) throw new InvalidOperationException("Pila vacía: underflow.");
        T result = cima.Dato; cima = cima.Siguiente; Count--; return result;
    }
    public T Peek() => cima is null ? throw new InvalidOperationException("Pila vacía: underflow.") : cima.Dato;
    public void Clear() { cima = null; Count = 0; }
    public T[] ToArray()
    {
        T[] result = new T[Count]; int i = 0;
        for (Nodo? p = cima; p is not null; p = p.Siguiente) result[i++] = p.Dato;
        return result;
    }
}

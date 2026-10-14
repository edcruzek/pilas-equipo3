using PilasCompartidas;
using System.Text.Json;
Console.OutputEncoding = System.Text.Encoding.UTF8;
if (args.Contains("--test")) { Pruebas.Ejecutar(); return; }
if (args.Contains("--demo"))
{
    IPila<int> p = new PilaArreglo<int>(5);
    var rows = new List<object>();
    foreach (int v in new[]{10,20,30}) { p.Push(v); rows.Add(new{operacion=$"Push({v})",cima=p.Peek(),count=p.Count,pila=p.ToArray()}); }
    int peek = p.Peek(); rows.Add(new{operacion="Peek()",cima=peek,count=p.Count,pila=p.ToArray()});
    int removed = p.Pop(); rows.Add(new{operacion=$"Pop() devuelve {removed}",cima=p.Peek(),count=p.Count,pila=p.ToArray()});
    Console.WriteLine(JsonSerializer.Serialize(rows,new JsonSerializerOptions{WriteIndented=true}));
    return;
}
Console.WriteLine("LABORATORIO DE PILAS\n1. Arreglo fijo (capacidad 5)\n2. Lista enlazada");
IPila<string> pila = Console.ReadLine()=="2" ? new PilaEnlazada<string>() : new PilaArreglo<string>(5);
while (true)
{
    Console.WriteLine($"\nCima a fondo: [{string.Join(", ",pila.ToArray())}]  Tamaño: {pila.Count}");
    Console.WriteLine("1 Push  2 Pop  3 Peek  4 Clear  5 Estado  6 Ejemplo Stack<T>  0 Salir");
    string? option = Console.ReadLine(); if (option is null || option=="0") break;
    try
    {
        switch(option)
        {
            case "1": Console.Write("Dato: "); string? value=Console.ReadLine(); if(value is not null) pila.Push(value); break;
            case "2": Console.WriteLine($"Extraído: {pila.Pop()}"); break;
            case "3": Console.WriteLine($"Cima sin retirar: {pila.Peek()}"); break;
            case "4": pila.Clear(); Console.WriteLine("Pila vaciada."); break;
            case "5": Console.WriteLine($"Vacía: {pila.IsEmpty}; capacidad: {pila.Capacity?.ToString() ?? "dinámica"}; llena: {(pila.Capacity is int c ? (pila.Count==c).ToString() : "sin límite fijo")}"); break;
            case "6": var nativa=new Stack<int>(); nativa.Push(10);nativa.Push(20); Console.WriteLine($"Stack<int>: Peek={nativa.Peek()}, Pop={nativa.Pop()}, Count={nativa.Count}"); break;
            default: Console.WriteLine("Opción no reconocida."); break;
        }
    }
    catch(InvalidOperationException e) { Console.WriteLine(e.Message); }
}
static class Pruebas
{
    static int checks;
    static void Check(bool condition,string name) { if(!condition) throw new Exception(name); checks++; }
    static void Throws(Action action) { bool raised=false;try{action();}catch(InvalidOperationException){raised=true;} Check(raised,"Error esperado"); }
    public static void Ejecutar()
    {
        foreach(IPila<int> p in new IPila<int>[]{new PilaArreglo<int>(1024),new PilaEnlazada<int>()})
        {
            Check(p.IsEmpty,"Inicio vacío"); Throws(()=>p.Pop());Throws(()=>p.Peek());
            var rng=new Random(24);var referencia=new Stack<int>();
            for(int i=0;i<600;i++)
            {
                if(referencia.Count==0 || rng.Next(3)>0) { int v=rng.Next(100);p.Push(v);referencia.Push(v); }
                else Check(p.Pop()==referencia.Pop(),"Comparación Pop");
                Check(p.Count==referencia.Count,"Count");Check(p.ToArray().SequenceEqual(referencia.ToArray()),"Orden");
                if(referencia.Count>0){int n=p.Count;Check(p.Peek()==referencia.Peek() && p.Count==n,"Peek no destructivo");}
            }
            p.Clear();Check(p.IsEmpty && p.Count==0,"Clear");p.Push(8);Check(p.Pop()==8,"Reutilización");
        }
        var fija=new PilaArreglo<string>(2); fija.Push("A");fija.Push("B");Throws(()=>fija.Push("C"));
        Check(fija.Count==2 && fija.Peek()=="B" && fija.IsFull,"Overflow conserva estado");
        Check(fija.Pop()=="B" && fija.Pop()=="A","Genéricos string");
        var ligada=new PilaEnlazada<string>();ligada.Push("igual");ligada.Push("igual");Check(ligada.Count==2,"Duplicados");
        Console.WriteLine($"PASS LaboratorioPilas: {checks} comprobaciones, incluidas secuencias aleatorias frente a Stack<T>.");
    }
}

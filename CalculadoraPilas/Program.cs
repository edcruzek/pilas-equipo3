using CalculadoraPilas;
using System.Text.Json;
Console.OutputEncoding=System.Text.Encoding.UTF8;
System.Globalization.CultureInfo.CurrentCulture=System.Globalization.CultureInfo.InvariantCulture;
if(args.Contains("--test")){Pruebas.Ejecutar();return;}
if(args.Contains("--demo"))
{
    var examples=new[]{"(6+2)*5-8/4","2+3*4","(2+3)*4","10-3-2","12.5/2.5+1"}.Select(Calculadora.Resolver).ToArray();
    Console.WriteLine(JsonSerializer.Serialize(examples,new JsonSerializerOptions{WriteIndented=true}));return;
}
Console.WriteLine("CALCULADORA CON PILAS\nNúmeros no negativos, decimales con punto, + - * / y paréntesis. Escribe salir para terminar.");
while(true)
{
    Console.Write("\nInfija: ");string? line=Console.ReadLine();if(line is null || line.Trim().Equals("salir",StringComparison.OrdinalIgnoreCase))break;
    try
    {
        Resultado r=Calculadora.Resolver(line);Console.WriteLine($"Postfija: {r.Postfija}\nResultado: {r.Valor}\nCONVERSIÓN (cima primero)");
        foreach(var s in r.Conversion)Console.WriteLine($"{s.Token,-6} pila=[{s.PilaCimaPrimero}] salida=[{s.Salida}]");
        Console.WriteLine("EVALUACIÓN (cima primero)");foreach(var s in r.Evaluacion)Console.WriteLine($"{s.Token,-6} {s.Accion} pila=[{s.PilaCimaPrimero}]");
    }
    catch(Exception e) when(e is FormatException or DivideByZeroException or OverflowException){Console.WriteLine($"Entrada rechazada: {e.Message}");}
}
static class Pruebas
{
    static int count;
    static void Check(bool ok,string name){if(!ok)throw new Exception(name);count++;}
    static void Reject(string input){bool rejected=false;try{Calculadora.Resolver(input);}catch(Exception e)when(e is FormatException or DivideByZeroException or OverflowException){rejected=true;}Check(rejected,$"Debía rechazar {input}");}
    public static void Ejecutar()
    {
        (string input,decimal expected)[] cases={ ("(6+2)*5-8/4",38m),("2+3*4",14m),("(2+3)*4",20m),("10-3-2",5m),("20/2/5",2m),("12.5/2.5+1",6m),("2-5",-3m),("0",0m),("((8))",8m),("1/4",0.25m),("2*(3+4*(5-2))",30m)};
        foreach(var c in cases)Check(Calculadora.Resolver(c.input).Valor==c.expected,c.input);
        Check(Calculadora.Resolver("(6+2)*5-8/4").Postfija=="6 2 + 5 * 8 4 / -","Postfija esperada");
        foreach(string input in new[]{"","()","(2+3","2+3)","2+","+2","-2","2**3","2 3","2(3)","a+2","2^3","1/0","1.2.3+2",".5+1","5.+1","2+-3","(2+)"})Reject(input);
        foreach(string[] invalid in new[]{new[]{"2","+"},new[]{"2","3"},Array.Empty<string>(),new[]{"2","3","^"}})
        {bool raised=false;try{Calculadora.EvaluarPostfija(invalid);}catch(FormatException){raised=true;}Check(raised,"Postfija inválida");}
        var rng=new Random(24);
        for(int i=0;i<150;i++){int a=rng.Next(30),b=rng.Next(30),c=rng.Next(30);Check(Calculadora.Resolver($"{a}+{b}*{c}").Valor==a+b*c,"Precedencia aleatoria");Check(Calculadora.Resolver($"({a}+{b})*{c}").Valor==(a+b)*c,"Paréntesis aleatorios");}
        Console.WriteLine($"PASS CalculadoraPilas: {count} casos, incluidos entradas inválidas y cálculo independiente de expresiones generadas.");
    }
}

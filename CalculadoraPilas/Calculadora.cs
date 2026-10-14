using System.Globalization;
using PilasCompartidas;
namespace CalculadoraPilas;
public record Paso(string Token, string Accion, string PilaCimaPrimero, string Salida);
public record Resultado(string Infija,string Postfija,decimal Valor,List<Paso> Conversion,List<Paso> Evaluacion);
public static class Calculadora
{
    static bool Numero(string token) => decimal.TryParse(token,NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out _);
    static int Prioridad(string token)=>token is "*" or "/"?2:1;
    static string Mostrar<T>(IPila<T> p)=>string.Join(" ",p.ToArray());
    public static List<string> Tokenizar(string text)
    {
        var result=new List<string>();
        for(int i=0;i<text.Length;)
        {
            char ch=text[i];if(char.IsWhiteSpace(ch)){i++;continue;}
            if(ch>='0' && ch<='9')
            {
                int start=i++;while(i<text.Length && ((text[i]>='0' && text[i]<='9') || text[i]=='.')) i++;
                string token=text[start..i];
                if(!Numero(token) || token.EndsWith('.')) throw new FormatException("Número inválido; usa decimales con punto y dígitos a ambos lados.");
                result.Add(token);
            }
            else if("+-*/()".Contains(ch)) {result.Add(ch.ToString());i++;}
            else throw new FormatException($"Carácter no admitido: {ch}");
        }
        if(result.Count==0)throw new FormatException("Expresión vacía.");
        return result;
    }
    public static (List<string> Tokens,List<Paso> Pasos) Convertir(string input)
    {
        var output=new List<string>();var steps=new List<Paso>();var operators=new PilaEnlazada<string>();bool needOperand=true;
        foreach(string token in Tokenizar(input))
        {
            string action;
            if(Numero(token))
            { if(!needOperand)throw new FormatException("Falta un operador entre operandos.");output.Add(token);needOperand=false;action="Enviar número a la salida"; }
            else if(token=="(")
            { if(!needOperand)throw new FormatException("Falta un operador antes del paréntesis.");operators.Push(token);action="Push de apertura"; }
            else if(token==")")
            {
                if(needOperand)throw new FormatException("Paréntesis vacío u operando faltante.");
                while(!operators.IsEmpty && operators.Peek()!="(")output.Add(operators.Pop());
                if(operators.IsEmpty)throw new FormatException("Cierre sin apertura.");operators.Pop();action="Retirar operadores hasta la apertura";
            }
            else
            {
                if(needOperand)throw new FormatException("Operando faltante; el signo unario no está admitido.");
                while(!operators.IsEmpty && operators.Peek()!="(" && Prioridad(operators.Peek())>=Prioridad(token))output.Add(operators.Pop());
                operators.Push(token);needOperand=true;action="Retirar precedencia mayor o igual y hacer Push";
            }
            steps.Add(new Paso(token,action,Mostrar(operators),string.Join(" ",output)));
        }
        if(needOperand)throw new FormatException("La expresión termina sin operando.");
        while(!operators.IsEmpty){string op=operators.Pop();if(op=="(")throw new FormatException("Apertura sin cierre.");output.Add(op);}
        steps.Add(new Paso("FIN","Vaciar operadores",Mostrar(operators),string.Join(" ",output)));
        return (output,steps);
    }
    public static (decimal Valor,List<Paso> Pasos) EvaluarPostfija(IEnumerable<string> tokens)
    {
        var stack=new PilaEnlazada<decimal>();var steps=new List<Paso>();
        foreach(string token in tokens)
        {
            string action;
            if(Numero(token)){decimal value=decimal.Parse(token,CultureInfo.InvariantCulture);stack.Push(value);action="Push de operando";}
            else
            {
                if(token is not ("+" or "-" or "*" or "/"))throw new FormatException("Operador postfijo desconocido.");
                if(stack.Count<2)throw new FormatException("Faltan operandos en la postfija.");
                decimal right=stack.Pop(),left=stack.Pop();
                decimal value=token switch {"+"=>left+right,"-"=>left-right,"*"=>left*right,"/"=>right==0?throw new DivideByZeroException("División entre cero."):left/right,_=>throw new FormatException()};
                stack.Push(value); // Paso indispensable: el resultado vuelve a la pila.
                action=$"Pop derecho={right}; Pop izquierdo={left}; Push resultado={value}";
            }
            steps.Add(new Paso(token,action,Mostrar(stack),""));
        }
        if(stack.Count!=1)throw new FormatException("La postfija debe terminar con un único resultado.");
        return(stack.Pop(),steps);
    }
    public static Resultado Resolver(string input)
    { var conversion=Convertir(input);var evaluation=EvaluarPostfija(conversion.Tokens);return new Resultado(input,string.Join(" ",conversion.Tokens),evaluation.Valor,conversion.Pasos,evaluation.Pasos); }
}

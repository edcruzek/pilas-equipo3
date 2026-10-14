# Pilas · Equipo 3

Proyecto didáctico del tema 2.2 de Estructuras de datos. Un panel gráfico permite
acceder al **Laboratorio de pilas** y la **Calculadora con pilas**.

## Demostración en línea

[**Abrir el laboratorio y la calculadora**](https://pilas-equipo-tres.grittyplume257.chatgpt.site).
No necesita instalar .NET: esta demostración utiliza JavaScript en el navegador.
Los proyectos C# originales y las instrucciones de uso local siguen debajo.

## Ejecutar la aplicación gráfica

Se necesita **SDK .NET 10** y un navegador. No hay paquetes NuGet externos,
cuentas, servicios remotos ni conexión a Internet necesaria durante el uso.

Después de clonar o descargar el repositorio, abrir una terminal en su carpeta:

```powershell
dotnet run --project PanelPilas
```

Abrir **http://127.0.0.1:5222** y elegir un módulo. Mantener la terminal abierta;
**Ctrl+C** detiene el servidor. Recargar la página reinicia la demostración.

Si el puerto está ocupado:

```powershell
dotnet run --project PanelPilas --urls http://127.0.0.1:5223
```

Abrir entonces http://127.0.0.1:5223.

## Módulos

**Laboratorio:** pila visual con tope y base, arreglo fijo de 5 posiciones o nodos
enlazados. Permite Push, Pop, Peek y Clear; muestra cantidad, estado e historial.
El ejemplo inserta 10, 20 y 30: 30 será el primero que salga.

**Calculadora:** recibe una expresión infija, la convierte a postfija y la evalúa.
Muestra las pilas y las acciones de cada token con Anterior, Siguiente y Reproducir.
Ejemplo: `(6+2)*5-8/4` produce `6 2 + 5 * 8 4 / -` y resultado **38**.

Acepta números no negativos de entrada, decimales con punto, operadores binarios
`+ - * /` y paréntesis. Puede producir resultados negativos. No admite signos
unarios, potencias, variables, funciones ni multiplicación implícita.
La precisión de decimal es finita. El panel limita la expresión a 512 caracteres,
cada dato a 80 caracteres y la demostración enlazada a 512 elementos.

## Código

- `Compartido/Pilas.cs`: pilas genéricas propias con arreglo y nodos.
- `LaboratorioPilas`: operaciones, versión de consola y pruebas.
- `CalculadoraPilas`: conversión, evaluación, versión de consola y pruebas.
- `PanelPilas`: interfaz gráfica común, API C# y HTML/CSS/JavaScript.

El panel comparte las clases originales: el algoritmo no se duplica en JavaScript.
Los dos módulos son los dos proyectos didácticos; PanelPilas los integra.
Más detalles de la interfaz en [PanelPilas/README.md](PanelPilas/README.md).

## Pruebas y demostraciones de consola

```powershell
dotnet run --project LaboratorioPilas -- --test
dotnet run --project CalculadoraPilas -- --test
dotnet run --project LaboratorioPilas -- --demo
dotnet run --project CalculadoraPilas -- --demo
```

Con el panel iniciado, se puede comprobar la API usando Python (opcional):

```powershell
python ProbarPanel.py
```

Pruebas verificadas: 3,996 comprobaciones del laboratorio, 334 casos de la
calculadora y 59 comprobaciones de la API. Las pruebas fallidas devuelven un código
de salida distinto de cero. Python no es necesario para utilizar la aplicación.

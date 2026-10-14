# Panel gráfico de pilas

Aplicación local con dos módulos: Laboratorio de pilas y Calculadora con pilas.
Se necesita SDK .NET 10 y un navegador. No utiliza paquetes NuGet, fuentes remotas,
CDN, cuentas ni conexión a Internet durante el uso.

Desde la carpeta proyectos, ejecutar:

```powershell
dotnet run --project PanelPilas
```

Abrir http://127.0.0.1:5222 en el navegador. Mantener la terminal abierta.
Para detener el servidor: Ctrl+C. Si el puerto está ocupado:

```powershell
dotnet run --project PanelPilas --urls http://127.0.0.1:5223
```

En ese caso abrir la dirección con el puerto 5223.

## Laboratorio

- Elegir arreglo fijo de capacidad 5 o nodos enlazados.
- Escribir un dato y pulsar Insertar; también funciona Enter.
- Retirar, consultar o vaciar con los botones correspondientes.
- Cargar ejemplo inserta 10, 20 y 30: el tope es 30.
- Cada representación conserva su pila mientras la página permanece abierta.
- Cambiar de módulo conserva la demostración. Recargar la página la reinicia.
- El tope es el primer dato ocupado, aunque existan posiciones libres encima en el arreglo.
- Historial: 25 operaciones recientes; datos de hasta 80 caracteres.
- El panel limita la demostración enlazada a 512 elementos; la clase de nodos no tiene un límite fijo.

## Calculadora

1. Escribir una expresión o elegir un ejemplo.
2. Resolver para ver la postfija y el resultado.
3. Elegir Conversión o Evaluación.
4. Siguiente y Anterior muestran los estados después de cada token.
5. Reproducir avanza automáticamente; Pausar detiene la reproducción.
6. Volver al inicio reinicia únicamente los pasos. Reiniciar limpia el cálculo.

Entrada de hasta 512 caracteres: números no negativos con punto decimal,
operadores binarios + - * / y paréntesis. Resultados negativos permitidos.
Sin signo unario, potencias, variables, funciones ni multiplicación implícita.
Los resultados decimal se envían como texto para conservar su precisión finita.

## Organización y comprobaciones

wwwroot contiene HTML, CSS y JavaScript. Program.cs expone /api/pila y /api/calcular.
El panel enlaza los mismos archivos C# que los proyectos de consola: no mantiene
una segunda implementación del algoritmo en JavaScript. La pila del laboratorio
se reconstruye desde su estado en el navegador para cada operación; no hay estado
global compartido entre ventanas. Los costos de la estructura no incluyen esta
reconstrucción, el transporte HTTP o el dibujo de la interfaz.

Las pruebas de las clases siguen en los dos proyectos originales (--test).
Con el panel iniciado, ProbarPanel.py verifica 59 comprobaciones de la API:

```powershell
python ProbarPanel.py
```

Python solo es necesario para esa prueba opcional, no para utilizar la aplicación.
La interfaz fue comprobada en navegador: LIFO, Peek, llenado, cambios de
representación, errores de cálculo, fases, avance/retroceso y reproducción.

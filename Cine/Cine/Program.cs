using System;
using System.Text;
using System.Text.RegularExpressions;
using Cine.Enum;
using Cine.Structs;
Console.OutputEncoding = Encoding.UTF8;

var rand = new Random();
const double Precio = 6.5;
const string LetrasFilas = "ABCDEFG";
const int MinFilas = 4;
const int MaxFilas = 7;
const int MinColumnas = 5;
const int MaxColumnas = 9;
/*
 * Main()
 */
Main(args);

void Main(string[] args) {

    Console.WriteLine("Bienvenido a CINEMAD.");
    if (args.Length > 0) {
        Console.WriteLine($"Has ingresado: {args.Length} argumentos:");
        foreach (var arg in args) {
            Console.WriteLine(arg);
        }
    }  else {
        Console.WriteLine("Has ingresado: No argumentos!");
    }

    Configuracion configuracion = ValidarArgumentos(args);
    var salaMatriz = new Estado[configuracion.Filas, configuracion.Columnas];
    Console.WriteLine($"Configuración cargada: Filas={configuracion.Filas}, Columnas={configuracion.Columnas}");

    InitSala(salaMatriz, configuracion);
    MostrarSala(salaMatriz, configuracion);

    MenuSistema(salaMatriz, configuracion);
}
//END MAIN
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


/*
 * Funcion que valida los argumentos 
 */
Configuracion ValidarArgumentos(string[] args) {
    // Si no tiene la longitud 2 se pide la configuracion manual
    if (args.Length != 2) {
        Console.WriteLine("ERROR: Faltan argumentos. Formato de ejecución: filas:X columnas:Y");
        Console.WriteLine("--- Modo Consola de Respaldo ---");
        return PedirConfiguracion();
    }
    // Si no se puede parsear o es menor que 4 y mayor que 7 se pide de nuevo la configuracion manual
    var filas = args[0].Split(':');
    if (!int.TryParse(filas[1], out var filasParsed) || (filasParsed < MinFilas || filasParsed > MaxFilas)) { 
        Console.WriteLine($"Error: El argumento {args[0]} no es valido. Debe ser un numero de fila entre {MinFilas} y {MaxFilas} incluidos");
        return PedirConfiguracion();
    }
    // Si no se puede parsear o es menor que 5 y mayor que 9 se pide de nuevo la configuracion manual
    var columnas = args[1].Split(':');
    if (!int.TryParse(columnas[1], out var columnasParsed) || (columnasParsed < MinColumnas || columnasParsed > MaxColumnas)) {
        Console.WriteLine($"ERROR: El argumento {args[0]} no es válido o está fuera de rango. Filas permitidas: {MinFilas}-{MaxFilas}.");
        Console.WriteLine("--- Modo Consola de Respaldo ---");
        return PedirConfiguracion();
    }
    //Si esta todo bien lo guardamos en la struct de configuracion
    return new Configuracion {
        Filas = filasParsed,
        Columnas = columnasParsed
    };
}
/*
 * Si los argumentos estan mal llamamos a esta funcion de configuracion manual
 */
Configuracion PedirConfiguracion() {
    Console.WriteLine($"Introduzca las dimensiones de la sala (F:C). Rango permitido: {MinFilas}-{MaxFilas}:{MinColumnas}-{MaxColumnas}");
    var regex = new Regex($"^{MinFilas}-{MaxFilas}:{MinColumnas}-{MaxColumnas}$"); 
    var input = (Console.ReadLine() ?? "").Trim();
    while (!regex.IsMatch(input)) {
        Console.WriteLine($"Error: Entrada inválida. Inténtalo de nuevo. Formato correcto: Fila:{MinFilas}-{MaxFilas} Columnas:{MinColumnas}-{MaxColumnas}");
        input = (Console.ReadLine() ?? "").Trim();
    }

    string[] partes = input.Split(':'); 
    var filas = int.Parse(partes[0]);
    var columnas = int.Parse(partes[1]);
    
    return new Configuracion {
        Filas = filas,
        Columnas = columnas
    };
}
/*
 * Procedimiento que inicializa la matriz
 */
void InitSala(Estado[,] matrix, Configuracion config) {
    //Ponemos primero la butacas no disponemos
    var cantidadNoDisponible = rand.Next(1, 4); 
    int colocados = 0;
    
    while (colocados < cantidadNoDisponible) {
        var f = rand.Next(0, config.Filas);
        var c = rand.Next(0, config.Columnas);

        if (matrix[f, c] != Estado.NoDisponible) {
            matrix[f, c] = Estado.NoDisponible;
            colocados++;
        }
    }
    //Luego colocamos la butaca libre siempre y cuando no sea una butaca no disponible
    for (int i = 0; i < config.Filas; i++) {
        for (int j = 0; j < config.Columnas; j++) {
            if (matrix[i, j] != Estado.NoDisponible) {
                matrix[i, j] = Estado.Libre;
            }
        }
    }
}

void MenuSistema(Estado[,] salaMatriz, Configuracion config) {
    PosicionButaca posicion = new PosicionButaca();
    Console.WriteLine("-- --- Menú CINEMAD ---");
    Console.WriteLine("-- 1: Ver sala");
    Console.WriteLine("-- 2: Comprar Entrada");
    Console.WriteLine("-- 3: Devoler entrada");
    Console.WriteLine("-- 4: Recaudar");
    Console.WriteLine("-- 5: Informar");
    Console.WriteLine("-- 6: Salir");
    int opcion = 0;
    do {
        opcion = LeerEntero("-- Elije una opcion:");
        switch (opcion) {
            case 1:
                MostrarSala(salaMatriz, config);
                break;
            case 2:
                ComprarEntrada(salaMatriz, posicion);
                break;
            case 3:
                DevolverEntrada(salaMatriz, ref posicion);
                break;
            case 4:
                VerRecaudacion(salaMatriz, config);
                break;
            case 5:
                VerIndorme(salaMatriz, config);
                break;
            case 6:
                Console.WriteLine("!Nos vemos fuera de CINEMAD¡ :)");
                break;
            default:
                Console.WriteLine($"Opción inválida. Introduce una de las 6 posibles.");
                break;
        }
    } while (opcion != 6);
    
}

int LeerEntero(string mensase) {
    Console.WriteLine(mensase);
    var regex = new Regex("^[1-6]{1}$");
    var input = (Console.ReadLine() ?? "").Trim();
    while (!regex.IsMatch(input)) {
        Console.WriteLine("Error: Entrada inválida. Inténtalo de nuevo. Formato correcto: Numero entre 1 y 6(Ambos incluidos)");
        input = (Console.ReadLine() ?? "").Trim();
    }
    return int.Parse(input);
}

void LeerEntrada(ref PosicionButaca posicion) {
    var regex = new Regex("^[A-G]{1}:[1-9]{1}$");
    var input = (Console.ReadLine() ?? "").Trim();
     while (!regex.IsMatch(input)) {
         Console.WriteLine("Error: Entrada inválida. Inténtalo de nuevo. Formato correcto: A-G:1-7");
         input = Console.ReadLine()?.Trim().ToUpper() ?? "";
     }
     //Como input sera: L:N lo dividimos con un split y lo almacenamos en un vector de string
     string[] partes = input.Split(':');
     //Cojo el primer elemento del array y lo convierto a Char
     char letra = char.Parse(partes[0]);
     //Busco la posicion de la letra en un string de letras 
     //Y me devuelve el indice q sera la posicion de la fila
     posicion.Fila = LetrasFilas.IndexOf(letra);
     //Convierto el string de la columna a entero
     posicion.Columna = int.Parse(partes[1]) - 1;
    
}

void MostrarSala(Estado[,] matrix, Configuracion config) {
    Console.Write("   "); 
    for (int j = 0; j < config.Columnas; j++) {
        Console.Write($"{j + 1, 2}"); 
    }
    Console.WriteLine("");
    for (int i = 0; i < config.Filas; i++) {
        Console.Write($"{LetrasFilas[i]}");
        for (int j = 0; j < config.Columnas; j++) {
            switch (matrix[i, j]) {
                case Estado.Libre: Console.Write("[🟢]"); break;
                case Estado.NoDisponible: Console.Write("[🚫]"); break;
                case Estado.Ocupada: Console.Write("[🔴]"); break;
            }
        }
        Console.WriteLine();
    }
}

void ComprarEntrada(Estado[,] matrix, PosicionButaca posicion) {
    Console.WriteLine("-- Entradas maximas: 6");
    var cantidadEntradas = LeerEntero("-- ¿Cuantas entradas quiere comprar Entrada?");
    
    for (int i = 0; i < cantidadEntradas; i++) {
        bool colocada = false;
        Console.WriteLine($"Donde quiere la {i + 1} Entrada");
        while (!colocada) {
            LeerEntrada(ref posicion);
            if (matrix[posicion.Fila, posicion.Columna] == Estado.Libre) {
                matrix[posicion.Fila, posicion.Columna] = Estado.Ocupada;
                colocada = true;
                Console.WriteLine($"Butaca comprada con éxito. Precio: {Precio}.");
            } else if (matrix[posicion.Fila, posicion.Columna] == Estado.Ocupada) {
                Console.WriteLine("La butaca que ha seleccionado está ocupada");
            } else if (matrix[posicion.Fila, posicion.Columna] == Estado.NoDisponible) {
                Console.WriteLine("La butaca que ha seleccionado está fuera de servicio.");
            }
        }
    }
}

void DevolverEntrada(Estado[,] matrix, ref PosicionButaca posicion) {
    Console.WriteLine("-- ¿Que butaca desea devolver?");
    bool isValid = false;
    do {
        LeerEntrada(ref posicion);
        if(matrix[posicion.Fila, posicion.Columna] == Estado.Ocupada) {
            matrix[posicion.Fila, posicion.Columna] = Estado.Libre;
            isValid = true;
        } else if (matrix[posicion.Fila, posicion.Columna] == Estado.Libre) {
            Console.WriteLine("La butaca que ha seleccionado está libre");
        } else if (matrix[posicion.Fila, posicion.Columna] == Estado.NoDisponible) {
            Console.WriteLine("La butaca que ha seleccionado está fuera de servicio.");
        }
    } while (!isValid);
}

void VerRecaudacion(Estado[,] matriz, Configuracion config) {
    var contador = 0;
    for (var i = 0; i < config.Filas; i++) {
        for (var j = 0; j < config.Columnas; j++) {
            if (matriz[i, j] == Estado.Ocupada) {
                contador++;
            }
        }
    }

    var recaudacion = contador * Precio;
    Console.WriteLine($"La recaudacion total ha sido de: {recaudacion} euros");
}

void VerIndorme(Estado[,] matriz, Configuracion config) {
    var cantidadEntradasVendidas = 0;
    var asientosLibres = 0;
    var asientosNoDisponibles = 0;
    
    for (int i = 0; i < config.Filas; i++) {
        for (int j = 0; j < config.Columnas; j++) {
            if (matriz[i, j] == Estado.Ocupada) {
                cantidadEntradasVendidas++;
            }else if (matriz[i, j] == Estado.Libre) {
                asientosLibres++;
            } else if (matriz[i, j] == Estado.NoDisponible) {
                asientosNoDisponibles++;
            }
        }
    }
    Console.WriteLine($"Entradas Vendidas: {cantidadEntradasVendidas}");
    Console.WriteLine($"Asientos Libres: {asientosLibres}");
    Console.WriteLine($"sientos No Disponibles (F/S): {asientosNoDisponibles}");
    VerRecaudacion(matriz, config);
    PorcentajeDeOcupacion(ref cantidadEntradasVendidas,ref asientosLibres);
}

void PorcentajeDeOcupacion(ref int vendidas, ref int libres) {
   // (Vendidas / Total de Asientos Disponibles) * 100. (Asientos Disponibles = Total - Fuera Servicio).
   var totalDisponibles = vendidas + libres;
   if (totalDisponibles == 0) {
       Console.WriteLine("El porcentaje de ocupación es del 0% (No hay asientos disponibles).");
       return;
   }
   double porcentaje = ((double)vendidas / totalDisponibles) * 100;

   Console.WriteLine($"El porcentaje de ocupación es de: {porcentaje}%");
}


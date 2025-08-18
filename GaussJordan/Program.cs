using System.Globalization;
using GaussJordan.utils;

// Programa interactivo: no se detiene salvo que el usuario escriba "salir"
while (true)
{
    Console.WriteLine("Ingrese la cantidad de ecuaciones (m) o escriba 'salir' para terminar:");
    string? mInput = Console.ReadLine();
    if (IsExit(mInput)) break;
    if (!TryParsePositiveInt(mInput, out int m))
    {
        Console.WriteLine("Entrada inválida. Debe ser un entero positivo.");
        continue;
    }

    Console.WriteLine("Ingrese la cantidad de variables (n) o escriba 'salir' para terminar:");
    string? nInput = Console.ReadLine();
    if (IsExit(nInput)) break;
    if (!TryParsePositiveInt(nInput, out int n))
    {
        Console.WriteLine("Entrada inválida. Debe ser un entero positivo.");
        continue;
    }

    // Crear matriz aumentada m x (n+1)
    double[][] A = new double[m][];
    for (int i = 0; i < m; i++)
        A[i] = new double[n + 1];

    Console.WriteLine($"Ingrese los coeficientes y términos independientes ({m} filas, {n + 1} valores por fila).\n\tSugerencia: separe con espacio, coma, punto y coma o tabulación.\n\tEscriba 'salir' en cualquier momento para terminar.");

    bool userExited = false;
    for (int i = 0; i < m; i++)
    {
        while (true)
        {
            Console.WriteLine($"Fila {i + 1} (separados por espacio):");
            string? line = Console.ReadLine();
            if (IsExit(line)) { userExited = true; break; }

            if (!TryParseRow(line, n + 1, out double[] row))
            {
                Console.WriteLine($"Entrada inválida. Debe ingresar exactamente {n + 1} números.");
                continue; // Reintentar misma fila
            }

            A[i] = row;
            break; // siguiente fila
        }

        if (userExited) break;
    }

    if (userExited) break;

    var result = Helpers.Solve(A, m, n);

    Console.WriteLine(result.Message);
    if (result.Solutions is not null)
    {
        Console.WriteLine("Solución (particular si hay infinitas):");
        for (int i = 0; i < result.Solutions.Length; i++)
            Console.WriteLine($"x{i + 1} = {result.Solutions[i]:G6}");
    }

    Console.WriteLine();
}

static bool IsExit(string? s) => s != null && s.Trim().Equals("salir", StringComparison.OrdinalIgnoreCase);

static bool TryParsePositiveInt(string? s, out int n)
{
    if (!int.TryParse(s, out n)) return false;
    return n > 0;
}

static bool TryParseRow(string? line, int expectedCount, out double[] row)
{
    row = Array.Empty<double>();
    if (string.IsNullOrWhiteSpace(line)) return false;

    var parts = line.Split(new[] { ' ', '\t', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length != expectedCount) return false;

    var values = new double[expectedCount];
    for (int j = 0; j < expectedCount; j++)
    {
        if (!double.TryParse(parts[j], NumberStyles.Float, CultureInfo.InvariantCulture, out double v) &&
            !double.TryParse(parts[j], NumberStyles.Float, CultureInfo.CurrentCulture, out v))
        {
            return false;
        }
        values[j] = v;
    }

    row = values;
    return true;
}

namespace GaussJordan.utils
{
    /// <summary>
    /// Conjunto de utilidades para resolver sistemas de ecuaciones lineales mediante el
    /// método de Gauss-Jordan (reducción a forma escalonada reducida por renglones, RREF).
    /// </summary>
    /// <remarks>
    /// - Todas las operaciones son in-place: los arreglos de la matriz recibida se modifican directamente.
    /// - Se admite tanto matrices cuadradas (n ecuaciones x n variables) como rectangulares (m ecuaciones x n variables).
    /// - El parámetro <c>Epsilon</c> se usa como tolerancia numérica para considerar un valor como cero.
    /// - Las matrices aumentadas deben tener n+1 columnas: las primeras n son coeficientes y la última es el término independiente.
    /// </remarks>
    internal static class Helpers
    {
        /// <summary>
        /// Tolerancia numérica para comparar valores con cero y evitar divisiones por pivotes muy pequeños.
        /// </summary>
        private const double Epsilon = 1e-10;

        /// <summary>
        /// Clasificación del tipo de solución de un sistema lineal.
        /// </summary>
        internal enum SolutionType
        {
            /// <summary>
            /// El sistema tiene una única solución (rango == número de variables).
            /// </summary>
            Unique,
            /// <summary>
            /// El sistema tiene infinitas soluciones (rango &lt; número de variables y no hay inconsistencia).
            /// </summary>
            Infinite,
            /// <summary>
            /// El sistema es incompatible (no tiene solución) por presentar una fila de la forma 0 ... 0 | b, con b ≠ 0.
            /// </summary>
            Inconsistent
        }

        /// <summary>
        /// Busca la fila pivote dentro de un sub-bloque de filas, utilizando pivoteo parcial por columna.
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada) sobre la que se trabaja. Se espera al menos <paramref name="m"/> filas.</param>
        /// <param name="startRow">Índice de fila inicial desde donde se busca el pivote (inclusive).</param>
        /// <param name="column">Índice de columna en la que se buscará el pivote.</param>
        /// <param name="m">Cantidad de filas efectivas a considerar (límite superior exclusivo del recorrido).</param>
        /// <returns>
        /// Índice de la fila con el mayor valor absoluto en la columna indicada dentro del rango [startRow, m),
        /// o -1 si todos los valores en dicho rango son menores que <see cref="Epsilon"/> (no hay pivote válido).
        /// </returns>
        /// <remarks>
        /// Este método implementa pivoteo parcial: selecciona la fila con mayor |a[r][column]| para mejorar la estabilidad numérica.
        /// No modifica el contenido de la matriz; solo calcula el índice de fila recomendado para el pivote.
        /// Complejidad temporal: O(m - startRow).
        /// </remarks>
        public static int FindPivot(double[][] matrix, int startRow, int column, int m)
        {
            double maxValue = 0.0;
            int maxRow = -1;
            for (int r = startRow; r < m; r++)
            {
                double val = Math.Abs(matrix[r][column]);
                if (val > maxValue)
                {
                    maxValue = val;
                    maxRow = r;
                }
            }
            return (maxValue < Epsilon) ? -1 : maxRow;
        }

        /// <summary>
        /// Versión de compatibilidad para matrices cuadradas: busca el pivote en la columna <paramref name="column"/>
        /// a partir de la fila igual a la columna (diagonal principal) y hasta n filas.
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada).</param>
        /// <param name="column">Columna donde se busca el pivote. También se utiliza como fila de inicio.</param>
        /// <param name="n">Número de filas a considerar (matriz cuadrada n x n, o aumentada n x (n+1)).</param>
        /// <returns>Índice de fila pivote o -1 si no se encuentra un pivote numéricamente válido.</returns>
        public static int FindPivot(double[][] matrix, int column, int n) => FindPivot(matrix, column, column, n);

        /// <summary>
        /// Intercambia dos filas de la matriz (operación elemental E1).
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada) a modificar.</param>
        /// <param name="row1">Índice de la primera fila.</param>
        /// <param name="row2">Índice de la segunda fila.</param>
        /// <remarks>
        /// La operación es in-place y afecta todas las columnas de las filas indicadas.
        /// Si <paramref name="row1"/> == <paramref name="row2"/>, no se realiza ninguna acción.
        /// Complejidad temporal: O(1) al intercambiar referencias de arreglos jagged.
        /// </remarks>
        public static void SwapRows(double[][] matrix, int row1, int row2)
        {
            if (row1 == row2) return;
            var tmp = matrix[row1];
            matrix[row1] = matrix[row2];
            matrix[row2] = tmp;
        }

        /// <summary>
        /// Normaliza una fila dividiéndola por su pivote en la columna especificada (operación elemental E2).
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada) a modificar.</param>
        /// <param name="row">Índice de la fila a normalizar.</param>
        /// <param name="col">Columna donde se encuentra el pivote (valor por el que se divide toda la fila).</param>
        /// <returns>
        /// <c>true</c> si la fila fue normalizada (|pivote| ≥ <see cref="Epsilon"/>); <c>false</c> si el pivote es demasiado pequeño.
        /// </returns>
        /// <remarks>
        /// Esta operación convierte el pivote en 1 y escala el resto de la fila en consecuencia.
        /// Supone que la fila existe y contiene al menos <paramref name="col"/> + 1 columnas.
        /// Complejidad temporal: O(nColumnasDeLaFila).
        /// </remarks>
        public static bool NormalizeRow(double[][] matrix, int row, int col)
        {
            double pivot = matrix[row][col];
            if (Math.Abs(pivot) < Epsilon) return false;
            int columns = matrix[row].Length;
            for (int j = 0; j < columns; j++)
            {
                matrix[row][j] /= pivot;
            }
            return true;
        }

        /// <summary>
        /// Versión de compatibilidad que asume un pivote en la diagonal: <c>col == row</c>.
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada) a modificar.</param>
        /// <param name="row">Índice de la fila a normalizar.</param>
        /// <returns>
        /// <c>true</c> si la fila fue normalizada; <c>false</c> en caso contrario.
        /// </returns>
        public static bool NormalizeRow(double[][] matrix, int row) => NormalizeRow(matrix, row, row);

        /// <summary>
        /// Elimina (lleva a cero) el valor de la columna <paramref name="col"/> en una fila objetivo
        /// usando la fila pivote (operación elemental E3).
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada) a modificar.</param>
        /// <param name="pivotRow">Índice de la fila pivote (se recomienda que esté previamente normalizada).</param>
        /// <param name="col">Columna sobre la cual se anulará el valor en la <paramref name="targetRow"/>.</param>
        /// <param name="targetRow">Índice de la fila objetivo donde se desea anular el valor en la columna <paramref name="col"/>.</param>
        /// <remarks>
        /// Si <paramref name="pivotRow"/> == <paramref name="targetRow"/> o el factor a eliminar es numéricamente cero, no se realiza trabajo.
        /// Complejidad temporal: O(nColumnasDeLaFila).
        /// </remarks>
        public static void Eliminate(double[][] matrix, int pivotRow, int col, int targetRow)
        {
            if (pivotRow == targetRow) return;
            double factor = matrix[targetRow][col];
            if (Math.Abs(factor) < Epsilon) return;
            int columns = matrix[pivotRow].Length;
            for (int j = 0; j < columns; j++)
            {
                matrix[targetRow][j] -= factor * matrix[pivotRow][j];
            }
        }

        /// <summary>
        /// Versión de compatibilidad que asume <c>col == pivotRow</c> (común en el caso cuadrado en la diagonal).
        /// </summary>
        /// <param name="matrix">Matriz (posiblemente aumentada).</param>
        /// <param name="pivotRow">Índice de la fila pivote.</param>
        /// <param name="targetRow">Índice de la fila objetivo.</param>
        public static void Eliminate(double[][] matrix, int pivotRow, int targetRow) => Eliminate(matrix, pivotRow, pivotRow, targetRow);

        /// <summary>
        /// Resuelve un sistema lineal general m x n a partir de su matriz aumentada m x (n+1),
        /// aplicando el método de Gauss-Jordan para obtener una forma RREF.
        /// </summary>
        /// <param name="augmented">Matriz aumentada de tamaño m x (n+1). Se modificará in-place durante el proceso.</param>
        /// <param name="m">Número de ecuaciones (filas a considerar).</param>
        /// <param name="n">Número de variables (columnas de coeficientes; la última columna es el término independiente).</param>
        /// <returns>
        /// Una tupla con:
        /// - <see cref="SolutionType"/> <c>Type</c>: clasificación del sistema (Unique, Infinite, Inconsistent).
        /// - <c>double[]? Solutions</c>: vector solución de tamaño n (única o particular con libres = 0), o <c>null</c> si es inconsistente.
        /// - <c>string Message</c>: descripción textual del resultado.
        /// </returns>
        /// <remarks>
        /// Proceso general:
        /// 1) Para cada columna de 0..n-1 y mientras queden filas disponibles, buscar un pivote con <see cref="FindPivot(double[][], int, int, int)"/>.
        /// 2) Intercambiar filas para llevar el pivote a la posición actual (<see cref="SwapRows(double[][], int, int)"/>).
        /// 3) Normalizar la fila pivote (<see cref="NormalizeRow(double[][], int, int)"/>).
        /// 4) Eliminar el resto de elementos de la columna en todas las filas (<see cref="Eliminate(double[][], int, int, int)"/>).
        /// 5) Al final, detectar inconsistencias (filas 0..0 | b, b ≠ 0) y calcular el rango para decidir el tipo de solución.
        ///
        /// La solución devuelta para el caso "Infinite" es una solución particular en la que todas las variables libres se fijan a 0.
        /// </remarks>
        public static (SolutionType Type, double[]? Solutions, string Message) Solve(double[][] augmented, int m, int n)
        {
            int row = 0;
            var pivotCols = new List<int>();

            for (int col = 0; col < n && row < m; col++)
            {
                int pRow = FindPivot(augmented, row, col, m);
                if (pRow == -1) continue; // no hay pivote en esta columna

                SwapRows(augmented, row, pRow);
                if (!NormalizeRow(augmented, row, col))
                {
                    // columna problemática, continuar
                    continue;
                }
                for (int r = 0; r < m; r++)
                {
                    if (r == row) continue;
                    Eliminate(augmented, row, col, r);
                }

                pivotCols.Add(col);
                row++;
            }

            int rank = pivotCols.Count;

            // Detectar inconsistencia: 0 ... 0 | b, b != 0
            for (int r = 0; r < m; r++)
            {
                bool allZero = true;
                for (int c = 0; c < n; c++)
                {
                    if (Math.Abs(augmented[r][c]) > Epsilon) { allZero = false; break; }
                }
                if (allZero && Math.Abs(augmented[r][n]) > Epsilon)
                {
                    return (SolutionType.Inconsistent, null, "Sistema incompatible (sin solución).");
                }
            }

            // Construir solución
            if (rank == n)
            {
                // Única solución: cada columna es pivote
                double[] x = new double[n];
                for (int rPivot = 0; rPivot < rank; rPivot++)
                {
                    int cPivot = pivotCols[rPivot];
                    x[cPivot] = augmented[rPivot][n];
                }
                return (SolutionType.Unique, x, "Solución única encontrada.");
            }
            else
            {
                // Infinitas soluciones: devolver solución particular (variables libres = 0)
                double[] x = new double[n];
                Array.Fill(x, 0.0);
                for (int rPivot = 0; rPivot < pivotCols.Count; rPivot++)
                {
                    int cPivot = pivotCols[rPivot];
                    x[cPivot] = augmented[rPivot][n];
                }
                return (SolutionType.Infinite, x, "Sistema con infinitas soluciones. Se muestra una solución particular (variables libres = 0).");
            }
        }

        /// <summary>
        /// Versión de compatibilidad para matrices cuadradas n x (n+1).
        /// Solo retorna éxito cuando existe una solución única.
        /// </summary>
        /// <param name="augmented">Matriz aumentada n x (n+1) que será modificada in-place.</param>
        /// <param name="n">Número de variables (y de ecuaciones).</param>
        /// <returns>
        /// Una tupla con:
        /// - <c>Success</c>: <c>true</c> si existe solución única, <c>false</c> en caso contrario.
        /// - <c>Solutions</c>: el vector solución cuando <c>Success</c> es <c>true</c>; <c>null</c> si no.
        /// - <c>Error</c>: mensaje descriptivo cuando <c>Success</c> es <c>false</c>.
        /// </returns>
        public static (bool Success, double[]? Solutions, string? Error) Solve(double[][] augmented, int n)
        {
            var res = Solve(augmented, n, n);
            if (res.Type != SolutionType.Unique)
                return (false, null, res.Message);
            return (true, res.Solutions, null);
        }
    }
}

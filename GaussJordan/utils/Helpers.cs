namespace GaussJordan.utils
{
    internal static class Helpers
    {
        private const double Epsilon = 1e-10;

        internal enum SolutionType
        {
            Unique,
            Infinite,
            Inconsistent
        }

        // Generalizado: encuentra fila pivote a partir de startRow en la columna 'column' dentro de m filas
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

        // Compatibilidad anterior: cuadrada
        public static int FindPivot(double[][] matrix, int column, int n) => FindPivot(matrix, column, column, n);

        // Intercambia filas
        public static void SwapRows(double[][] matrix, int row1, int row2)
        {
            if (row1 == row2) return;
            var tmp = matrix[row1];
            matrix[row1] = matrix[row2];
            matrix[row2] = tmp;
        }

        // Generalizado: normaliza la fila 'row' usando como pivote la columna 'col'
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

        // Compatibilidad anterior (asume col == row)
        public static bool NormalizeRow(double[][] matrix, int row) => NormalizeRow(matrix, row, row);

        // Generalizado: elimina el valor de la columna 'col' en la fila 'targetRow' usando 'pivotRow'
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

        // Compatibilidad anterior (asume col == pivotRow)
        public static void Eliminate(double[][] matrix, int pivotRow, int targetRow) => Eliminate(matrix, pivotRow, pivotRow, targetRow);

        // Resuelve para matriz m x (n+1)
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

        // Compatibilidad: versión cuadrada
        public static (bool Success, double[]? Solutions, string? Error) Solve(double[][] augmented, int n)
        {
            var res = Solve(augmented, n, n);
            if (res.Type != SolutionType.Unique)
                return (false, null, res.Message);
            return (true, res.Solutions, null);
        }
    }
}

# Documentación de la clase Helpers (Gauss-Jordan)

Este documento describe en detalle la clase `GaussJordan.utils.Helpers`, responsable de ejecutar operaciones del método de Gauss-Jordan para resolver sistemas lineales. Incluye qué valores reciben sus miembros, en qué proceso se utilizan y con qué finalidad.

---

## Visión general

- Propósito: proveer funciones auxiliares para llevar una matriz aumentada a ***forma escalonada reducida por renglones*** (RREF), clasificar el sistema y extraer soluciones.
- Operación: todas las funciones trabajan in-place sobre la matriz (tipo `double[][]`, matriz jagged) suministrada.
- Compatibilidad: existen sobrecargas para el caso cuadrado que preservan APIs anteriores.
- Tolerancia numérica: `Epsilon = 1e-10` para evitar inestabilidades numéricas y divisiones por valores casi nulos.
- Entrada esperada: una matriz aumentada de tamaño m x (n+1), donde las primeras n columnas son coeficientes y la última columna son los términos independientes.

---

## Enumeración: SolutionType

- Unique: solución única (rango igual al número de variables).
- Infinite: infinitas soluciones (rango menor que el número de variables y sin inconsistencia).
- Inconsistent: sistema incompatible (existe una fila 0 ... 0 | b, con b ≠ 0).

Finalidad: clasificar el resultado luego de aplicar Gauss-Jordan.

---

## Métodos

### FindPivot(matrix, startRow, column, m) -> int
- Entradas:
  - matrix: `double[][]` sobre la que se operará (se espera al menos `m` filas).
  - startRow: fila inicial (inclusive) desde donde buscar el pivote.
  - column: columna donde se evalúa el pivote.
  - m: cantidad de filas efectivas a considerar (límite superior exclusivo).
- Proceso: busca el índice de la fila con mayor valor absoluto en la columna `column` dentro del rango [startRow, m). Si dicho valor es < Epsilon, retorna -1.
- Finalidad: implementar pivoteo parcial por columna para mejorar estabilidad numérica. No modifica la matriz.

Versión compatible: `FindPivot(matrix, column, n)` asume `startRow = column` y `m = n` (caso cuadrado).

---

### SwapRows(matrix, row1, row2)
- Entradas:
  - matrix: `double[][]` a modificar.
  - row1, row2: índices de filas a intercambiar.
- Proceso: intercambia referencias de las filas (operación elemental E1). Si `row1 == row2`, no hace nada.
- Finalidad: llevar la fila pivote al lugar de trabajo actual. Afecta a todas las columnas de la fila.

---

### NormalizeRow(matrix, row, col) -> bool
- Entradas:
  - matrix: `double[][]` a modificar.
  - row: fila a normalizar.
  - col: columna del pivote.
- Proceso: divide toda la fila por el valor pivote `matrix[row][col]`. Si |pivote| < Epsilon, retorna false y no normaliza.
- Finalidad: convertir el pivote en 1 (operación elemental E2) para facilitar la eliminación en otras filas.

Versión compatible: `NormalizeRow(matrix, row)` asume `col == row` (diagonal).

---

### Eliminate(matrix, pivotRow, col, targetRow)
- Entradas:
  - matrix: `double[][]` a modificar.
  - pivotRow: fila pivote (se recomienda normalizada).
  - col: columna en la que se desea anular el valor de la fila objetivo.
  - targetRow: fila objetivo.
- Proceso: resta a la fila objetivo un múltiplo de la fila pivote para anular el valor en la columna `col` (operación elemental E3). Si el factor es ≈ 0, no hace nada.
- Finalidad: llevar a cero los elementos por encima y por debajo del pivote.

Versión compatible: `Eliminate(matrix, pivotRow, targetRow)` asume `col == pivotRow` (común en cuadradas).

---

### Solve(augmented, m, n) -> (SolutionType Type, double[]? Solutions, string Message)
- Entradas:
  - augmented: matriz aumentada m x (n+1). Se modifica in-place.
  - m: número de ecuaciones (filas consideradas).
  - n: número de variables (n columnas de coeficientes + 1 columna de términos independientes).
- Proceso:
  1) Para cada columna 0..n-1 (y mientras queden filas disponibles), buscar pivote con `FindPivot` empezando en la fila `row`.
  2) Intercambiar la fila actual con la fila pivote (`SwapRows`).
  3) Normalizar la fila pivote (`NormalizeRow`).
  4) Eliminar el resto de entradas de la columna en todas las filas (`Eliminate`).
  5) Detectar inconsistencia: si alguna fila tiene todos los coeficientes ≈ 0 y término independiente ≠ 0, el sistema es Inconsistent.
  6) Calcular el rango como el número de columnas pivote. Si `rank == n`, Unique. Si `rank < n`, Infinite.
- Finalidad: clasificar y resolver el sistema. En caso de infinitas soluciones, se devuelve una solución particular con variables libres = 0.

Salida:
- Type: clasificación.
- Solutions: vector de tamaño n (única o particular) o null si inconsistente.
- Message: texto descriptivo del resultado.

Versión compatible (cuadrada): `Solve(augmented, n)` retorna `(Success, Solutions, Error)` y solo tiene éxito si hay solución única.

---

## Consideraciones y validaciones
- Tolerancia Epsilon = 1e-10 para estabilidad.
- Pivoteo parcial por columna reduce errores de redondeo.
- Todas las funciones trabajan in-place: la matriz de entrada se transforma.
- Se espera que cada fila tenga longitud n+1 y que existan al menos m filas.

---

## Ejemplo rápido

Para una matriz aumentada 3 x 4 (3 ecuaciones, 3 variables):

1) Llamar a `Solve(A, m: 3, n: 3)`.
2) El método modificará A a RREF, clasificará el sistema y devolverá el vector solución si aplica.

Si `Type == Infinite`, el vector retornado es una solución particular con variables libres fijadas a 0.

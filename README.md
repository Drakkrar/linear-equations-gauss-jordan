# Gauss-Jordan Solver (C# .NET 8)

Aplicación de consola para resolver sistemas de ecuaciones lineales usando el método de Gauss-Jordan.

## Features
- Soporta matrices aumentadas rectangulares m x (n+1): m ecuaciones y n variables.
- Clasifica el sistema: solución única, infinitas soluciones (devuelve una solución particular) o sistema inconsistente.
- Interfaz interactiva que solo termina cuando se escribe "salir".
- Pivoteo parcial por columna y tolerancia numérica (Epsilon = 1e-10).

## Requirements
- .NET 8

## Usage
1) Ejecute la aplicación.
2) Ingrese la cantidad de ecuaciones (m) o escriba "salir" para terminar.
3) Ingrese la cantidad de variables (n) o "salir" para terminar.
4) Ingrese la matriz aumentada con m filas y n+1 números por fila (los primeros n coeficientes y el último el término independiente). Puede separar con espacios, comas, punto y coma o tabulaciones.
5) Revise el tipo de solución y el vector reportado:
   - Solución única: el vector corresponde a la única solución.
   - Infinitas soluciones: se muestra una solución particular con variables libres = 0.
   - Inconsistente: el sistema no tiene solución.

## Input format (examples)
- Sistema m=2, n=3 (dos ecuaciones, tres variables):
  Fila 1: 1 2 3 4
  Fila 2: 0 1 -1 2
- Sistema m=3, n=2 (tres ecuaciones, dos variables):
  Fila 1: 2 1 5
  Fila 2: 1 -1 1
  Fila 3: 0 3 6

## Technical documentation
- Matriz aumentada: m x (n+1).
- Helpers (GaussJordan/utils/Helpers.cs):
  - FindPivot(matrix, startRow, column, m): busca fila pivote desde startRow (generalizado).
  - FindPivot(matrix, column, n): sobrecarga compatible para casos cuadrados (equivale a startRow=column, m=n).
  - SwapRows(matrix, r1, r2): intercambia filas.
  - NormalizeRow(matrix, row, col): normaliza usando el pivote en col (generalizado).
  - NormalizeRow(matrix, row): sobrecarga compatible que asume pivote en la diagonal.
  - Eliminate(matrix, pivotRow, col, targetRow): elimina el valor en col de targetRow usando pivotRow (generalizado).
  - Eliminate(matrix, pivotRow, targetRow): sobrecarga compatible que asume col=pivotRow.
  - Solve(augmented, m, n): devuelve (SolutionType, double[]?, string) con clasificación Unique/Infinite/Inconsistent.
  - Solve(augmented, n): sobrecarga compatible para n x (n+1), tiene éxito solo si hay solución única.

## Numerical considerations
- Epsilon = 1e-10 para evitar divisiones por valores muy pequeños.
- Pivoteo parcial reduce errores de redondeo.
- La solución particular en el caso infinito fija variables libres = 0.

## Notes
- Si la entrada de una fila no contiene exactamente n+1 números válidos, se le pedirá reintentar.
- Puede escribir "salir" en cualquier paso para finalizar.
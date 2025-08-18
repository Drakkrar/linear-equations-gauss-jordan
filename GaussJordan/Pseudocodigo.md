# Pseudocódigo: Resolvedor de Ecuaciones - Método Gauss-Jordan (cuadradas y rectangulares)

Este documento describe el algoritmo y las utilidades implementadas para resolver sistemas de ecuaciones lineales mediante el método de Gauss-Jordan. La implementación soporta:
- Matrices cuadradas y rectangulares (m ecuaciones x n variables), por ejemplo 3x2, 3x3, 2x3.
- Clasificación del sistema: solución única, infinitas soluciones o sistema inconsistente.
- Tolerancia numérica (Epsilon) para mayor estabilidad.
- API con funciones originales y nuevas sobrecargas compatibles hacia atrás.

---

## Tipos y convenciones
- Matriz aumentada de tamaño m x (n+1): las primeras n columnas son coeficientes; la última columna es el vector de términos independientes.
- Epsilon: 1e-10 (tolerancia para considerar valores como cero).
- Enumeración para clasificar soluciones:
  - SolutionType = { Unique, Infinite, Inconsistent }

---

## API de utilidades (Helpers)

Se listan las funciones clave con sus sobrecargas y propósito. Las firmas aquí son conceptuales, no estrictamente de un lenguaje.

1) Encontrar pivote (generalizado y compatible)
- FindPivot(matriz, startRow, column, m) -> int
  - Busca la fila con mayor valor absoluto en la columna `column` empezando en `startRow` hasta `m-1`.
  - Retorna el índice de la fila pivote o -1 si no existe un pivote válido (|valor| < Epsilon).
- FindPivot(matriz, column, n) -> int
  - Versión compatible para matrices cuadradas; equivale a `FindPivot(matriz, startRow=column, column, m=n)`.

2) Intercambiar filas
- SwapRows(matriz, row1, row2)
  - Intercambia por referencia las filas completas `row1` y `row2`.

3) Normalizar fila (generalizado y compatible)
- NormalizeRow(matriz, row, col) -> bool
  - Normaliza la fila `row` dividiendo toda la fila por el pivote ubicado en la columna `col`.
  - Retorna falso si el pivote es ~0 (|pivote| < Epsilon).
- NormalizeRow(matriz, row) -> bool
  - Versión compatible que asume pivote en la diagonal: `col = row`.

4) Eliminar elemento (generalizado y compatible)
- Eliminate(matriz, pivotRow, col, targetRow)
  - Hace cero el valor de la fila `targetRow` en la columna `col` usando la fila pivote `pivotRow` (ya normalizada).
- Eliminate(matriz, pivotRow, targetRow)
  - Versión compatible que asume `col = pivotRow`.

5) Resolver sistema (generalizado y compatible)
- Solve(augmented, m, n) -> (SolutionType type, double[]? solutions, string message)
  - Ejecuta Gauss-Jordan (forma reducida por renglones, RREF) sobre una matriz aumentada m x (n+1).
  - Devuelve el tipo de solución y, si aplica, un vector de tamaño n:
    - Unique: solución única en `solutions`.
    - Infinite: solución particular con variables libres = 0 en `solutions`.
    - Inconsistent: `solutions = null` y mensaje indicando incompatibilidad.
- Solve(augmented, n) -> (bool success, double[]? solutions, string? error)
  - Versión compatible para matriz cuadrada n x (n+1). Solo tiene éxito si hay solución única; en otro caso retorna `error`.

---

## Algoritmo general (RREF) para m x (n+1)

Entrada: `augmented` (matriz m x (n+1))

Variables internas:
- row = 0
- pivotCols = []

Para col = 0 hasta n-1 y mientras row < m:
1. pivotRow = FindPivot(augmented, row, col, m)
   - Si pivotRow == -1, continuar con la siguiente columna (no hay pivote en esta columna).
2. SwapRows(augmented, row, pivotRow)
3. NormalizeRow(augmented, row, col)
4. Para r = 0 hasta m-1, r != row: Eliminate(augmented, row, col, r)
5. pivotCols.agregar(col); row++

Posterior a la eliminación:
- Detectar inconsistencia: si existe una fila con todos los coeficientes ≈ 0 y término independiente ≠ 0, el sistema es Inconsistent.
- rank = pivotCols.count
- Si rank == n: Unique
  - La solución se obtiene de la última columna (ya que cada columna de variables tiene pivote).
- Si rank < n: Infinite
  - Devolver una solución particular: asignar 0 a variables libres y tomar los valores de la última columna en las filas pivote para las variables básicas.

---

## Pseudocódigo (generalizado)

```
FUNCIÓN resolverGaussJordan(augmented, m, n)
    row = 0
    pivotCols = []

    PARA col = 0 HASTA n-1 MIENTRAS row < m
        pivotRow = FindPivot(augmented, row, col, m)
        SI pivotRow == -1 ENTONCES
            CONTINUAR // no hay pivote en esta columna
        FIN SI

        SwapRows(augmented, row, pivotRow)
        SI !NormalizeRow(augmented, row, col) ENTONCES
            CONTINUAR // pivote demasiado pequeño
        FIN SI

        PARA r = 0 HASTA m-1
            SI r != row ENTONCES
                Eliminate(augmented, row, col, r)
            FIN SI
        FIN PARA

        pivotCols.AGREGAR(col)
        row = row + 1
    FIN PARA

    // Inconsistencia
    PARA r = 0 HASTA m-1
        allZero = VERDADERO
        PARA c = 0 HASTA n-1
            SI abs(augmented[r][c]) > EPSILON ENTONCES
                allZero = FALSO
                ROMPER
            FIN SI
        FIN PARA
        SI allZero Y abs(augmented[r][n]) > EPSILON ENTONCES
            RETORNAR (Inconsistent, NULL, "Sistema incompatible (sin solución)")
        FIN SI
    FIN PARA

    rank = TAM(pivotCols)
    SI rank == n ENTONCES
        // Única solución
        x = ARREGLO[n]
        PARA i = 0 HASTA n-1
            // cPivot = pivotCols[i]  // en RREF, filas pivote están en orden
            x[i] = augmented[i][n]
        FIN PARA
        RETORNAR (Unique, x, "Solución única encontrada")
    SINO
        // Infinitas soluciones (particular con libres = 0)
        x = ARREGLO[n] LLENO CON 0
        PARA i = 0 HASTA rank-1
            cPivot = pivotCols[i]
            x[cPivot] = augmented[i][n]
        FIN PARA
        RETORNAR (Infinite, x, "Infinitas soluciones: se muestra una solución particular (variables libres = 0)")
    FIN SI
FIN FUNCIÓN
```

---

## Pseudocódigo funciones auxiliares (Helpers)
### Encontrar Pivote
```
FUNCIÓN encontrarPivote(matriz, columna, n)
    max_valor = 0
    max_fila = -1
    
    PARA i = columna HASTA n-1
        SI abs(matriz[i][columna]) > max_valor ENTONCES
            max_valor = abs(matriz[i][columna])
            max_fila = i
        FIN SI
    FIN PARA
    
    SI max_valor < EPSILON ENTONCES  // EPSILON = tolerancia pequeña
        RETORNAR -1  // No hay pivote válido
    FIN SI
    
    RETORNAR max_fila
FIN FUNCIÓN
```

### Intercambiar Filas
```
FUNCIÓN intercambiarFilas(matriz, fila1, fila2, columnas)
    PARA j = 0 HASTA columnas-1
        temp = matriz[fila1][j]
        matriz[fila1][j] = matriz[fila2][j]
        matriz[fila2][j] = temp
    FIN PARA
FIN FUNCIÓN
```

### Normalizar Fila
```
FUNCIÓN normalizarFila(matriz, fila, columnas)
    pivote = matriz[fila][fila]
    
    SI abs(pivote) < EPSILON ENTONCES
        MOSTRAR "Error: pivote muy pequeño"
        RETORNAR FALSO
    FIN SI
    
    PARA j = 0 HASTA columnas-1
        matriz[fila][j] = matriz[fila][j] / pivote
    FIN PARA
    
    RETORNAR VERDADERO
FIN FUNCIÓN
```

### Eliminar Elemento
```
FUNCIÓN eliminarElemento(matriz, fila_pivote, fila_objetivo, columnas)
    factor = matriz[fila_objetivo][fila_pivote]
    
    PARA j = 0 HASTA columnas-1
        matriz[fila_objetivo][j] = matriz[fila_objetivo][j] - 
                                   (factor * matriz[fila_pivote][j])
    FIN PARA
FIN FUNCIÓN
```

### Extraer Soluciones
```
FUNCIÓN extraerSoluciones(matriz, n)
    soluciones = ARREGLO[n]
    
    PARA i = 0 HASTA n-1
        soluciones[i] = matriz[i][n]  // Última columna
    FIN PARA
    
    RETORNAR soluciones
FIN FUNCIÓN
```

---

## Programa principal (interactivo)
El programa es interactivo y solo finaliza cuando el usuario escribe `salir`.

Resumen de flujo:
1. Solicita m (ecuaciones) o `salir`.
2. Solicita n (variables) o `salir`.
3. Lee la matriz aumentada (m filas, n+1 números por fila). Puede usar espacio, coma, punto y coma o tabulador como separadores.
4. Ejecuta `Solve(augmented, m, n)`.
5. Muestra el tipo de solución y, si aplica, un vector solución (particular si hay infinitas soluciones).
6. Regresa al paso 1.

---

## Consideraciones numéricas y validaciones
- Tolerancia numérica `EPSILON = 1e-10` para evitar divisiones por números muy pequeños y mejorar estabilidad.
- Pivoteo parcial por columna: elegimos la fila con mayor |valor| en la columna para reducir errores de redondeo.
- Detección de inconsistencia y cálculo de rango para clasificar soluciones.
- Entradas validadas: cantidad de números por fila y formato numérico (acepta cultura invariante o actual).

---

## Complejidad
- Tiempo aproximado: O(m · n · min(m, n)) para la reducción completa (en el caso cuadrado O(n³)).
- Espacio: O(m · (n+1)).

---

## Casos especiales
- Única solución: rank == n.
- Infinitas soluciones: rank < n y sin filas inconsistentes. Se devuelve una solución particular con variables libres = 0.
- Inconsistente: existe una fila con todos los coeficientes ≈ 0 y término independiente ≠ 0.
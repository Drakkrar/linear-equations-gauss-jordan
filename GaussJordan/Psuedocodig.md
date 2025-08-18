# Pseudocódigo: Resolvedor de Ecuaciones - Método Gauss-Jordan

## Función Principal
```
FUNCIÓN resolverGaussJordan(matriz_aumentada, n)
    // n = número de ecuaciones/incógnitas
    // matriz_aumentada = matriz de coeficientes con términos independientes
    
    PARA i = 0 HASTA n-1
        // Paso 1: Encontrar el pivote
        pivote_fila = encontrarPivote(matriz_aumentada, i, n)
        
        SI pivote_fila == -1 ENTONCES
            RETORNAR "Sistema sin solución única"
        FIN SI
        
        // Paso 2: Intercambiar filas si es necesario
        SI pivote_fila != i ENTONCES
            intercambiarFilas(matriz_aumentada, i, pivote_fila, n+1)
        FIN SI
        
        // Paso 3: Normalizar la fila pivote
        normalizarFila(matriz_aumentada, i, n+1)
        
        // Paso 4: Eliminar elementos de la columna
        PARA j = 0 HASTA n-1
            SI j != i ENTONCES
                eliminarElemento(matriz_aumentada, i, j, n+1)
            FIN SI
        FIN PARA
    FIN PARA
    
    // Extraer soluciones
    soluciones = extraerSoluciones(matriz_aumentada, n)
    RETORNAR soluciones
FIN FUNCIÓN
```

## Funciones Auxiliares

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

## Programa Principal
```
ALGORITMO principal
    // Entrada de datos
    ESCRIBIR "Ingrese el número de ecuaciones: "
    LEER n
    
    // Crear matriz aumentada
    matriz_aumentada = MATRIZ[n][n+1]
    
    ESCRIBIR "Ingrese los coeficientes y términos independientes:"
    PARA i = 0 HASTA n-1
        ESCRIBIR "Ecuación ", i+1, ":"
        PARA j = 0 HASTA n
            SI j < n ENTONCES
                ESCRIBIR "Coeficiente x", j+1, ": "
            SINO
                ESCRIBIR "Término independiente: "
            FIN SI
            LEER matriz_aumentada[i][j]
        FIN PARA
    FIN PARA
    
    // Resolver sistema
    resultado = resolverGaussJordan(matriz_aumentada, n)
    
    // Mostrar resultados
    SI resultado == "Sistema sin solución única" ENTONCES
        ESCRIBIR resultado
    SINO
        ESCRIBIR "Soluciones del sistema:"
        PARA i = 0 HASTA n-1
            ESCRIBIR "x", i+1, " = ", resultado[i]
        FIN PARA
    FIN SI
FIN ALGORITMO
```

## Consideraciones Adicionales

### Validaciones Recomendadas
- Verificar que la matriz no sea singular
- Manejar casos de sistemas inconsistentes
- Implementar tolerancia numérica para evitar errores de punto flotante
- Validar entrada de datos del usuario

### Optimizaciones Posibles
- Pivoteo parcial o total para mayor estabilidad numérica
- Detección temprana de filas linealmente dependientes
- Manejo especial para matrices dispersas (sparse)

### Complejidad Computacional
- **Tiempo:** O(n³)
- **Espacio:** O(n²)

### Casos Especiales
- Sistema compatible determinado: Solución única
- Sistema compatible indeterminado: Infinitas soluciones
- Sistema incompatible: Sin solución
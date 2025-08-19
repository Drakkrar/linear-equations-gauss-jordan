using GaussJordan.utils;
using Xunit;

namespace GaussJordan.Tests
{
    /// <summary>
    /// Comprehensive unit tests for the GaussJordan.utils.Helpers class.
    /// Tests all public methods including both generalized and compatibility versions.
    /// </summary>
    public class HelpersTests
    {
        private const double Tolerance = 1e-10;

        // Helper method to create test matrices
        private static double[][] CreateMatrix(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            double[][] matrix = new double[rows][];
            
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new double[cols];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = data[i, j];
                }
            }
            
            return matrix;
        }

        // Helper method to compare matrices with tolerance
        private static void AssertMatrixEquals(double[][] expected, double[][] actual)
        {
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Length, actual[i].Length);
                for (int j = 0; j < expected[i].Length; j++)
                {
                    Assert.True(Math.Abs(expected[i][j] - actual[i][j]) < Tolerance, 
                        $"Matrix difference at [{i},{j}]: expected {expected[i][j]}, actual {actual[i][j]}");
                }
            }
        }

        // Helper method to compare arrays with tolerance
        private static void AssertArrayEquals(double[] expected, double[] actual)
        {
            Assert.Equal(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.True(Math.Abs(expected[i] - actual[i]) < Tolerance, 
                    $"Array difference at [{i}]: expected {expected[i]}, actual {actual[i]}");
            }
        }

        #region FindPivot Tests

        [Fact]
        public void FindPivot_Generalized_FindsCorrectPivot()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 4.0, 5.0 },
                { 0.0, 6.0, 7.0 }
            });

            // Act
            int pivotRow = Helpers.FindPivot(matrix, 1, 1, 3);

            // Assert
            Assert.Equal(2, pivotRow); // Row 2 has the largest absolute value (6.0) in column 1 starting from row 1
        }

        [Fact]
        public void FindPivot_Generalized_ReturnsNegativeOneWhenNoPivot()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 0.0, 3.0 },
                { 2.0, 0.0, 5.0 },
                { 3.0, 0.0, 7.0 }
            });

            // Act
            int pivotRow = Helpers.FindPivot(matrix, 0, 1, 3);

            // Assert
            Assert.Equal(-1, pivotRow); // No valid pivot in column 1
        }

        [Fact]
        public void FindPivot_Generalized_HandlesVerySmallValues()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 1e-11, 3.0 },
                { 2.0, 1e-12, 5.0 },
                { 3.0, 1e-9, 7.0 }
            });

            // Act
            int pivotRow = Helpers.FindPivot(matrix, 0, 1, 3);

            // Assert - 1e-9 is greater than epsilon (1e-10), so row 2 should be the pivot
            Assert.Equal(2, pivotRow); // Row 2 has the largest value (1e-9) above epsilon threshold
        }

        [Fact]
        public void FindPivot_Generalized_ReturnsNegativeOneForValuesBelowEpsilon()
        {
            // Arrange - All values below epsilon threshold
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 1e-11, 3.0 },
                { 2.0, 1e-12, 5.0 },
                { 3.0, 1e-11, 7.0 }
            });

            // Act
            int pivotRow = Helpers.FindPivot(matrix, 0, 1, 3);

            // Assert
            Assert.Equal(-1, pivotRow); // All values in column 1 are below epsilon threshold
        }

        [Fact]
        public void FindPivot_Generalized_FindsLargestAbsoluteValue()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, -5.0, 5.0 },
                { 0.0, 3.0, 7.0 }
            });

            // Act
            int pivotRow = Helpers.FindPivot(matrix, 1, 1, 3);

            // Assert
            Assert.Equal(1, pivotRow); // Row 1 has the largest absolute value (-5.0) in column 1 starting from row 1
        }

        [Fact]
        public void FindPivot_Square_CallsGeneralizedVersion()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 2.0, 1.0, 3.0 },
                { 0.0, 4.0, 5.0 },
                { 0.0, 1.0, 7.0 }
            });

            // Act
            int pivotRow = Helpers.FindPivot(matrix, 1, 3);

            // Assert
            Assert.Equal(1, pivotRow); // Row 1 has the largest value (4.0) in column 1
        }

        #endregion

        #region SwapRows Tests

        [Fact]
        public void SwapRows_SwapsRowsCorrectly()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 7.0, 8.0, 9.0 },
                { 4.0, 5.0, 6.0 },
                { 1.0, 2.0, 3.0 }
            });

            // Act
            Helpers.SwapRows(matrix, 0, 2);

            // Assert
            AssertMatrixEquals(expected, matrix);
        }

        [Fact]
        public void SwapRows_SameRowDoesNothing()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            // Act
            Helpers.SwapRows(matrix, 1, 1);

            // Assert
            AssertMatrixEquals(expected, matrix);
        }

        #endregion

        #region NormalizeRow Tests

        [Fact]
        public void NormalizeRow_Generalized_NormalizesCorrectly()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 4.0, 8.0 },
                { 0.0, 0.0, 6.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 1.0, 2.0 },
                { 0.0, 0.0, 6.0 }
            });

            // Act
            bool result = Helpers.NormalizeRow(matrix, 1, 1);

            // Assert
            Assert.True(result);
            AssertMatrixEquals(expected, matrix);
        }

        [Fact]
        public void NormalizeRow_Generalized_ReturnsFalseForSmallPivot()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 1e-11, 8.0 },
                { 0.0, 0.0, 6.0 }
            });

            var original = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 1e-11, 8.0 },
                { 0.0, 0.0, 6.0 }
            });

            // Act
            bool result = Helpers.NormalizeRow(matrix, 1, 1);

            // Assert
            Assert.False(result);
            AssertMatrixEquals(original, matrix); // Matrix should remain unchanged
        }

        [Fact]
        public void NormalizeRow_Square_CallsGeneralizedVersion()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 2.0, 4.0, 6.0 },
                { 0.0, 3.0, 9.0 },
                { 0.0, 0.0, 4.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 2.0, 4.0, 6.0 },
                { 0.0, 1.0, 3.0 },
                { 0.0, 0.0, 4.0 }
            });

            // Act
            bool result = Helpers.NormalizeRow(matrix, 1);

            // Assert
            Assert.True(result);
            AssertMatrixEquals(expected, matrix);
        }

        [Fact]
        public void NormalizeRow_HandlesNegativePivot()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, -4.0, 8.0 },
                { 0.0, 0.0, 6.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 1.0, -2.0 },
                { 0.0, 0.0, 6.0 }
            });

            // Act
            bool result = Helpers.NormalizeRow(matrix, 1, 1);

            // Assert
            Assert.True(result);
            AssertMatrixEquals(expected, matrix);
        }

        #endregion

        #region Eliminate Tests

        [Fact]
        public void Eliminate_Generalized_EliminatesCorrectly()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 0.0, 2.0 },
                { 3.0, 1.0, 8.0 },
                { 2.0, 0.0, 6.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 0.0, 2.0 },
                { 0.0, 1.0, 2.0 },
                { 2.0, 0.0, 6.0 }
            });

            // Act - eliminate column 0 in row 1 using row 0 as pivot
            Helpers.Eliminate(matrix, 0, 0, 1);

            // Assert
            AssertMatrixEquals(expected, matrix);
        }

        [Fact]
        public void Eliminate_Generalized_SameRowDoesNothing()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            // Act
            Helpers.Eliminate(matrix, 1, 1, 1);

            // Assert
            AssertMatrixEquals(expected, matrix);
        }

        [Fact]
        public void Eliminate_Generalized_SkipsSmallFactors()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 1e-11, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 1e-11, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            });

            // Act - try to eliminate very small factor
            Helpers.Eliminate(matrix, 0, 0, 1);

            // Assert
            AssertMatrixEquals(expected, matrix); // Should remain unchanged
        }

        [Fact]
        public void Eliminate_Square_CallsGeneralizedVersion()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 2.0, 4.0, 8.0 },
                { 3.0, 6.0, 15.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 0.0, 2.0 },
                { 3.0, 6.0, 15.0 }
            });

            // Act - eliminate using pivot row 0 for target row 1
            Helpers.Eliminate(matrix, 0, 1);

            // Assert
            AssertMatrixEquals(expected, matrix);
        }

        [Fact]
        public void Eliminate_HandlesNegativeFactors()
        {
            // Arrange
            var matrix = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { -2.0, 1.0, 4.0 },
                { 3.0, 6.0, 15.0 }
            });

            var expected = CreateMatrix(new double[,] {
                { 1.0, 2.0, 3.0 },
                { 0.0, 5.0, 10.0 },
                { 3.0, 6.0, 15.0 }
            });

            // Act - eliminate column 0 in row 1 using row 0 as pivot
            Helpers.Eliminate(matrix, 0, 0, 1);

            // Assert
            AssertMatrixEquals(expected, matrix);
        }

        #endregion

        #region Solve Tests - Generalized Version

        [Fact]
        public void Solve_Generalized_UniqueSquareSolution()
        {
            // Arrange - System: x + y = 3, 2x + 3y = 8
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 3.0, 8.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Unique, result.Type);
            Assert.NotNull(result.Solutions);
            Assert.Equal("Solución única encontrada.", result.Message);
            AssertArrayEquals(new double[] { 1.0, 2.0 }, result.Solutions);
        }

        [Fact]
        public void Solve_Generalized_InfiniteSolutions()
        {
            // Arrange - System: x + y = 3, 2x + 2y = 6 (second equation is multiple of first)
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 2.0, 6.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Infinite, result.Type);
            Assert.NotNull(result.Solutions);
            Assert.Contains("infinitas soluciones", result.Message);
            // Should provide a particular solution with free variables = 0
            Assert.Equal(2, result.Solutions.Length);
        }

        [Fact]
        public void Solve_Generalized_InconsistentSystem()
        {
            // Arrange - System: x + y = 3, 2x + 2y = 7 (inconsistent)
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 2.0, 7.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Inconsistent, result.Type);
            Assert.Null(result.Solutions);
            Assert.Equal("Sistema incompatible (sin solución).", result.Message);
        }

        [Fact]
        public void Solve_Generalized_RectangularMoreEquationsThanVariables()
        {
            // Arrange - Let's use a consistent overdetermined system
            // System: x + y = 3, 2x + y = 5, 3x + 2y = 8
            // This should have a unique solution: x = 2, y = 1
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 1.0, 5.0 },
                { 3.0, 2.0, 8.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 3, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Unique, result.Type);
            Assert.NotNull(result.Solutions);
            AssertArrayEquals(new double[] { 2.0, 1.0 }, result.Solutions);
        }

        [Fact]
        public void Solve_Generalized_InconsistentOverdeterminedSystem()
        {
            // Arrange - Inconsistent overdetermined system: x + y = 3, 2x + y = 5, x - y = -1, x + y = 4 (contradicts first equation)
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 1.0, 5.0 },
                { 1.0, -1.0, -1.0 },
                { 1.0, 1.0, 4.0 }  // This contradicts the first equation
            });

            // Act
            var result = Helpers.Solve(augmented, 4, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Inconsistent, result.Type);
            Assert.Null(result.Solutions);
            Assert.Equal("Sistema incompatible (sin solución).", result.Message);
        }

        [Fact]
        public void Solve_Generalized_RectangularMoreVariablesThanEquations()
        {
            // Arrange - Underdetermined system: x + y + z = 6, 2x + y + 3z = 14
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 1.0, 6.0 },
                { 2.0, 1.0, 3.0, 14.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2, 3);

            // Assert
            Assert.Equal(Helpers.SolutionType.Infinite, result.Type);
            Assert.NotNull(result.Solutions);
            Assert.Contains("infinitas soluciones", result.Message);
            Assert.Equal(3, result.Solutions.Length);
        }

        [Fact]
        public void Solve_Generalized_HandlesZeroPivots()
        {
            // Arrange - System with zeros on diagonal
            var augmented = CreateMatrix(new double[,] {
                { 0.0, 1.0, 2.0 },
                { 1.0, 0.0, 3.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Unique, result.Type);
            Assert.NotNull(result.Solutions);
        }

        [Fact]
        public void Solve_Generalized_EmptySystem()
        {
            // Arrange - Empty system (0 equations, 2 variables)
            var augmented = CreateMatrix(new double[,] { });

            // Act
            var result = Helpers.Solve(augmented, 0, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Infinite, result.Type);
            Assert.NotNull(result.Solutions);
            Assert.Equal(2, result.Solutions.Length);
            // All variables should be free (set to 0)
            AssertArrayEquals(new double[] { 0.0, 0.0 }, result.Solutions);
        }

        #endregion

        #region Solve Tests - Square Version (Compatibility)

        [Fact]
        public void Solve_Square_UniqueSquareSolution()
        {
            // Arrange - System: x + y = 3, 2x + 3y = 8
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 3.0, 8.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Solutions);
            Assert.Null(result.Error);
            AssertArrayEquals(new double[] { 1.0, 2.0 }, result.Solutions);
        }

        [Fact]
        public void Solve_Square_InfiniteSolutions()
        {
            // Arrange - System: x + y = 3, 2x + 2y = 6 (infinite solutions)
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 2.0, 6.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Solutions);
            Assert.NotNull(result.Error);
            Assert.Contains("infinitas soluciones", result.Error);
        }

        [Fact]
        public void Solve_Square_InconsistentSystem()
        {
            // Arrange - System: x + y = 3, 2x + 2y = 7 (inconsistent)
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 3.0 },
                { 2.0, 2.0, 7.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Solutions);
            Assert.NotNull(result.Error);
            Assert.Equal("Sistema incompatible (sin solución).", result.Error);
        }

        [Fact]
        public void Solve_Square_LargerSystem()
        {
            // Arrange - 3x3 system: x + y + z = 6, 2x + y + 3z = 14, x - y + 2z = 7
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 1.0, 1.0, 6.0 },
                { 2.0, 1.0, 3.0, 14.0 },
                { 1.0, -1.0, 2.0, 7.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 3);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Solutions);
            Assert.Null(result.Error);
            Assert.Equal(3, result.Solutions.Length);
            // Verify the solution by substitution
            double[] sol = result.Solutions;
            Assert.True(Math.Abs(sol[0] + sol[1] + sol[2] - 6.0) < Tolerance);
            Assert.True(Math.Abs(2 * sol[0] + sol[1] + 3 * sol[2] - 14.0) < Tolerance);
            Assert.True(Math.Abs(sol[0] - sol[1] + 2 * sol[2] - 7.0) < Tolerance);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CompleteGaussJordanProcess_TracksAllSteps()
        {
            // Arrange - Simple 2x2 system where we can track each step
            var augmented = CreateMatrix(new double[,] {
                { 2.0, 1.0, 5.0 },
                { 1.0, 3.0, 8.0 }
            });

            // Test individual components first
            
            // 1. Find pivot for first column
            int pivot1 = Helpers.FindPivot(augmented, 0, 0, 2);
            Assert.Equal(0, pivot1); // Row 0 has largest value (2.0)

            // 2. Normalize first row
            bool normalized = Helpers.NormalizeRow(augmented, 0, 0);
            Assert.True(normalized);

            // 3. Eliminate first column in second row
            Helpers.Eliminate(augmented, 0, 0, 1);

            // 4. Find pivot for second column in remaining rows
            int pivot2 = Helpers.FindPivot(augmented, 1, 1, 2);
            Assert.Equal(1, pivot2);

            // 5. Normalize second row
            bool normalized2 = Helpers.NormalizeRow(augmented, 1, 1);
            Assert.True(normalized2);

            // 6. Eliminate second column in first row
            Helpers.Eliminate(augmented, 1, 1, 0);

            // Now test the complete solution
            var freshMatrix = CreateMatrix(new double[,] {
                { 2.0, 1.0, 5.0 },
                { 1.0, 3.0, 8.0 }
            });

            var result = Helpers.Solve(freshMatrix, 2, 2);
            Assert.Equal(Helpers.SolutionType.Unique, result.Type);
            Assert.NotNull(result.Solutions);
        }

        [Fact]
        public void EdgeCase_AllZeroMatrix()
        {
            // Arrange - Matrix with all zeros
            var augmented = CreateMatrix(new double[,] {
                { 0.0, 0.0, 0.0 },
                { 0.0, 0.0, 0.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 2, 2);

            // Assert
            Assert.Equal(Helpers.SolutionType.Infinite, result.Type);
            Assert.NotNull(result.Solutions);
            AssertArrayEquals(new double[] { 0.0, 0.0 }, result.Solutions);
        }

        [Fact]
        public void EdgeCase_SingleVariableSingleEquation()
        {
            // Arrange - Simple 1x1 system: 2x = 6
            var augmented = CreateMatrix(new double[,] {
                { 2.0, 6.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 1, 1);

            // Assert
            Assert.Equal(Helpers.SolutionType.Unique, result.Type);
            Assert.NotNull(result.Solutions);
            AssertArrayEquals(new double[] { 3.0 }, result.Solutions);
        }

        [Fact]
        public void EdgeCase_IdentityMatrix()
        {
            // Arrange - Identity matrix augmented system
            var augmented = CreateMatrix(new double[,] {
                { 1.0, 0.0, 0.0, 1.0 },
                { 0.0, 1.0, 0.0, 2.0 },
                { 0.0, 0.0, 1.0, 3.0 }
            });

            // Act
            var result = Helpers.Solve(augmented, 3, 3);

            // Assert
            Assert.Equal(Helpers.SolutionType.Unique, result.Type);
            Assert.NotNull(result.Solutions);
            AssertArrayEquals(new double[] { 1.0, 2.0, 3.0 }, result.Solutions);
        }

        #endregion
    }
}
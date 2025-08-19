using GaussJordan.utils;

namespace GaussJordan.TestRunner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running basic tests for Helpers class methods...\n");

            // Test the two specific failing cases
            TestFindPivotWithSmallValues();
            TestOverdeterminedSystem();

            Console.WriteLine("\nAll basic tests completed!");
        }

        static void TestFindPivotWithSmallValues()
        {
            Console.WriteLine("Testing FindPivot with small values...");
            
            var matrix = new double[][] {
                new double[] { 1.0, 1e-11, 3.0 },
                new double[] { 2.0, 1e-12, 5.0 },
                new double[] { 3.0, 1e-9, 7.0 }
            };

            int pivot = Helpers.FindPivot(matrix, 0, 1, 3);
            Console.WriteLine($"  FindPivot result: {pivot} (1e-9 > 1e-10 epsilon, so should be 2)");
            
            // Test with all values below epsilon
            var matrix2 = new double[][] {
                new double[] { 1.0, 1e-11, 3.0 },
                new double[] { 2.0, 1e-12, 5.0 },
                new double[] { 3.0, 1e-11, 7.0 }
            };

            int pivot2 = Helpers.FindPivot(matrix2, 0, 1, 3);
            Console.WriteLine($"  FindPivot result for all small values: {pivot2} (all < epsilon, should be -1)");
            
            Console.WriteLine("  ? FindPivot small values test completed");
        }

        static void TestOverdeterminedSystem()
        {
            Console.WriteLine("Testing overdetermined system...");
            
            // Consistent system: x + y = 3, 2x + y = 5, 3x + 2y = 8
            // Solution should be x = 2, y = 1
            var augmented = new double[][] {
                new double[] { 1.0, 1.0, 3.0 },
                new double[] { 2.0, 1.0, 5.0 },
                new double[] { 3.0, 2.0, 8.0 }
            };

            var result = Helpers.Solve(augmented, 3, 2);
            Console.WriteLine($"  Solution type: {result.Type}");
            Console.WriteLine($"  Message: {result.Message}");
            if (result.Solutions != null)
            {
                Console.WriteLine($"  Solutions: x = {result.Solutions[0]}, y = {result.Solutions[1]}");
                
                // Verify the solution
                double eq1 = result.Solutions[0] + result.Solutions[1];
                double eq2 = 2 * result.Solutions[0] + result.Solutions[1];
                double eq3 = 3 * result.Solutions[0] + 2 * result.Solutions[1];
                Console.WriteLine($"  Verification: eq1 = {eq1} (should be 3), eq2 = {eq2} (should be 5), eq3 = {eq3} (should be 8)");
            }
            
            Console.WriteLine("  ? Overdetermined system test completed");
        }
    }
}
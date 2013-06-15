using System;
using Solver.Models;

namespace Solver.Tools
{
    /*code is taken from http://www.codeproject.com/Tips/388179/Linear-Equation-Solver-Gaussian-Elimination-Csharp*/
    public static class LinearEquationSolver
    {
        /// <summary>Computes the solution of a linear equation system.</summary>
        /// <param name="M">
        /// The system of linear equations as an augmented matrix[row, col] where (rows + 1 == cols).
        /// It will contain the solution in "row canonical form" if the function returns "true".
        /// </param>
        /// <returns>Returns whether the matrix has a unique solution or not.</returns>
        public static bool Solve(float[,] M)
        {
            // input checks
            int rowCount = M.GetUpperBound(0) + 1;
            if (M == null || M.Length != rowCount * (rowCount + 1))
                throw new ArgumentException("The algorithm must be provided with a (n x n+1) matrix.");
            if (rowCount < 1)
                throw new Exception("System should have at least one equation");

            // pivoting
            for (int col = 0; col + 1 < rowCount; col++) 
                if (M[col, col] == 0)
                // check for zero coefficients
                {
                    // find non-zero coefficient
                    int swapRow = col + 1;
                    for (; swapRow < rowCount; swapRow++) 
                        if (M[swapRow, col] != 0) 
                            break;
                    if (swapRow == rowCount) throw new LinearSystemException("System has infinitely many solutions");
                        
                    if (M[swapRow, col] != 0) // found a non-zero coefficient?
                    {
                        // yes, then swap it with the above
                        float[] tmp = new float[rowCount + 1];
                        for (int i = 0; i < rowCount + 1; i++)
                        { tmp[i] = M[swapRow, i]; M[swapRow, i] = M[col, i]; M[col, i] = tmp[i]; }
                    }
                    else
                        throw new LinearSystemException("System has infinitely many solutions"); // no, then the matrix has no unique solution
                }

            // elimination
            for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
            {
                for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
                {
                    float df = M[sourceRow, sourceRow];
                    float sf = M[destRow, sourceRow];
                    for (int i = 0; i < rowCount + 1; i++)
                        M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
                }
            }

            //check if system is incompatible
            for (int row = rowCount - 1; row >= 0; row--)
            {
                if (M[row,rowCount] == 0) continue;
                var isCompatible = false;
                for (int index = rowCount-1; index >= 0; index --)
                {
                    if (M[row, index] != 0)
                    {
                        isCompatible = true;
                        break;
                    }
                }
                if (!isCompatible) return false;
            }
            

            // back-insertion
                for (int row = rowCount - 1; row >= 0; row--)
                {
                    
                    float f = M[row, row];
                    if (f == 0)
                    {
                        if (M[row, row + 1] != 0)
                            return false;
                        throw new LinearSystemException("System has infinitely many solutions");
                    }

                    for (int i = 0; i < rowCount + 1; i++) M[row, i] /= f;
                    for (int destRow = 0; destRow < row; destRow++)
                    { M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount]; M[destRow, row] = 0; }
                }
            return true;
        }
    }
}
using System;

namespace Solver.Models
{
    public class LinearSystemException : Exception
    {
         public LinearSystemException(string message) : base(message)
         {
            
         }
    }
}
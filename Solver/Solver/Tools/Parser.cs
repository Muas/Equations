using System;
using System.Collections.Generic;
using System.Linq;

namespace Solver.Tools
{
    public static class Parser
    {
        private static readonly char[] Signs = {'-','+','*','/','='};

        public static SortedSet<char> GetAllVariables(List<string> equations)
        {
            var variables = new SortedSet<char>();
             foreach (var equation in equations)
             {
                 foreach (var symbol in equation)
                 {
                     if (symbol >= 'a' && symbol <= 'z')
                         variables.Add(symbol);
                 }
             }
            return variables;
        }

        public static bool SystemIsCorrect(this List<string> equations)
        {
            foreach (var equation in equations)
            {
                foreach (var symbol in equation)
                {
                    if (!Signs.Any(x => x == symbol) && !(symbol >= 'a' && symbol <= 'z') && !(symbol >='0' && symbol <='9'))
                        return false;
                }
            }
            return true;
        }

        public static float[,] GetMatrix(this List<string> equations)
        {
            var variables = GetAllVariables(equations);
            float[,] matrix = new float[variables.Count,variables.Count + 1];
            var i = 0;
            foreach (var equation in equations)
            {
                var equalityPosition = equation.IndexOf('=');
                var conditionString = equation.Substring(0,equalityPosition);
                var resultString = equation.Substring(equalityPosition + 1);

                try
                {
                    var conditionList = ParseCondition(conditionString);
                    var result = Convert.ToSingle(resultString);
                    var j = 0;
                    foreach (var variable in variables)
                    {
                        if (conditionList.ContainsKey(variable))
                            matrix[i, j] = conditionList.ElementAt(conditionList.IndexOfKey(variable)).Value;
                        else matrix[i, j] = 0;
                        j++;
                    }
                    matrix[i, variables.Count] = result;
                }
                catch(Exception)
                {
                    
                }
                i++;
            }
            return matrix;
        }

        private static SortedList<char,float> ParseCondition(string conditionString)
        {
            var conditionList = new SortedList<char, float>();
            var i = 0;
            var currentCoefficient = string.Empty;
            while (i < conditionString.Length)
            {
                if (conditionString[i] >= '0' && conditionString[i] <= '9' || conditionString[i] == '.' || conditionString[i] == '-')
                {
                    currentCoefficient += conditionString[i];
                }
                if (conditionString[i] >= 'a' && conditionString[i] <= 'z')
                {
                    float coefficient;
                    if (currentCoefficient == "-")
                        coefficient = -1;
                    else if (currentCoefficient == string.Empty)
                        coefficient = 1;
                    else coefficient = Convert.ToSingle(currentCoefficient);
                    conditionList.Add(conditionString[i],coefficient);
                    currentCoefficient = string.Empty;
                }
                i++;
            }
            return conditionList;
        }
 

    }
}
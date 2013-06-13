using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Solver.Tools;

namespace Solver.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult GetResult(string equations)
        {
            var equationsList = new JavaScriptSerializer().Deserialize<IEnumerable<string>>(equations).ToList();
            RemoveWhiteSpaces(equationsList);
            if (!equationsList.SystemIsCorrect()) return Json(new {result = false, reason = "It's not a system"});
            if (!SystemIsSolvable(equationsList)) return Json(new { result = false, reason = "System cannot be solved" });
            float[,] matrix;
            bool isSolved = false;
            try
            {
                matrix = equationsList.GetMatrix();
                
            }
            catch(Exception e)
            {
                return Json(new {result = false, reason = e.Message});
            }
            try
            {
                isSolved = LinearEquationSolver.Solve(matrix);
            }
            catch (Exception)
            {
                return Json(new {result = false, reason = "This system cannot be solved"});
            }
            
            if (isSolved)
            {
                var variables = Parser.GetAllVariables(equationsList);
                var resultList = new List<string>();
                for (int i = 0; i < variables.Count; i++)
                {
                    resultList.Add(string.Format("{0}={1}", variables.ElementAt(i), matrix[i, variables.Count]));
                }
                return Json(new { result = true, reason = resultList });
            }
            
            return Json(new {result = false});
        }

        public void RemoveWhiteSpaces(List<string> equations)
        {
            int i = 0;
            while (i < equations.Count)
            {
                equations[i].Trim();
                if (string.IsNullOrWhiteSpace(equations[i]))
                {
                    equations.Remove(equations[i]);
                    continue;
                }
                equations[i].ToLower();
                var index = equations[i].IndexOf(' ');
                while (index > 0)
                {
                   equations[i] = equations[i].Remove(index, 1);
                   index = equations[i].IndexOf(' ');
                }
                i++;
            }
            
         }

        public bool SystemIsSolvable(List<string> equations)
        {
            var variables = Parser.GetAllVariables(equations);
            return variables.Any() && variables.Count <= equations.Count;
        }

    }
}

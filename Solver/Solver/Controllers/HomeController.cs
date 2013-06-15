using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Solver.Models;
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
            ChangeCommasToPoints(equationsList);
            if (!equationsList.SystemIsCorrect()) return Json(new {result = false, reason = new Reason{ isMathematical = false, reasonText =  "It's not a system"}});
            float[,] matrix;
            bool isSolved = false;
            try
            {
                matrix = equationsList.GetMatrix();
            }
            catch(Exception e)
            {
                if (e is LinearSystemException)
                {
                    return
                        Json(new { result = false, reason = new Reason { isMathematical = true, reasonText = e.Message } });
                }
                return Json(new {result = false, reason = new Reason{isMathematical = false, reasonText = e.Message}});
            }
            try
            {
                isSolved = LinearEquationSolver.Solve(matrix);
            }
            catch (Exception e)
            {
                if (e is LinearSystemException)
                {
                    return
                        Json(new {result = false, reason = new Reason {isMathematical = true, reasonText = e.Message}});
                }
                return Json(new {result = false, reason = new Reason{isMathematical = false, reasonText = e.Message}});
            }
            
            if (isSolved)
            {
                var variables = Parser.GetAllVariables(equationsList);
                string resultString = string.Empty;
                for (int i = 0; i < variables.Count; i++)
                {
                    resultString += string.Format("{0} = {1}\n", variables.ElementAt(i), matrix[i, variables.Count]);
                }
                return Json(new { result = true, solution = resultString});
            }
            
            return Json(new {result = false, reason = new Reason{isMathematical = true, reasonText = "System is incompatible"}});
        }

        private void ChangeCommasToPoints(List<string> equations)
        {
            for (int i = 0; i < equations.Count; i++)
            {
                equations[i] = equations[i].Replace(',', '.');
            }
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
    }
}

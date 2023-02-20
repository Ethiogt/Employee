using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Employee.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Employee.Controllers
{
    public static class SessionHelper
    {
        public static void SetEmployeeInSession(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetEmployeeFromSession<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private List<SalariedEmployeeModel> salariedEmployees = new List<SalariedEmployeeModel>();
        private List<HourlyEmployeeModel> hourlyEmployees = new List<HourlyEmployeeModel>();
        private List<ManagerEmployeeModel> managerEmployees = new List<ManagerEmployeeModel>();
        private SalariedEmployeeModel salariedEmployee = new SalariedEmployeeModel();
        private HourlyEmployeeModel hourlyEmployee = new HourlyEmployeeModel();
        private ManagerEmployeeModel managerEmployee = new ManagerEmployeeModel();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //HttpContext.Session.Remove("salariedEmployees");
            //HttpContext.Session.Remove("hourlyEmployees");
            //HttpContext.Session.Remove("managerEmployees");

            salariedEmployees = HttpContext.Session.GetEmployeeFromSession< List<SalariedEmployeeModel>>("salariedEmployees");
            hourlyEmployees = HttpContext.Session.GetEmployeeFromSession<List<HourlyEmployeeModel>>("hourlyEmployees");
            managerEmployees = HttpContext.Session.GetEmployeeFromSession<List<ManagerEmployeeModel>>("managerEmployees");

            if (salariedEmployees == null)
            {
                salariedEmployees = new List<SalariedEmployeeModel>();
                //salaries employee
                
                for (int i = 0; i < 10; i++)
                {
                    SalariedEmployeeModel salariedEmployee = new SalariedEmployeeModel();
                    salariedEmployee.employeeId = "s" + i;
                    salariedEmployee.firstName = "f" + i;
                    salariedEmployee.lastName = "l" + i;
                    salariedEmployee.SSN = "ssn" + i;
                    salariedEmployee.vacation = 0;
                    salariedEmployees.Add(salariedEmployee);
                }
            }

            if (hourlyEmployees == null)
            {
                hourlyEmployees = new List<HourlyEmployeeModel>();
                //hourly employee
                
                for (int i = 0; i < 10; i++)
                {
                    HourlyEmployeeModel hourlyEmployee = new HourlyEmployeeModel();
                    hourlyEmployee.employeeId = "h" + i;
                    hourlyEmployee.firstName = "f" + i;
                    hourlyEmployee.lastName = "l" + i;
                    hourlyEmployee.SSN = "hssn" + i;
                    hourlyEmployee.vacation = 0;
                    hourlyEmployees.Add(hourlyEmployee);
                }
            }
            if (managerEmployees == null)
            {
                managerEmployees = new List<ManagerEmployeeModel>();
                //managers
                
                for (int i = 0; i < 10; i++)
                {
                    ManagerEmployeeModel managerEmployee = new ManagerEmployeeModel();
                    managerEmployee.employeeId = "m" + i;
                    managerEmployee.managerId = "mm" + i;
                    managerEmployee.firstName = "f" + i;
                    managerEmployee.lastName = "l" + i;
                    managerEmployee.SSN = "mssn" + i;
                    managerEmployee.vacation = 0;
                    managerEmployees.Add(managerEmployee);
                }
            }
            HttpContext.Session.SetEmployeeInSession("salariedEmployees", salariedEmployees);
            HttpContext.Session.SetEmployeeInSession("hourlyEmployees", hourlyEmployees);
            HttpContext.Session.SetEmployeeInSession("managerEmployees", managerEmployees);
            var allEmployees = new Tuple<List<SalariedEmployeeModel>, List<HourlyEmployeeModel>, List<ManagerEmployeeModel>>(salariedEmployees, hourlyEmployees, managerEmployees);
            return View(allEmployees);
        }
        public IActionResult Work(string employeeId = null, string employeeType = null)
        {
          
            salariedEmployees = HttpContext.Session.GetEmployeeFromSession<List<SalariedEmployeeModel>>("salariedEmployees");
            hourlyEmployees = HttpContext.Session.GetEmployeeFromSession<List<HourlyEmployeeModel>>("hourlyEmployees");
            managerEmployees = HttpContext.Session.GetEmployeeFromSession<List<ManagerEmployeeModel>>("managerEmployees");
            if (employeeType == null || (employeeType != "S" && employeeType != "H" && employeeType != "M"))
                return RedirectToAction("index", "Home");

            if (employeeType == "S") {
                salariedEmployee = salariedEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                if (salariedEmployee == null)
                    return RedirectToAction("index", "Home");
                //Work(employeeId, employeeType);
            }
            else if (employeeType == "H") {
                hourlyEmployee = hourlyEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                if (hourlyEmployee == null)
                    return RedirectToAction("index", "Home");
            }
            else if (employeeType == "M") {
                managerEmployee = managerEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                if (managerEmployee == null)
                    return RedirectToAction("index", "Home");
            }
            ViewBag.employeeType = employeeType;
            var employees = new Tuple<SalariedEmployeeModel, HourlyEmployeeModel, ManagerEmployeeModel>(salariedEmployee, hourlyEmployee, managerEmployee);
            return View("Work",employees);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Work(string employeeId = null, string employeeType = null,
            [Bind(Prefix = "Item1")] SalariedEmployeeModel salariedEmployeeModel = null,
            [Bind(Prefix = "Item2")] HourlyEmployeeModel hourlyEmployeeModel = null,
            [Bind(Prefix = "Item3")] ManagerEmployeeModel managerEmployeeModel = null)
        {
          
            salariedEmployees = HttpContext.Session.GetEmployeeFromSession<List<SalariedEmployeeModel>>("salariedEmployees");
            hourlyEmployees = HttpContext.Session.GetEmployeeFromSession<List<HourlyEmployeeModel>>("hourlyEmployees");
            managerEmployees = HttpContext.Session.GetEmployeeFromSession<List<ManagerEmployeeModel>>("managerEmployees");
            var employees = new Tuple<SalariedEmployeeModel, HourlyEmployeeModel, ManagerEmployeeModel>(salariedEmployee, hourlyEmployee, managerEmployee);

            double vacation = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    if (employeeType == null || (employeeType != "S" && employeeType != "H" && employeeType != "M"))
                        return RedirectToAction("index", "Home");

                    if (employeeType == "S")
                    {
                        salariedEmployee = salariedEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                        if (salariedEmployee == null)
                            return RedirectToAction("index","Home");
                        vacation = CalculateVacation(salariedEmployeeModel.numberofWorkedDays, employeeType);
                        salariedEmployee.numberofWorkedDays = salariedEmployeeModel.numberofWorkedDays;
                        //salariedEmployee.numberofVacationDays = (salariedEmployeeModel.numberofVacationDays < 0 ? salariedEmployee.numberofVacationDays : salariedEmployeeModel.numberofVacationDays);
                        salariedEmployee.vacation = (vacation - salariedEmployee.numberofVacationDays) < 0 ? 0 : (vacation - salariedEmployee.numberofVacationDays);
                    }
                    else if (employeeType == "H")
                    {
                        hourlyEmployee = hourlyEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                        if (hourlyEmployee == null)
                            return RedirectToAction("index", "Home");
                        vacation = CalculateVacation(hourlyEmployeeModel.numberofWorkedDays, employeeType);
                        hourlyEmployee.numberofWorkedDays = hourlyEmployeeModel.numberofWorkedDays;
                        //hourlyEmployee.numberofVacationDays = (hourlyEmployeeModel.numberofVacationDays < 0 ? hourlyEmployeeModel.numberofVacationDays : hourlyEmployeeModel.numberofVacationDays);
                        hourlyEmployee.vacation = (vacation - hourlyEmployee.numberofVacationDays) < 0 ? 0: (vacation - hourlyEmployee.numberofVacationDays);
                    }
                    else if (employeeType == "M")
                    {
                        managerEmployee = managerEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                        if (managerEmployee == null)
                            return RedirectToAction("index", "Home");
                        vacation = CalculateVacation(managerEmployeeModel.numberofWorkedDays, employeeType);
                        managerEmployee.numberofWorkedDays = managerEmployeeModel.numberofWorkedDays;
                        //managerEmployee.numberofVacationDays = (managerEmployeeModel.numberofVacationDays < 0 ? managerEmployeeModel.numberofVacationDays : managerEmployeeModel.numberofVacationDays);
                        managerEmployee.vacation = (vacation - managerEmployeeModel.numberofVacationDays) < 0 ? 0 : (vacation - managerEmployeeModel.numberofVacationDays);

                    }
                    employees = new Tuple<SalariedEmployeeModel, HourlyEmployeeModel, ManagerEmployeeModel>(salariedEmployee, hourlyEmployee, managerEmployee);
                    ViewBag.successMsg = "Employee worked days info has been update";
                }

            }
            catch (Exception)
            {
                ViewBag.errorMsg = "Employee  worked days info hasn't been update"; ;
                return View("Work", employees);
            }
            ViewBag.employeeType = employeeType;
            HttpContext.Session.SetEmployeeInSession("salariedEmployees", salariedEmployees);
            HttpContext.Session.SetEmployeeInSession("hourlyEmployees", hourlyEmployees);
            HttpContext.Session.SetEmployeeInSession("managerEmployees", managerEmployees);
            return View("Work", employees);

        }
        
        public IActionResult TakeVacation(string employeeId = null, string employeeType = null)
        {
            string empId = employeeId;
            List<SalariedEmployeeModel> salariedEmployees = new List<SalariedEmployeeModel>();
            List<HourlyEmployeeModel> hourlyEmployees = new List<HourlyEmployeeModel>();
            List<ManagerEmployeeModel> managerEmployees = new List<ManagerEmployeeModel>();
            SalariedEmployeeModel salariedEmployee = new SalariedEmployeeModel();
            HourlyEmployeeModel hourlyEmployee = new HourlyEmployeeModel();
            ManagerEmployeeModel managerEmployee = new ManagerEmployeeModel();

            salariedEmployees = HttpContext.Session.GetEmployeeFromSession<List<SalariedEmployeeModel>>("salariedEmployees");
            hourlyEmployees = HttpContext.Session.GetEmployeeFromSession<List<HourlyEmployeeModel>>("hourlyEmployees");
            managerEmployees = HttpContext.Session.GetEmployeeFromSession<List<ManagerEmployeeModel>>("managerEmployees");
            if (employeeType == null || (employeeType != "S" && employeeType != "H" && employeeType != "M"))
                return RedirectToAction("index", "Home");

            if (employeeType == "S")
            {
                salariedEmployee = salariedEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                if (salariedEmployee == null)
                    return RedirectToAction("index", "Home");
            }
            else if (employeeType == "H")
            {
                hourlyEmployee = hourlyEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                if (hourlyEmployee == null)
                    return RedirectToAction("index", "Home");
            }
            else if (employeeType == "M")
            {
                managerEmployee = managerEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                if (managerEmployee == null)
                    return RedirectToAction("index", "Home");
            }
            ViewBag.employeeType = employeeType;
            var Employees = new Tuple<SalariedEmployeeModel, HourlyEmployeeModel, ManagerEmployeeModel>(salariedEmployee, hourlyEmployee, managerEmployee);
            return View("TakeVacation", Employees);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TakeVacation(string employeeId = null, string employeeType = null,
            [Bind(Prefix = "Item1")] SalariedEmployeeModel salariedEmployeeModel = null,
            [Bind(Prefix = "Item2")] HourlyEmployeeModel hourlyEmployeeModel = null,
            [Bind(Prefix = "Item3")] ManagerEmployeeModel managerEmployeeModel = null)
        {
            double vacation = 0;
            salariedEmployees = HttpContext.Session.GetEmployeeFromSession<List<SalariedEmployeeModel>>("salariedEmployees");
            hourlyEmployees = HttpContext.Session.GetEmployeeFromSession<List<HourlyEmployeeModel>>("hourlyEmployees");
            managerEmployees = HttpContext.Session.GetEmployeeFromSession<List<ManagerEmployeeModel>>("managerEmployees");
            var employees = new Tuple<SalariedEmployeeModel, HourlyEmployeeModel, ManagerEmployeeModel>(salariedEmployee, hourlyEmployee, managerEmployee);
            try
            {
                if (ModelState.IsValid)
                {
                    if (employeeType == null || (employeeType != "S" && employeeType != "H" && employeeType != "M"))
                        return RedirectToAction("index", "Home");

                    if (employeeType == "S")
                    {
                        salariedEmployee = salariedEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                        if (salariedEmployee == null)
                            return RedirectToAction("index", "Home");
                        vacation = CalculateVacation(salariedEmployee.numberofWorkedDays, employeeType);
                        salariedEmployee.numberofVacationDays = (salariedEmployeeModel.numberofVacationDays < 0 ? salariedEmployee.numberofVacationDays : salariedEmployeeModel.numberofVacationDays);
                        salariedEmployee.vacation = (vacation - salariedEmployee.numberofVacationDays) < 0 ? 0: (vacation - salariedEmployee.numberofVacationDays);
                    }
                    else if (employeeType == "H")
                    {
                        hourlyEmployee = hourlyEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                        if (hourlyEmployee == null)
                            return RedirectToAction("index", "Home");
                        hourlyEmployee.numberofVacationDays = (hourlyEmployeeModel.numberofVacationDays < 0 ? hourlyEmployeeModel.numberofVacationDays : hourlyEmployeeModel.numberofVacationDays);
                        vacation = CalculateVacation(hourlyEmployeeModel.numberofWorkedDays, employeeType);
                        hourlyEmployee.vacation = (vacation - hourlyEmployee.numberofVacationDays) < 0 ? 0 : (vacation - hourlyEmployee.numberofVacationDays);
                    }
                    else if (employeeType == "M")
                    {
                        managerEmployee = managerEmployees.Where(s => s.employeeId == employeeId).FirstOrDefault();
                        if (managerEmployee == null)
                            return RedirectToAction("index", "Home");
                        vacation = CalculateVacation(managerEmployee.numberofWorkedDays, employeeType);
                        managerEmployee.numberofVacationDays = (managerEmployeeModel.numberofVacationDays < 0 ? managerEmployeeModel.numberofVacationDays : managerEmployeeModel.numberofVacationDays);
                        managerEmployee.vacation = (vacation - managerEmployeeModel.numberofVacationDays) < 0 ? 0 : (vacation - managerEmployeeModel.numberofVacationDays);

                    }
                    employees = new Tuple<SalariedEmployeeModel, HourlyEmployeeModel, ManagerEmployeeModel>(salariedEmployee, hourlyEmployee, managerEmployee);
                    ViewBag.successMsg = "Employee vacation info has been update";
                }
            }
            catch (Exception)
            {
                ViewBag.errorMsg = "Employee vacation info hasn't been update"; ;
                return View("TakeVacation", employees);
            }
            ViewBag.employeeType = employeeType;
            HttpContext.Session.SetEmployeeInSession("salariedEmployees", salariedEmployees);
            HttpContext.Session.SetEmployeeInSession("hourlyEmployees", hourlyEmployees);
            HttpContext.Session.SetEmployeeInSession("managerEmployees", managerEmployees);
            return View("TakeVacation", employees);

        }
        public double CalculateVacation(int workedDays, string employeeType)
        {
            double vacation = 0;
            if (employeeType == "S")
                vacation = (EmployeeVacationDays.salariedEmployeeVacation * workedDays) / 260;
            else if (employeeType == "H")
                vacation = (EmployeeVacationDays.hourlyEmployeeVacation * workedDays) / 260;
            else if (employeeType == "M")
                vacation = (EmployeeVacationDays.managerEmployeeVacation * workedDays) / 260;

            return vacation;
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

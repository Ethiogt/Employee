using System;
using System.ComponentModel.DataAnnotations;
namespace Employee.Models
{
    public class EmployeeModel
    {
        [Display(Name="Employee ID")]
        public string employeeId { get; set; }
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "last Name")]
        public string lastName { get; set; }
        [Display(Name = "SSN")]
        public string SSN { get; set; }
        [Display(Name = "Vacation Days left")]
        public double vacation { get; set; }
        [Display(Name = "number of Worked Days")]
        [Range(0, 260, ErrorMessage = "Worked Days Must be between 0 to 260")]
        public int numberofWorkedDays { get; set; }
        [Display(Name = "Vacation Days Used")]
        [Range(0, 30, ErrorMessage = "Vacation Days Must be between 0 to 30")]
        public int numberofVacationDays { get; set; }
    }
    public class SalariedEmployeeModel : EmployeeModel {
        public double weeklySalary { get; set; }
       
    }
    public class HourlyEmployeeModel : EmployeeModel
    {
        public double wage { get; set; }
        public double hours { get; set; }
       
    }
    public class ManagerEmployeeModel : EmployeeModel
    {
        public string managerId { get; set; }
        public string title { get; set; }
        
    }
    public static class EmployeeVacationDays {
        public static int salariedEmployeeVacation = 10;
        public static int hourlyEmployeeVacation = 15;
        public static int managerEmployeeVacation = 30;

    }
}

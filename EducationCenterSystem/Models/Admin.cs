using System;

namespace EducationSystem.Models
{
    public class Admin : Person
    {
        public double Salary { get; set; }
        public string WorkType { get; set; }
        public int WorkingHours { get; set; }

        public override string GetRole() => "Admin";

        public override string GetInfo()
        {
            return $"[Admin] {Name} | {Phone} | {Email} | Salary: {Salary} | {WorkType} | Hours: {WorkingHours}";
        }
    }
}
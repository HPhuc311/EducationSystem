using System;

namespace EducationSystem.Models
{
    public class Teacher : Person
    {
        public double Salary { get; set; }
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }

        public override string GetRole() => "Teacher";

        public override string GetInfo()
        {
            return $"[Teacher] {Name} | {Phone} | {Email} | Salary: {Salary} | {Subject1}, {Subject2}";
        }
    }
}
using System;

namespace EducationSystem.Models
{
    public class Student : Person
    {
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }
        public string Subject3 { get; set; }

        public override string GetRole() => "Student";

        public override string GetInfo()
        {
            return $"[Student] {Name} | {Phone} | {Email} | {Subject1}, {Subject2}, {Subject3}";
        }
    }
}
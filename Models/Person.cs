using System;

namespace EducationSystem.Models
{
    // Base abstract class
    public abstract class Person
    {
        // Encapsulation
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Polymorphism
        public abstract string GetInfo();
        public abstract string GetRole();
    }
}
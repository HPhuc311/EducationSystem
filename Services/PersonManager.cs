using System.Collections.Generic;
using System.Linq;
using EducationSystem.Models;

namespace EducationSystem.Services
{
    public class PersonManager
    {
        // List used to store all Person objects (Teacher, Admin, Student)
        private List<Person> people = new List<Person>();

        // Add a new person into the list
        public void Add(Person p)
        {
            people.Add(p);
        }

        // Return all people in the system
        public List<Person> GetAllPeople()
        {
            return people;
        }

        // Return people filtered by role (Teacher/Admin/Student)
        public List<Person> GetPeopleByRole(string role)
        {
            return people.Where(p => p.GetRole() == role).ToList();
        }

        // Remove a person from the list
        public void Delete(Person p)
        {
            if (p != null)
                people.Remove(p);
        }

        // Search people by keyword (name, email, or phone)
        public List<Person> Search(string keyword)
        {
            keyword = keyword.ToLower();

            return people.Where(p =>
                p.Name.ToLower().Contains(keyword) ||
                p.Email.ToLower().Contains(keyword) ||
                p.Phone.ToLower().Contains(keyword)
            ).ToList();
        }
    }
}
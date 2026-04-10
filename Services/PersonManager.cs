using System.Collections.Generic;
using System.Linq;
using EducationSystem.Models;

namespace EducationSystem.Services
{
    public class PersonManager
    {
        // Data structure: dynamic list
        private List<Person> people = new List<Person>();

        public void Add(Person p)
        {
            people.Add(p);
        }

        public List<Person> GetAll()
        {
            return people;
        }

        public List<Person> GetByRole(string role)
        {
            return people.Where(p => p.GetRole() == role).ToList();
        }

        public void Delete(Person p)
        {
            if (p != null)
                people.Remove(p);
        }

        public Person Get(int index)
        {
            if (index >= 0 && index < people.Count)
                return people[index];
            return null;
        }
    }
}
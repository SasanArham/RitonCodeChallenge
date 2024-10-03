using Domain.Base;
using Domain.Modules.ContactManagement.People.Exceptions;

namespace Domain.Modules.ContactManagement.People
{
    public class Person : BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }

        public Person()
        {

        }

        public static Person Create(string name, string lastName)
        {
            EnforceInvariant(name, lastName);
            var person = new Person
            {
                Name = name,
                LastName = lastName,
                ID = Guid.NewGuid().ToString()
            };
            return person;
        }

        private static void EnforceInvariant(string name, string lastName)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(lastName))
            {
                throw new PersonMustHaveNameOrLastNameException();
            }
        }

    }
}

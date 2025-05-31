using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Address { get; set; }
        public string ContactNumber{ get; set; }

        public virtual string DisplayInfo()
        {
            return  $"Name: {FirstName} {MiddleName} {LastName}\n" +
                    $"Address: {Address}\n" +
                    $"ContactNumber: {ContactNumber}";
        }
    }
}

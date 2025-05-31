using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    public class Guardian : Person
    {
        public int Id { get; set; }
        public string Relationship { get; set; }    
        public Guardian() { }

        public Guardian(int id, string fname, string mname, string lname, string relationship, string address, string contact) 
        {
            Id = id;
            FirstName = fname;
            MiddleName = mname;
            LastName = lname;
            Relationship = relationship;
            Address = address;
            ContactNumber = contact;
        }

        public override string DisplayInfo()
        {
            return base.DisplayInfo() + $"\nRelationship: {Relationship}"; ;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    internal class StudentRegistration : Registration
    {
        private Requirements _requirements;

        public StudentRegistration(Requirements requirements)
        {
            _requirements = requirements;
        }

        public override bool ValidateRequirements()
        {
            return _requirements.AllSubmitted();
        }
    }
}

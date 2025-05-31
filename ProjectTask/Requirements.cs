using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    public class Requirements
    {
        public int Id { get; set; }
        public bool BirthCertificate { get; set; }
        public bool TOR {  get; set; }
        public bool GoodMoral { get; set; }

        public bool AllSubmitted()
        {
            return BirthCertificate && TOR && GoodMoral;
        }
    }
}

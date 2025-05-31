using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    public class ProgramInfo
    {
        public int Id { get; set; }
        public string ProgramName { get; set; }
        public decimal TuitionFee { get; set; }

        public ProgramInfo() { }

        public ProgramInfo(int id, string name, decimal fee)
        {
            Id = id;
            ProgramName = name;
            TuitionFee = fee;
        }
    }
}

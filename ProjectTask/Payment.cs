using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTask
{
    public class Payment
    {

        public int Id { get; set; }
        public decimal AmountPaid { get; set; }
        public string Method { get; set; }
        public decimal ScholarshipDiscount { get; set; }


        public Payment() { }


        public Payment(int id, decimal amountPaid, string method, decimal discount)
        {
            Id = id;
            AmountPaid = amountPaid;
            Method = method;
            ScholarshipDiscount = discount;
        }

        public decimal CalculateBalance(decimal tuitionFee)
        {
            decimal discountAmount =  tuitionFee * ScholarshipDiscount;
            return (tuitionFee - discountAmount) - AmountPaid;
        }
    }
}

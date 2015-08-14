using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAgency.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }
}

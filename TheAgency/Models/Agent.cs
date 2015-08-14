using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TheAgency.Models
{
   public class Agent
    {
        public int AgentID { get; set; }

        [Required]
        [StringLength(30)]
        public string CodeName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$")]
        public string FirstName { get; set; }
        [RegularExpression("^[a-zA-Z]+$")]
        public string LastName { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public IList<Task> Tasks { get; set; }
    }
}

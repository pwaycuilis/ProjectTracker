using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Models
{
    public class Ticket
    {
		[Key]
		public int Id { get; set; }
		[Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
		public string Name { get; set; }
		public string Description { get; set; }
		public string Priority { get; set; }
		public string Status { get; set; }
		public string Type { get; set; }
		
		public DateTime Created { get; set; }
		public DateTime Modified { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "You must choose a project")]
		public int ProjectId { get; set; }

		[ForeignKey("ProjectId")]
		public virtual Project Project { get; set; }
		public string Submitter { get; set; }
	}
}

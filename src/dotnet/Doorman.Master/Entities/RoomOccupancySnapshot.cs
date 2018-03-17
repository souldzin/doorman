using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Doorman.Master.Entities
{
    public class RoomOccupancySnapshot
    {
	    [Key]
	    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RoomOccupancySnapshotId { get; set; }
		[ForeignKey("Room")]
		public int RoomId { get; set; }
	    [Required]
		public int Count { get; set; }
	    [Required]
		public DateTime CreateDateTime { get; set; }
		public virtual Room Room { get; set; }

	}
}

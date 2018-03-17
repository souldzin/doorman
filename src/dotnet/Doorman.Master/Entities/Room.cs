using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Doorman.Master.Entities
{
    public class Room
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RoomId { get; set; }
		[Required]
		public string Description { get; set; }
    }
}

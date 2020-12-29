using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NGA.MonolithAPI.SignalR
{
    public class Connection
    {
        [Key]
        public string ConnectionID { get; set; }
        public Guid UserId { get; set; }
        public bool Connected { get; set; }
        public Guid? GroupId { get; set; }
    }
}

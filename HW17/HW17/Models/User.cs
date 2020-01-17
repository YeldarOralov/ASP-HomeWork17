using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW17.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Login { get; set; }
        public string Password { get; set; }
    }
}

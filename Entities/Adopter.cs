using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkDapper.Entities
{
    public class Adopter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<Dog> Dogs { get; set; } = new List<Dog>();
    }
}

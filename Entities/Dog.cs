using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkDapper.Entities
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string DogBreed { get; set; }
        public bool IsAdopted { get; set; }

        public int? AdopterId { get; set; }  
        public Adopter Adopter { get; set; }

        public override string ToString()
        {
            return $"<----{Id}----->\nName of Dog: {Name} -- {Age}\nDog breed: {DogBreed} -- Is Adopted: {IsAdopted}";
        }
    }
}

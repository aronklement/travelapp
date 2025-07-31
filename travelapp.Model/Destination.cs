using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace travelapp.Model
{
    public class Destination
    {
        public Destination(string city, string country, int distance, float price, bool discounted, int popularity)
        {
            this.city = city;
            this.country = country;
            this.distance = distance;
            this.price = price;
            this.discounted = discounted;
            this.popularity = popularity;
            destinationID = Guid.NewGuid().ToString();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string destinationID { get; set; }
        public string airlineName { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public int distance { get; set; }
        public float price { get; set; }
        public bool discounted { get; set; }
        public int popularity { get; set; }
    }
}

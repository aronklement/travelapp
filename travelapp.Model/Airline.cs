using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace travelapp.Model
{
    public class Airline
    {
        public Airline(string airlineName)
        {
            this.airlineName = airlineName;
            destinations = new HashSet<Destination>();
            ID = Guid.NewGuid().ToString();
        }
        public Airline()
        {
            destinations = new HashSet<Destination>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public string airlineName { get; set; }
        public virtual ICollection<Destination> destinations { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Campus_Events.Models
{
    public abstract class Entity
    {
        [Key]
        public Guid ID { get; set; }
    }
}

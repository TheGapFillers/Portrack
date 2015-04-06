using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGapFillers.Portrack.Models.Identity
{
    public class Audience
    {
        public Guid AudienceId { get; set; }

        public string Base64Secret { get; set; }

        public string Name { get; set; }

        public ICollection<CustomIdentityUser> Users { get; set; }
    }

    public class AudienceModel
    {
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
    }
}

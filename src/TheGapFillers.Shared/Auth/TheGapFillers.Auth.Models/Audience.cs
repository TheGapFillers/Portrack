using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheGapFillers.Auth.Models
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

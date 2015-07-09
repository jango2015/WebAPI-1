using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPITutoria.Models.Domain
{
    public class RockBand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Prize> Prizes { get; set; }

    }
}
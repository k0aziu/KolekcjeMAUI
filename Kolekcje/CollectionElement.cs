using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolekcje
{
    public class CollectionElement
    {
        public string Name { get; set; }
        public string State { get; set; } = "Nowy";
        public decimal Price { get; set; } = 0;
    }
}

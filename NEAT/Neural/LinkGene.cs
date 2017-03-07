using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    struct LinkGene : IComparable<LinkGene>, IEquatable<LinkGene>
    {
        private static int GlobalInnovation = 0;

        public int Innovation { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }
        public double Weight { get; set; }
        public bool Disabled { get; set; }

        public LinkGene(int s, int d, double w)
        {
            Source = s;
            Destination = d;
            Weight = d;
            Disabled = false;
            Innovation = GlobalInnovation++;
        }

        public int CompareTo(LinkGene other)
        {
            return Innovation - other.Innovation;
        }

        public bool Equals(LinkGene other)
        {
            return Innovation == other.Innovation;
        }

        public static bool operator ==(LinkGene a, LinkGene b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LinkGene a, LinkGene b)
        {
            return !a.Equals(b);
        }
    }
}

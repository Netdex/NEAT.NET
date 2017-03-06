using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    struct Gene : IComparable<Gene>, IEquatable<Gene>
    {
        private static int GlobalInnovation = 0;

        public int Innovation { get; private set; }
        public int Source { get; set; }
        public int Destination { get; set; }
        public double Weight { get; set; }

        public Gene(int s, int d, double w)
        {
            Source = s;
            Destination = d;
            Weight = d;
            Innovation = GlobalInnovation++;
        }

        public int CompareTo(Gene other)
        {
            return Innovation - other.Innovation;
        }

        public bool Equals(Gene other)
        {
            return Innovation == other.Innovation;
        }

        public static bool operator ==(Gene a, Gene b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Gene a, Gene b)
        {
            return !a.Equals(b);
        }
    }
}

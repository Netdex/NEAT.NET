using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAT.Neural;

namespace NEAT
{
    class Program
    {
        static void Main(string[] args)
        {
            Genome a = new Genome();
            AddDummyWithInnovation(a, 1);
            AddDummyWithInnovation(a, 2);
            AddDummyWithInnovation(a, 3);
            AddDummyWithInnovation(a, 5);
            AddDummyWithInnovation(a, 6);
            AddDummyWithInnovation(a, 7);
            AddDummyWithInnovation(a, 10);
            AddDummyWithInnovation(a, 11);
            Genome b = new Genome();
            AddDummyWithInnovation(b, 1);
            AddDummyWithInnovation(b, 2);
            AddDummyWithInnovation(b, 3);
            AddDummyWithInnovation(b, 4);
            AddDummyWithInnovation(b, 6);
            AddDummyWithInnovation(b, 7);
            AddDummyWithInnovation(b, 8);
            AddDummyWithInnovation(b, 9);
            Genome.GetCompatibility(a, b);
        }

        public static void AddDummyWithInnovation(Genome g, int innovation)
        {
            g.LinkGenotype.Add(new LinkGene(0, 0, 0) {Innovation = innovation});
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    class Genome
    {
        public Random RandomSource = new Random();
        public SortedSet<Gene> GeneticInfo { get; set; } = new SortedSet<Gene>();
        public int NeuronCount { get; set; } = 0;

        private const double EXCESS_WEIGHT = 1.0;
        private const double DISJOINT_WEIGHT = 1.0;
        private const double WEIGHT_DIFF_WEIGHT = 0.4;

        public Genome()
        {
            
        }

        public static double GetCompatibility(Genome a, Genome b)
        {
            int disjoint = 0;
            int excess = 0;
            SortedSet<Gene> disj =new SortedSet<Gene>(a.GeneticInfo);
            disj.SymmetricExceptWith(b.GeneticInfo);
            int maxInnovation = Math.Min(a.GeneticInfo.Last().Innovation, b.GeneticInfo.Last().Innovation);
            
            foreach (Gene g in disj)
            {
                if (g.Innovation > maxInnovation)
                    excess++;
                else
                    disjoint++;
            }
            SortedSet<Gene> common = new SortedSet<Gene>(a.GeneticInfo);
            common.IntersectWith(b.GeneticInfo);
            return 0;
        }
    }
}

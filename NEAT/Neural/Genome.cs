using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    class Genome
    {
        public Random RandomSource = new Random();
        public List<Gene> Genetic { get; set; } = new List<Gene>();

        private const double EXCESS_WEIGHT = 1.0;
        private const double DISJOINT_WEIGHT = 1.0;
        private const double WEIGHT_DIFF_WEIGHT = 0.4;

        public Genome()
        {
            
        }

        /*
         * Rank compatibility between two genomes
         */
        public static double GetCompatibility(Genome a, Genome b)
        {
            int D = 0;
            int E = 0;
            double WS = 0;
            int WC = 0;
            // Count number of disjoint genes
            int ac = 0, bc = 0;
            while (ac < a.Genetic.Count && bc < b.Genetic.Count)
            {
                if (a.Genetic[ac].Innovation > b.Genetic[bc].Innovation)
                {
                    bc++;
                    D++;
                }
                else if (a.Genetic[ac].Innovation < b.Genetic[bc].Innovation)
                {
                    ac++;
                    D++;
                }
                else
                {
                    // Add to mean weight differences between matching genes
                    WS += Math.Abs(a.Genetic[ac].Weight - b.Genetic[bc].Weight);
                    WC++;
                    ac++;
                    bc++;
                }
            }
            // Count number of matching genes
            E += a.Genetic.Count - ac;
            E += b.Genetic.Count - bc;

            int N = Math.Max(a.Genetic.Count, b.Genetic.Count);
            // Console.WriteLine($"D:{D} E:{E} W/:{WS/WC} N:{N}");
            return EXCESS_WEIGHT * E / N + DISJOINT_WEIGHT * D / N + WEIGHT_DIFF_WEIGHT * (WS / WC);
        }
    }
}

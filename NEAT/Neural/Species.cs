using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    class Species
    {
        // Chance for crossover to occur between two genomes during mating
        private const double CrossoverChance = 0.75;
        // The bottom x percentage of this species will be wiped out on reproduction
        private const double WeakCullPercentage = 0.25; // TODO NOT USED

        public Genome AmbassadorGenome { get; set; } = null;
        public List<Genome> Genomes { get; set; } = new List<Genome>();

        public IFitnessEvaluator FitnessEvaluator;
        public double[] Fitness;

        public Species(Genome ambassador)
        {
            AmbassadorGenome = ambassador;
            Genomes.Add(ambassador);
        }

        public void EvaluateFitness()
        {
            for (int i = 0; i < Genomes.Count; i++)
            {
                Genomes[i].Fitness = FitnessEvaluator.EvaluateFitness(Genomes[i]);
            }
        }

        public double AverageFitness()
        {
            double s = 0;
            for (int i = 0; i < Genomes.Count; i++)
                s += Genomes[i].Fitness;
            return s / Genomes.Count;
        }

        /*
         * Remove weak members of the population.
         * Fitness MUST be evaluated first.
         * TODO This only culls the lower half of the population, which may be too much, find a more efficient way to cull smaller portions
         */
        public void CullWeak()
        {
            double avg = AverageFitness();
            for (int i = 0; i < Genomes.Count; i++)
            {
                if (Genomes[i].Fitness < avg)
                {
                    Genomes.RemoveAt(i);
                    i--;
                }
            }
        }

        /*
         * Generates offspring between two random members
         */
        public Genome GetNextOffspring()
        {
            if (Genomes.Count == 1)
                return Genomes[0];

            // TODO
            int ridxa = NEATNET.Random.Next(Genomes.Count);
            int ridxb = ridxa;
            while (ridxa == ridxb)
                ridxb = NEATNET.Random.Next(Genomes.Count);
            return Genome.Crossover(Genomes[ridxa], Genomes[ridxb]);
        }

        public double GetLocalFitnessMetric(double fitness)
        {
            return fitness / Genomes.Count;
        }

        public double GetSpeciesCompatibility(Genome o)
        {
            return Genome.GetCompatibility(AmbassadorGenome, o);
        }
    }
}

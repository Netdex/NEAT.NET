using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    class Species
    {
        // The bottom x percentage of this species will be wiped out on reproduction
        private const double WeakCullPercentage = 0.25;

        public Genome AmbassadorGenome { get; set; } = null;
        public List<Genome> Genomes { get; set; } = new List<Genome>();

        public IFitnessEvaluator FitnessEvaluator;
        public double[] LocalFitness;

        public Species(Genome ambassador)
        {
            AmbassadorGenome = ambassador;
            Genomes.Add(ambassador);
        }

        public void EvaluateFitness()
        {
            for (int i = 0; i < Genomes.Count; i++)
            {
                LocalFitness[i] = GetLocalFitnessMetric(FitnessEvaluator.EvaluateFitness(Genomes[i]));
            }
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

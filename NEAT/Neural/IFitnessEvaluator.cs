using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    interface IFitnessEvaluator
    {
        double EvaluateFitness(Genome g);
    }
}

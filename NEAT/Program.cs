using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEAT.Debug;
using NEAT.Neural;

namespace NEAT
{
    class Program
    {
        static void Main(string[] args)
        {
            Genome a = new Genome(1,1);
            for(int i = 0; i < 20; i++)
                a.Mutate();
            //Genome b = new Genome(1,1);
            //for (int i = 0; i < 10; i++)
            //    b.Mutate();
            //var o = a.EvaluateNeuralNetwork(new double[] {1});
            //var c = Genome.Crossover(a, b);
            //ObjectDumper.Write(o);
            foreach (LinkGene lg in a.LinkGenotype)
            {
                Console.WriteLine($"{lg.Innovation} {lg.Source} -> {lg.Destination}");
            }
            new NeuralNetworkVisualization(a).ShowDialog();
        }

        public static void AddDummyWithInnovation(Genome g, int innovation)
        {
            g.LinkGenotype.Add(new LinkGene(0, 0, 0) {Innovation = innovation});
        }
    }
}

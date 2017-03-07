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

        public List<LinkGene> LinkGenotype { get; set; } = new List<LinkGene>();
        public List<NodeGene> NodeGenotype { get; set; } = new List<NodeGene>();

        public readonly int InputNodeCount;
        public readonly int OutputNodeCount;
        private int NextNeuronID = 0;

        private const double ExcessWeight = 1.0;
        private const double DisjointWeight = 1.0;
        private const double WeightDiffWeight = 0.4;

        // Chance for crossover to occur between two genomes during mating
        private const double CrossoverChance = 0.75;
        // Chance for a disabled gene to re-enable
        private const double GeneEnableChance = 0.2;
        // Chance for an enabled gene to disable
        private const double GeneDisableChance = 0.4;
        // Chance for a link's weight to mutate to a random value
        private const double WeightMutationChance = 0.9;
        // Chance for a node to be added along a random link
        private const double AddNodeMutationChance = 0.5;
        // Chance for two nodes to be linked randomly
        private const double AddLinkMutationChance = 1.0;
        // Chance for a node to connect to the bias input
        private const double BiasMutationChance = 0.4;

        public Genome(int inputNodeCount, int outputNodeCount)
        {
            InputNodeCount = inputNodeCount;
            OutputNodeCount = outputNodeCount;
            if (InputNodeCount == 0 || OutputNodeCount == 0)
                throw new ArgumentException("Invalid input/output count of NN");
        }

        /*
         * Initiate structures for input and output nodes
         */
        public void InitializeUtilityNodes()
        {
            for (int i = 0; i < InputNodeCount; i++)
            {
                AddNode(NodeType.Input);
            }
            for (int i = 0; i < OutputNodeCount; i++)
            {
                AddNode(NodeType.Output);
            }
        }
        public void Mutate()
        {
            if (NEATNET.Random.NextDouble() < AddNodeMutationChance)
            {
                if (LinkGenotype.Count > 0)
                {
                    // Pick random link
                    int ridx = NEATNET.Random.Next(LinkGenotype.Count);
                    LinkGene a = LinkGenotype[ridx];
                    // Add an intermediate node in the link
                    AddIntermediateNode(a);
                }
            }
            if (NEATNET.Random.NextDouble() < AddLinkMutationChance)
            {
                // Pick two unique neurons, there are guaranteed to be at least two
                // TODO THIS IS NOT CORRECT
                int ridxa = NEATNET.Random.Next(NextNeuronID);
                int ridxb = ridxa;
                while (ridxb == ridxa)
                    ridxb = NEATNET.Random.Next(NextNeuronID);

            }
        }

        public void AddIntermediateNode(LinkGene a)
        {
            a.Disabled = true;
            int nid = AddNode(NodeType.Intermediate);
            AddConnection(a.Source, nid, RandomWeight());
            AddConnection(nid, a.Destination, RandomWeight());
        }

        public int AddNode(NodeType type)
        {
            int nid = NextNeuronID++;
            NodeGenotype.Add(new NodeGene(nid, type));
            return nid;
        }

        public void AddConnection(int source, int destination, double weight)
        {
            LinkGenotype.Add(new LinkGene(source, destination, weight));
        }

        public double RandomWeight()
        {
            return NEATNET.Random.NextDouble() - 0.5;
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
            while (ac < a.LinkGenotype.Count && bc < b.LinkGenotype.Count)
            {
                if (a.LinkGenotype[ac].Innovation > b.LinkGenotype[bc].Innovation)
                {
                    bc++;
                    D++;
                }
                else if (a.LinkGenotype[ac].Innovation < b.LinkGenotype[bc].Innovation)
                {
                    ac++;
                    D++;
                }
                else
                {
                    // Add to mean weight differences between matching genes
                    WS += Math.Abs(a.LinkGenotype[ac].Weight - b.LinkGenotype[bc].Weight);
                    WC++;
                    ac++;
                    bc++;
                }
            }
            // Count number of excess genes
            E += a.LinkGenotype.Count - ac;
            E += b.LinkGenotype.Count - bc;

            int N = Math.Max(a.LinkGenotype.Count, b.LinkGenotype.Count);
            // Console.WriteLine($"D:{D} E:{E} W/:{WS/WC} N:{N}");
            return ExcessWeight * E / N + DisjointWeight * D / N + WeightDiffWeight * (WS / WC);
        }
    }
}

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
        // Fitness metric only for use by species
        public double Fitness { get; set; }

        public List<LinkGene> LinkGenotype { get; set; } = new List<LinkGene>();
        public List<NodeGene> NodeGenotype { get; set; } = new List<NodeGene>();

        public readonly int InputNodeCount;
        public readonly int OutputNodeCount;
        private int NextNeuronID = 0;

        /* CONSTANTS */
        // Weighting in compatibility metric for excess genes
        private const double ExcessWeight = 1.0;
        // Weighting in compatibility metric for disjoint genes
        private const double DisjointWeight = 1.0;
        // Weighting in compatibility metric for weighting differences between matching genes
        private const double WeightDiffWeight = 0.4;

        // Chance for a disabled gene to re-enable
        private const double GeneEnableChance = 0.2;
        // Chance for an enabled gene to disable
        private const double GeneDisableChance = 0.4;

        // Chance for a link's weight to mutate
        private const double WeightMutationChance = 0.8;
        // Chance for the weight to mutate by a uniform value if the mutation is already decided
        private const double WeightPerturbChance = 0.9;
        // Range of weight perturbance
        private const double WeightPerturbEpsilon = 0.1;

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

        public double[] EvaluateNeuralNetwork(double[] input)
        {
            if (input.Length != InputNodeCount)
                throw new ArgumentException("Input length does not match neural network inputs");

            memoizedOutput = new double[NextNeuronID];
            for (int i = 0; i < NextNeuronID; i++)
                memoizedOutput[i] = -1;
            double[] outputs = new double[OutputNodeCount];
            for (int i = 0; i < OutputNodeCount; i++)
            {
                int onid = InputNodeCount + i;
                outputs[i] = GetNeuronOutput(input, onid);
            }
            return outputs;
        }

        private double[] memoizedOutput;

        /*
         * Evaluate the outputs of the neural network, while memoizing existing values to avoid repeating calculations
         */
        public double GetNeuronOutput(double[] inputs, int id)
        {
            // This is only allowed because the inputs are guaranteed to come first
            if (id < InputNodeCount)
                return inputs[id];
            
            if (memoizedOutput[id] > 0)
                return memoizedOutput[id];

            if(NodeGenotype[id].Inputs.Count == 0)
                throw new ArgumentException("Encountered intermediate/output neuron with no inputs!");

            double csum = 0;
            foreach (LinkGene lg in NodeGenotype[id].Inputs)
            {
                double op = GetNeuronOutput(inputs, lg.Source);
                memoizedOutput[lg.Source] = op;
                csum += op * lg.Weight;
            }

            return Sigmoid(csum);
        }

        
        /*
         * Initiate structures for input and output nodes
         */
        public void InitializeUtilityNodes()
        {
            for (int i = 0; i < InputNodeCount; i++)
                AddNode(NodeType.Input);
            for (int i = 0; i < OutputNodeCount; i++)
                AddNode(NodeType.Output);
        }

        public void Mutate()
        {
            // Add Node Mutation
            if (NEATNET.Random.NextDouble() < AddNodeMutationChance)
            {
                // There's a possibility that there aren't any links at all
                if (LinkGenotype.Count > 0)
                {
                    // Pick random link
                    int ridx = NEATNET.Random.Next(LinkGenotype.Count);
                    LinkGene a = LinkGenotype[ridx];
                    // Add an intermediate node in the link
                    AddIntermediateNode(a);
                }
            }

            // Add Link Mutation
            if (NEATNET.Random.NextDouble() < AddLinkMutationChance)
            {
                // Pick two unique neurons, there are guaranteed to be at least two
                // Starting neuron cannot be an output neuron, ending neuron cannot be an input neuron
                int ridxa;
                do
                {
                    ridxa = NEATNET.Random.Next(NextNeuronID);
                } while (NodeGenotype[ridxa].Type != NodeType.Output);
                int ridxb = ridxa;
                while (ridxb == ridxa || NodeGenotype[ridxb].Type == NodeType.Input)
                    ridxb = NEATNET.Random.Next(NextNeuronID);
                AddConnection(ridxa, ridxb, RandomWeight());
            }

            for (int i = 0; i < LinkGenotype.Count; i++)
            {
                var lg = LinkGenotype[i];
                if (NEATNET.Random.NextDouble() < WeightMutationChance)
                {
                    if (NEATNET.Random.NextDouble() < WeightPerturbChance)
                    {
                        // Perturb weighting by a uniform amount
                        lg.Weight += WeightPerturbEpsilon * RandomWeight();
                    }
                    else
                    {
                        // Randomize weighting
                        lg.Weight = RandomWeight();
                    }
                }
            }
            // TODO Implement other mutation types
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
            var lg = new LinkGene(source, destination, weight);
            LinkGenotype.Add(lg);
            NodeGenotype[source].Outputs.Add(lg);
            NodeGenotype[destination].Inputs.Add(lg);
        }

        private void AddConnection(int source, int destination, double weight, int innovation)
        {
            var lg = new LinkGene(source, destination, weight)
            {
                Innovation = innovation
            };
            LinkGenotype.Add(lg);
            NodeGenotype[source].Outputs.Add(lg);
            NodeGenotype[destination].Inputs.Add(lg);
        }

        /*
         * Gets a random double between 1 and -1
         */
        public double RandomWeight()
        {
            return NEATNET.Random.NextDouble() * 2 - 1;
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

        /*
         * Crosses two genomes of the same species together.
         * Fitness MUST be evaluated first.
         */
        public static Genome Crossover(Genome a, Genome b)
        {
            if (a.InputNodeCount != b.InputNodeCount || a.OutputNodeCount != b.OutputNodeCount)
                throw new ArgumentException("Genomes are fundamentally incompatible");

            var newGenome = new Genome(a.InputNodeCount, a.OutputNodeCount);
            int newNodeCount = Math.Max(a.NextNeuronID, b.NextNeuronID) - a.InputNodeCount - a.OutputNodeCount;
            for (int i = 0; i < newNodeCount; i++)
                newGenome.AddNode(NodeType.Intermediate);

            int ac = 0, bc = 0;
            while (ac < a.LinkGenotype.Count && bc < b.LinkGenotype.Count)
            {
                if (a.LinkGenotype[ac].Innovation > b.LinkGenotype[bc].Innovation)
                {
                    var ca = a.LinkGenotype[ac];
                    newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation);;
                    bc++;
                }
                else if (a.LinkGenotype[ac].Innovation < b.LinkGenotype[bc].Innovation)
                {
                    var ca = b.LinkGenotype[bc];
                    newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                    ac++;
                }
                else
                {
                    // Add to mean weight differences between matching genes
                    if (a.Fitness > b.Fitness)
                    {
                        var ca = a.LinkGenotype[ac];
                        newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                    }
                    else if (b.Fitness > a.Fitness)
                    {
                        var ca = b.LinkGenotype[bc];
                        newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                    }
                    else
                    {
                        if (NEATNET.Random.NextDouble() < 0.5)
                        {
                            var ca = a.LinkGenotype[ac];
                            newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                        }
                        else
                        {
                            var ca = b.LinkGenotype[bc];
                            newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                        }
                    }
                    ac++;
                    bc++;
                }
            }
            while (ac < a.LinkGenotype.Count)
            {
                var ca = a.LinkGenotype[ac];
                newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                ac++;
            }
            while (bc < b.LinkGenotype.Count)
            {
                var ca = b.LinkGenotype[bc];
                newGenome.AddConnection(ca.Source, ca.Destination, ca.Weight, ca.Innovation); ;
                bc++;
            }
            return newGenome;
        }

        public double Sigmoid(double d)
        {
            return 1.0 / (1 + Math.Exp(-4.9 * d));
        }
    }
}

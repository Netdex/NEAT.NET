using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    public enum NodeType
    {
        Input,
        Output,
        Intermediate
    }

    public struct NodeGene
    {
        public int ID { get; set; }
        public NodeType Type { get; set; }
        public List<LinkGene> Inputs { get; set; }
        public List<LinkGene> Outputs { get;set; }

        public NodeGene(int id, NodeType type)
        {
            ID = id;
            Type = type;
            Inputs = new List<LinkGene>();
            Outputs = new List<LinkGene>();
        }
    }
}

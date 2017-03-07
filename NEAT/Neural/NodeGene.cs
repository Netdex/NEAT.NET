using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT.Neural
{
    enum NodeType
    {
        Input,
        Output,
        Intermediate
    }

    struct NodeGene
    {
        public int ID { get; set; }
        public NodeType Type { get; set; }

        public NodeGene(int id, NodeType type)
        {
            ID = id;
            Type = type;
        }
    }
}

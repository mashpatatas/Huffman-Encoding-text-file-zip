using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_Week4_Doak
{
    public class Node
    {
        public char c = (char)0;
        public int freq;
        public Node left;
        public Node right;
        public Node root;
        public string encoding;

        public Node()
        {

        }

        public Node(char c)
        {
            this.c = c;
        }

        public Node(char c, int f)
        {
            this.c = c;
            freq = f;
        }

        public bool IsLeaf()
        {
            if (this.left == null && this.right == null)
                return true;
            else return false;
        }
    }
}

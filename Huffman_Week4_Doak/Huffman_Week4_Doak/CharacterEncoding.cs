using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huffman_Week4_Doak
{
    
    public class CharacterEncoding
    {
        private StringBuilder encoding;
        private Node[] eTable;
        public int size;

        public CharacterEncoding()
        {
            encoding = new StringBuilder();
            BuildTableStructure();
            size = eTable.Length;
        }
        public Node[] BuildEncodingTable(Node p)
        {
            if (p != null)
            {
                encoding.Append("0");
                BuildEncodingTable(p.left);

                if (p.IsLeaf() && (int)p.c <= 256)
                    eTable[(int)p.c].encoding = encoding.ToString();

                encoding.Append("1");
                BuildEncodingTable(p.right);

                if (encoding.Length > 0)
                    encoding.Remove(encoding.Length - 1, 1);
            }
            else
            {
                encoding.Remove(encoding.Length - 1, 1);
            }
            return eTable;
        }

        public void BuildTableStructure()
        {
            // one entry for each char value
            eTable = new Node[256];
            for (int i = 0; i < 256; i++)
            {
                eTable[i] = new Node((char)i);
            }
        }
    }
}

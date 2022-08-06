using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Linq;

namespace Huffman_Week4_Doak
{
    class Program
    {
        static void Main(string[] args)
        {
            // ================================================================================
            //                  HEADING
            //      -- file input
            //      -- instantiate prio. queue
            // ================================================================================
            PriorityQueue<Node> Q = new PriorityQueue<Node>();
            CharacterEncoding ce = new CharacterEncoding();

            string filepath;

            // ONLY USE ASCII TEXT FILES.
            // WARPEACE.TXT  WORKS.
            // TRY HUFFMANTEST.TXT FOR THE EXAMPLE FROM THE BOOK.
            // UTOPIA.TXT THROWS ERRORS SINCE IT'S IN UTF-8
            filepath = "warpeace.txt";
            FileInfo infile = new FileInfo(filepath);

            // This is a running count of how many characters are in the input file.
            int charCount = 0;

            // ================================================================================
            //                  CHARACTER COUNTING
            //      -- read file
            //      -- count frequency of each character
            //      -- build prio. queue
            // ================================================================================
            if (infile.Exists)
            {
                Console.WriteLine("Counting characters...");

                // A list of all the frequencies associated with each character found in the 
                // input file.
                LinkedList<Node> freqList = new LinkedList<Node>();
                StreamReader sr = infile.OpenText();

                int temp;

                while(!sr.EndOfStream)
                {
                    charCount++;
                    temp = sr.Read();
                    bool found = false;

                    // our finder node is the node we want to seek in our list
                    // if we find it, we increment that character's frequency
                    Node finder = new Node((char)temp, 0);
                    if (finder != null)
                    {
                        foreach(Node item in freqList)
                        {
                            if (item.c == finder.c)
                            {
                                item.freq++;
                                found = true;
                            }
                        }
                        // if we don't find it, then we add a new node for that character
                        if(!found)
                            freqList.AddLast(new Node((char)temp, 1));
                    }
                }
                sr.Close();

                // Add all the characters found + their frequencies to the priority queue.
                foreach(Node item in freqList)
                {
                    Q.Enqueue(new Node(item.c,item.freq),item.freq);
                }
            }
            else
            {
                Console.WriteLine("Input file doesn't exist.");
                return;
            }

            // =========================================================
            //                  PROCESS PRIO. QUEUE 
            //      -- create binary tree
            //      -- print out characters as they dequeue
            //      -- build encoding table
            // =========================================================

            int n = Q.Size();

            Console.WriteLine("Processing priority queue...");

            // Take the first two items off the front of the priority queue.
            // Make a new node which has the sum of the frequencies of the two items.
            // Add the new node back to the priority queue.
            for(int i = 0; i < n-1; i++)
            {
                Node z = new Node();
                z.left = Q.Dequeue();
                Console.WriteLine("DQ: " + z.left.c + " " + z.left.freq);
                z.right = Q.Dequeue();
                Console.WriteLine("DQ: " + z.right.c + " " + z.right.freq);
                z.freq = z.left.freq + z.right.freq;
                Q.Enqueue(z, z.freq);
            }
            
            // There should only be 1 item in the priority queue at the end, which is 
            // the root node of the binary tree.
            Node root = Q.Dequeue();

            if(root != null)
                Console.WriteLine("DQ: " + root.c + " " + root.freq);

            // Build an encoding table.
            // This is a 256-node array, one for each
            // character in ASCII. For each character,
            // the encoding table links the character
            // with its encoding. 
            Node[] eTable = ce.BuildEncodingTable(root);

            Console.WriteLine("\nCharacter codes: ");

            // Print out each character and its encoding to the console.
            for (int i = 0; i < 256; i++)
            {
                if(eTable[i].encoding != null)
                    Console.WriteLine(eTable[i].c + " " + eTable[i].encoding);
            }

            // =========================================================
            //                  COMPRESSION
            // =========================================================
            BinaryWriter bWriter = new BinaryWriter(File.Create("compressed.cmp"));
            StreamReader cmpReader = infile.OpenText();

            int charToCompress;
            int bitIndex = 8;
            byte baseline = 0;
            Console.WriteLine("\nCompressing " + infile.Name + "...\n");

            //
            // The idea here is to modify a baseline byte (0000 0000). 
            // Just flip the bits to 1 as needed. 
            //

            while (!cmpReader.EndOfStream)
            {
                charToCompress = cmpReader.Read();

                int encLength = eTable[charToCompress].encoding.Length;

                //
                // If the encoding string is n characters long...
                // e.g. in 0001 0100 the length is 8...
                // For every one of those characters which is a 1...
                // Flip the bit at the appropriate place in baseline byte. 
                //
                // Once 8 bits have been encoded, write the now completed
                // byte to file. 
                //
                for (int i = 0; i < encLength; i++)
                {
                    bitIndex--;
                    if (eTable[charToCompress].encoding.ElementAt(i) == '1')
                    {
                        baseline = (byte)(baseline | (int)(Math.Pow(2, bitIndex)));
                    }
                    if (bitIndex == 0)
                    {
                        //string test = Convert.ToString(baseline, 2).PadLeft(8, '0');
                        //Console.WriteLine(test);
                        // this writes a representation of the completed bytes to console. 
                        // warning: it's a lot of stuff.

                        bWriter.Write(baseline);
                        bitIndex = 8;
                        baseline = 0;
                    }
                }
            }
            //
            // 'leftovers' is the trailing 
            // garbage bits (zeroes) in the
            // last byte of encoding
            //
            // there's a chance things
            // will work out to a multiple of 8
            // but if they don't, the last byte
            // won't get written in the above
            // algorithm. (bitIndex doesn't get to 0)
            //

            int leftovers = 8 - bitIndex;
            if (leftovers != 0)
                bWriter.Write(baseline);


            cmpReader.Close();
            bWriter.Close();

            // =========================================================
            //                 DECOMPRESSION
            // =========================================================
            string cmpfile = "compressed.cmp";
            Console.WriteLine("Decompressing " + cmpfile + "...\n");

            BinaryReader bReader = new BinaryReader(File.OpenRead(cmpfile));
            StreamWriter sWriter = new StreamWriter(File.Create("decompressed.txt"));
            byte b;
            string enc = "";

            while (bReader.BaseStream.Position != bReader.BaseStream.Length)
            {
                b = bReader.ReadByte();

                byte[] bFromFile = new byte[1] { b };
                //
                // Each bit is stored in BitArray as a bool value.
                //
                BitArray ba = new BitArray(bFromFile);

                //
                // If we're at the last byte, it has a good chance to contain
                // garbage bits. The amount of garbage bits we already know
                // is the same as leftovers. Reading the byte from
                // left to right, we need to read up until the 
                // leftover bits. 
                //
                int bitToCountTo = 0;
                if (bReader.BaseStream.Length - bReader.BaseStream.Position == 0)
                    bitToCountTo = (8 - leftovers);

                for (int baIndex = 7; baIndex > bitToCountTo - 1; baIndex--)
                {
                    // if the bit at this location is ON, i.e. it's a 1
                    if (ba[baIndex] == true)
                        enc += '1';
                    // else the bit must be off i.e. 0. 
                    else
                        enc += '0';

                    foreach (Node item in eTable)
                    {
                        if (enc.Equals(item.encoding))
                        {
                            sWriter.Write(item.c);
                            enc = "";
                            //
                            // charCount is an int that counts the number of characters in the input file
                            // getting this back to zero means there's the same number of characters in the decompressed file
                            //
                            charCount--;
                        }
                    }
                }
            }

            bReader.Close();
            sWriter.Close();

            if (charCount == 0)
                Console.WriteLine("Done.");
            else
                Console.WriteLine("Character counting error; decompressed file may be inaccurate.");
        }

    }
}

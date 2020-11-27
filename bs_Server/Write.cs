using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.Emit;
using System.Text;

namespace bs_Server
{
    class Write
    {
        string packets = "";
        string ret;

        public Write(int opcode_)
        {
            //packets = opcode_ + "|";
        }

        public void writeLine(string line)
        {
            packets += line + "|";
        }

        public string getPacket(bool done)
        {
            //string length = string.Format("{0:D4}", packets.Length);
            //ret += length + "|";
            ret += packets;
            if (done)
            {
                ret += "><";
            }
            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BerldChess.Source.Model
{
    public static class PGNExporter
    {
        public static string MovesToString(string[] moves)
        {

            string pgnString = "";
            for (int i = 1; i <= moves.Length; i++)
            {
                if (i % 2 == 1)
                {
                    pgnString += " " + ((i/2)+1) + ".";
                }
                pgnString += " " + moves[i - 1];
            }

                return pgnString;
        }
    }
}

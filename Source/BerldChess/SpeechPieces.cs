using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerldChess
{
    public class SpeechPieces
    {
        public Dictionary<String, String> pieces = new Dictionary<string, string>();       
        public SpeechPieces(String culture = "en-US")
        {
            if (culture == "en-US")
            {
                pieces = new Dictionary<String, String>() {
                   {"p","pawn"},
                    {"r","rook" },
                    {"b","bishop" },
                    {"n","knight" },
                    {"q","queen" },
                    {"k","king" }
                };
            }
        }
    }
}

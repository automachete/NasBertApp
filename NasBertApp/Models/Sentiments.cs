using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NasBertApp.Models
{
    internal class Sentiments
    {
        static public readonly Dictionary<float, string> SentimentsDict = new Dictionary<float, string>(){
            {0, "Negative 😩" },
            {1, "Positive 🤗"},
        };
    }
}

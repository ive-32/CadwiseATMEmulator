using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadwiseATMEmulator
{
    public static class DictionaryExtention
    {
        public static string GetDescription<T1, T2>(this Dictionary<T1, T2> dict)
        {
            return string.Join(", ",dict.Select(d => $"[{d.Key} - {d.Value}]"));
        }
    }
}

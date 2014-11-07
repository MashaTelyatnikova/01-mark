using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class Aggregator<T> : ICloneable, IComparable
    {
        private List<T> elements;
        private T result;

        public Aggregator(T val)
        {
            elements.Add(val);
            result = elements.Sum(i => i);
        } 
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}

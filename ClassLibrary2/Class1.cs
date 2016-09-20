using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary2
{
    public class Class1
    {
        public void Call(string name)
        {
            Console.WriteLine($"{name}:lib2:{ss} ");
        }

        public static string ss = "12346";
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        public void Run(string name)
        {
            Console.WriteLine($"{name}:lib1: OK8eeeeddd!");
            new ClassLibrary2.Class1().Call(name);
           System.Threading.Thread.Sleep(1000);
        }
    }
}

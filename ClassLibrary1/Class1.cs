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
        public void Run()
        {
            Console.WriteLine("OK8eeeeddd!");
            new ClassLibrary2.Class1().Call();
           System.Threading.Thread.Sleep(3);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;

namespace UnloadDll
{
    class Program
    {
        public static List<Assembly> AssemblyList = new List<Assembly>();
        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (var item in AssemblyList)
            {
                if (item.ToString() == args.Name)
                {
                    return item;
                }
            }
            return null;
        }

        static void Main(string[] args)
        {
            string callingDomainName = AppDomain.CurrentDomain.FriendlyName; //Thread.GetDomain().FriendlyName;             
            Console.WriteLine(callingDomainName);
            Task.Run(() =>
            {
                while (true)
                {
                    AppDomain ad = AppDomain.CreateDomain("DLL Unload test1");
                    ad.AssemblyResolve += CurrentDomain_AssemblyResolve;
                    var instance = ad.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, "UnloadDll.ProxyObject");
                    ProxyObject obj = (ProxyObject)instance;
                    obj.LoadAssembly();
                    obj.Invoke("ClassLibrary1.Class1", "Run", "Thread1");
                    AppDomain.Unload(ad);
                }
            });
            Task.Run(() =>
            {
                AppDomain ad = AppDomain.CreateDomain("DLL Unload test2");
                ad.AssemblyResolve += CurrentDomain_AssemblyResolve;
                var instance = ad.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, "UnloadDll.ProxyObject");
                ProxyObject obj = (ProxyObject)instance;
                obj.LoadAssembly();
                while (true)
                {
                    obj.Invoke("ClassLibrary1.Class1", "Run", "Thread2");
                }
                AppDomain.Unload(ad);
            }).Wait();
        }
    }

    class ProxyObject : MarshalByRefObject
    {
        private Assembly assembly = null;

        public void LoadAssembly()
        {
            string dir = @"D:\github\UnloadDll\ClassLibrary1\bin\Debug";
            string fileName = Path.Combine(dir, "ClassLibrary1.dll");
            string fileName2 = Path.Combine(dir, "ClassLibrary2.dll");
            if (!File.Exists(fileName))
            {
                Console.WriteLine("dll file not exists");
                System.Threading.Thread.Sleep(1000);
                return;
            }
            try
            {
                var filesByte = File.ReadAllBytes(fileName);
                var assembly1 = Assembly.Load(filesByte);
                var filesByte2 = File.ReadAllBytes(fileName2);
                var assembly2 = Assembly.Load(filesByte2);
                Program.AssemblyList.Add(assembly1);
                Program.AssemblyList.Add(assembly2);
                assembly = assembly1;
            }
            catch (Exception)
            {
                Console.WriteLine("read file error");
                System.Threading.Thread.Sleep(1);
            }
        }

        public bool Invoke(string fullClassName, string methodName, params Object[] args)
        {
            if (assembly == null) return false;
            Type tp = assembly.GetType(fullClassName);
            if (tp == null) return false;
            MethodInfo method = tp.GetMethod(methodName);
            if (method == null) return false;
            Object obj = Activator.CreateInstance(tp);
            method.Invoke(obj, args);
            return true;
        }
    }
}
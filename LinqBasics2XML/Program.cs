using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data.Linq;
using System.Data.Linq.Mapping;

using System.Xml.Linq;

/* To use in mono 3.0.x+ copy
   /c/Windows/winsxs/x86_netfx35linq-system.data.entity_31bf3856ad364e35_6.1.7601.17514_none_351ba0ba39d5bcbe/System.Data.Entity.dll
 in the bin/ directory

*/
namespace LinqBasics2XML
{

    class Program
    {

        delegate int Transformer(int i);
        static void Main(string[] args)
        {
            // Lambda Example
            Transformer x2 = param => param * 2;
            say("4x2=" + x2(4));
            XElement x=
                XElement.Load("test1.xml");
            say("Test Loaded");
            IEnumerable<string> q=null;
            //q = x.DescendantNodes().e;
            var firstVariant = x.Descendants("customer").Descendants("name").AsEnumerable();

            IEnumerable<XElement> secondVariant = 
                x.Descendants("customer").Descendants("name").AsEnumerable();
            say("Customer names", secondVariant);

            say("Attribute access:",
                x.Descendants("customer").Where(v => v.Attribute("selectMe").Value == "true"));

            List<XElement> xlist=x.Descendants("customer").ToList<XElement>();
            say("List to convert", xlist);
            var procName=Process.GetCurrentProcess().ProcessName;
            var p = new PerformanceCounter("Process","Private Bytes",procName);
            say(procName+" Private Bytes::"+p.NextValue());


            /*
            say("Entity Framework Access: better many2many access. Namespaces cannot be mixed");
            studioEntities s = new studioEntities();
            say("Many 2 Many",
                     from cs in s.SoftwareCustomer
                     from csp in cs.SoftwareProduct
                     select cs.description + " buyed " + csp.description
             );
            */
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void say(string msg, IEnumerable<string> e =null){
            say<string>(msg, e);
        }
        private static void say<T>(string msg,IEnumerable<T> e=null)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("== " + msg);
            if (e != null)
            {
                foreach (T n in e)
                {
                    Console.WriteLine(n+" "+n.GetType());
                }
            }
        }
    }
}

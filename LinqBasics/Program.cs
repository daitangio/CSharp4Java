using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb; 

namespace LinqBasics
{
    [Table]
    public class Customers
    {
        [Column(IsPrimaryKey=true)] public string Customer_ID;
        [Column]                    public string City;
    }
    class Program
    {
        static void Main(string[] args)
        {
            //plainCVSAccessExample();
            plainExcelAccessExample();
            plainDBAccess();
            string[] names = { "Vanessa", "Mattia", "Giovanni", "Bill", "Steve" };
            IEnumerable<string> e = names.Where(n => n.Length <= 5);
            say(e);

            // Query comprehension
            say(
                from n in names where n.Contains("V") select n 
            );

            // Simple ordering with fluent syntax
            say(
                names .OrderBy( n => n) .Select(n => n) 
            );

            // Deferred Execution Example
            var l = new List<int>();

            int capturedSideEffectExample = 2;
            IEnumerable<int> q = l.Select(n => n * capturedSideEffectExample);
            // Play a bit:
            l.Add(1); l.Add(2);
            say(q);
            l.Clear();
            l.Add(3); l.Add(4);
            say(q);

            // But the variables are also CAPTURED in the query, so they are subjected to side effects:
            capturedSideEffectExample = 4;
            say(q);


            //Interpreted Queries
            DataContext dataContext = new DataContext("Data Source=C:\\Programmi\\Microsoft SQL Server Compact Edition\\v3.5\\Samples\\Northwind.sdf;Persist Security Info=False;");

            // Debuging http://msdn.microsoft.com/en-us/library/bb386961.aspx
            dataContext.Log = Console.Out;

            Table<Customers> customers = dataContext.GetTable<Customers>();
            say("Customers DB Connection OK");
            // NB subclass of IEnumerable. Query syntax
            // The Distinct operator is used. See http://blogs.msdn.com/b/charlie/archive/2006/11/19/linq-farm-group-and-distinct.aspx
            IQueryable<string> cities = (from c in customers orderby c.City select c.City).Distinct();
            // The Result query is well written
            say("Cities", cities);

            // Linq-2-sql is simpler, but .net 4 focus on Entit Framework.
            // Linq-2-sql autogenerator engine do not work on sdf compact sql file database.
            // Reference http://www.albahari.com/nutshell/10linqmyths.aspx

            dataContext = new StudioLinq2SQLDataContext();
            dataContext.Log = Console.Out;
            Table<Product> products=dataContext.GetTable<Product>();

            say("Products by LINQ__",
                from p in products select p.ProductName);
            Table<Order> orders = dataContext.GetTable<Order>();
            say("Details...",
                from o in orders
                from dt in o.OrderDetails
                select new {
                    // We can create on the fly a bean object with such properties:
                    // Static Typing will make us happy with the last say variant
                    Order="Order "+o.OrderID, 
                    PDesc="N# "+ dt.Quantity+ " Product:"+ dt.Product.ProductName+" Cat:"+dt.Product.Category.CategoryName
                } );

            say("Many2Many simple relation",
                from buys in dataContext.GetTable<Customer2Software>()
                select new
                {
                    desc=buys.Customer+" "+ buys.Software
                });
            /*
            say("Many2Many simple relation",
                from cx in dataContext.GetTable<SoftwareCustomer>()
                from rel in dataContext.GetTable<Customer2Software>()
                select new
                {
                    desc = buys.Customer + " " + buys.Software
                });
            */

            Console.ReadLine();
        }

        private static void plainDBAccess()
        {
            string sqlConnectString ; 
            sqlConnectString="Server=GIOVANNI\\SQLEXPRESS;Database=studio;Trusted_Connection=True;";
            string sqlSelect = 
                "SELECT ProductID, ProductName FROM Products "; 
            SqlConnection connection = new SqlConnection(sqlConnectString); 
            // Create the command and open the connection 
            SqlCommand command = new SqlCommand(sqlSelect, connection); 
            connection.Open(); 
            // Create the DataReader to retrieve data 
            using (SqlDataReader dr = command.ExecuteReader()) 
            { 
                while (dr.Read()) 
                {
                    // Output fields from DataReader row 
                    Console.WriteLine(
                        "ProductID = {0}\tProductID = {1}",
                        dr["ProductID"], dr["ProductName"]);
                }
            }
            connection.Close();
            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey(); 


        }

        private static void plainExcelAccessExample()
        {
            string oledbConnectString =
                    "Provider=Microsoft.ACE.OLEDB.12.0;" +
                    @"Data Source=..\..\database\Category.xlsx;" +
                    "Extended Properties=\"Excel 12.0;HDR=YES\";";
            string commandText = "SELECT CategoryID, CategoryName, " +
                "Description FROM [Sheet1$]";
            Console.WriteLine("---CONNECTION---");
            Console.WriteLine(oledbConnectString);
            OleDbConnection connection =
                new OleDbConnection(oledbConnectString);
            OleDbCommand command =
                new OleDbCommand(commandText, connection);
            connection.Open();
            OleDbDataReader dr = command.ExecuteReader();
            Console.WriteLine("\nID Name           Description");
            while (dr.Read())
            {
                Console.WriteLine("{0}  {1} {2}", dr["CategoryID"],
                    dr["CategoryName"].ToString().PadRight(14),
                    dr["Description"]);
            }
            connection.Close();
            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey(); 
        }

        private static void plainCVSAccessExample()
        {
            string sqlSelect = "SELECT * FROM [catalogCSV.txt]";
            string connectString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                @"Data Source=.\;" +
                "Extended Properties=\"text;HDR=yes;FMT=Delimited(,)\";";
            // Create and fill a DataTable. 
            OleDbDataAdapter da =
                new OleDbDataAdapter(sqlSelect, connectString);

            DataTable dt = new DataTable("catalogCSV");
            da.Fill(dt);
            // dt.Columns;
            Console.WriteLine("CategoryID; CategoryName; Description\n");
            //Console.WriteLine("" + dt.ToString());
            //dt.WriteXml(Console.Out);
            foreach (DataRow row in dt.Rows)
            {
                // Non va
                //Console.WriteLine("{0}; {1}; {2}", row["CategoryID"],
                //    row["CategoryName"], row["Description"]);
                Console.WriteLine("{0}      {1}     {2}", row[0],
                    row[1], row[2]);
            } 
        }

        private static void say(string s)
        {
            Console.WriteLine(s);
        }

        private static void say(string s, IEnumerable<string> e)
        {
            say(s);
            say(e);

        }

        private static void say(IEnumerable<string> e)
        {
            Console.WriteLine("=======================================");
            foreach (string n in e)
            {
                Console.WriteLine(n);
            }
        }

        private static void say(IEnumerable<int> e)
        {
            Console.WriteLine("=======================================");
            foreach(object n in e)
            {
                Console.WriteLine(""+n);
            }
        }

        private static void say<T>(string msg, IEnumerable<T> e = null)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("== " + msg);
            if (e != null)
            {
                foreach (T n in e)
                {
                    Console.WriteLine(n + " " + n.GetType());
                }
            }
        }
    }
}

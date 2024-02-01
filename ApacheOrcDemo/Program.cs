using ApacheOrcDotNet;
using ApacheOrcDotNet.ColumnTypes;
using ApacheOrcDotNet.FluentSerialization;
using ApacheOrcDotNet.Protocol;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace ApacheOrcDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter operation( 'read' for reading/displaying orc data  , 'write' for writing db data to orc file) : ");
            string val = Console.ReadLine();

            switch (val)
            {
                case "read":
                    var OrcFileReader = new OrcFileReader("wendy.orc");
                    OrcFileReader.Read();
                    break;
                case "write":
                    var orcFileWriter = new FinalOrcFileWriter("wendy.orc");
                    orcFileWriter.Write();
                    break;
            }

            Console.Write("operation finished , press any button to leave the console : ");

            Console.ReadLine();
        }








        //static void Main(string[] args)
        //{

        //    var fields = new List<Field>()
        //    {
        //        new Field("EmployeeID","int"),
        //        new Field("EmployeeName","String"),
        //        new Field("Designation","String")
        //    };

        //    var employeeClass = CreateClass(fields, "Employee");

        //    dynamic employee1 = Activator.CreateInstance(employeeClass);
        //    employee1.EmployeeID = 4213;
        //    employee1.EmployeeName = "Wendy Tailor";
        //    employee1.Designation = "Engineering Manager";

        //    dynamic employee2 = Activator.CreateInstance(employeeClass);
        //    employee2.EmployeeID = 3510;
        //    employee2.EmployeeName = "John Gibson";
        //    employee2.Designation = "Software Engineer";

        //    Type newType = employee2.GetType();


        //    var listType = typeof(List<>).MakeGenericType(employee2.GetType());

        //    var list = (IList<object>)Activator.CreateInstance(listType);
        //    list.Add(employee1);
        //    list.Add(employee2);
        //    Console.WriteLine($"{employee1.EmployeeName}");


        //    string workingDirectory = Environment.CurrentDirectory;
        //    string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
        //    string _filePath = Path.Combine(projectDirectory, "hola.orc");


        //    var serializationConfiguration = new SerializationConfiguration();

        //    serializationConfiguration.ConfigureType<object>().Build();
        //    //Type d1 = typeof(OrcWriter);
        //    //Type constructed = d1.MakeGenericType(newType);



        //    using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
        //    using (var writer = new OrcWriter<object>(fileStream, new WriterConfiguration(), serializationConfiguration)) //Use the default configuration
        //    {
        //        writer.AddRows(list);
        //    }

            //Console.WriteLine($"{employee2.EmployeeName}");

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();
        //}




    }




}

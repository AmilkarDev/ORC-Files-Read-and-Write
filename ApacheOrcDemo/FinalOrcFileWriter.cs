using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApacheOrcDemo
{
    public class FinalOrcFileWriter
    {
        private string _filePath;
        public FinalOrcFileWriter(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            _filePath = Path.Combine(projectDirectory, fileName);
        }
        public void Write()
        {           
            SqlConnection cn = new SqlConnection(@"Server=TA9TOU9\AMILKAR;Database=StudentDB;Trusted_Connection=True;TrustServerCertificate=True");
            cn.Open();

            SqlDataAdapter da = new SqlDataAdapter("select * from Students", cn);

            DataTable testdata = new DataTable();

            da.Fill(testdata);

            var propertiesDict = new Dictionary<string, string>();
            foreach (DataColumn column in testdata.Columns)
            {
                propertiesDict.Add(column.ColumnName, column.DataType.ToString());
            }

            var fields = new List<Field>();
            foreach(var elem in propertiesDict)
            {
                fields.Add(new Field(elem.Key, elem.Value));
            }

            var onFlyClass = RoslynLogic.CreateClass(fields, "Employee");
            var listType = typeof(List<>).MakeGenericType(onFlyClass);
            var list = Activator.CreateInstance(listType);

            foreach (DataRow row in testdata.Rows)
            {
                var newObj = Activator.CreateInstance(onFlyClass);
                foreach (var el in propertiesDict)
                {
                    var columnName = el.Key;
                    if (columnName == "startDate")
                    {
                        var val = (DateTime)row[columnName];

                        onFlyClass.GetProperty(columnName).SetValue(newObj, DateTime.SpecifyKind(val, DateTimeKind.Utc), null);
                    }
                    else
                    {
                        onFlyClass.GetProperty(columnName).SetValue(newObj, row[columnName], null);

                    }
                }
                list.GetType().GetMethod("Add").Invoke(list, new[] { newObj });

            }
            cn.Close();

            var myGenericOrcWriter = Activator.CreateInstance(typeof(GenericOrcWriter<>).MakeGenericType(onFlyClass), _filePath);

            Type type = myGenericOrcWriter.GetType();
            MethodInfo method = type.GetMethod("Write");

            object[] parametersArray = new object[] { list };

            method.Invoke(myGenericOrcWriter, parametersArray);
        }

    }
}

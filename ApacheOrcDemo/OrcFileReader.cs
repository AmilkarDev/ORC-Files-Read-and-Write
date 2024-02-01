using ApacheOrcDotNet.ColumnTypes;
using ApacheOrcDotNet.Protocol;
using Microsoft.Data.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacheOrcDemo
{
    public class OrcFileReader
    {
        private string _filePath;
        public OrcFileReader(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            _filePath = Path.Combine(projectDirectory, fileName);
        }
        public void Read()
        {
            var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            var fileTail = new ApacheOrcDotNet.FileTail(stream);
            var df = new DataFrame();

            foreach (var stripe in fileTail.Stripes)
            {
                Console.WriteLine($"Reading stripe with {stripe.NumRows} rows");
                var stripeStreamCollection = stripe.GetStripeStreamCollection();

                if (fileTail.Footer.Types[0].Kind != ColumnTypeKind.Struct)
                    throw new InvalidDataException($"The base type must be {nameof(ColumnTypeKind.Struct)}");
                var names = fileTail.Footer.Types[0].FieldNames;

                for (int columnId = 1; columnId < fileTail.Footer.Types.Count; columnId++)
                {
                    var columnType = fileTail.Footer.Types[columnId];
                    var columnName = names[columnId - 1];

                    switch (columnType.Kind)
                    {
                        case ColumnTypeKind.Long:
                        case ColumnTypeKind.Int:
                        case ColumnTypeKind.Short:
                            {
                                var reader = new LongReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<long> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Byte:
                            {
                                var reader = new ByteReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<byte> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Boolean:
                            {
                                var reader = new BooleanReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<bool> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Float:
                            {
                                var reader = new FloatReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<float> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Double:
                            {
                                var reader = new DoubleReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<double> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Decimal:
                            {
                                var reader = new DecimalReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<decimal> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Timestamp:
                            {
                                var reader = new TimestampReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<DateTime> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.Date:
                            {
                                var reader = new DateReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                PrimitiveDataFrameColumn<DateTime> dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        case ColumnTypeKind.String:
                            {
                                var reader = new ApacheOrcDotNet.ColumnTypes.StringReader(stripeStreamCollection, (uint)columnId);
                                var data = reader.Read().ToList();
                                StringDataFrameColumn dfCol = new(columnName, data);
                                df.Columns.Add(dfCol);

                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }

                df.PrettyPrint();
                Console.WriteLine("Done reading stripe");
            }



        }
    }
}

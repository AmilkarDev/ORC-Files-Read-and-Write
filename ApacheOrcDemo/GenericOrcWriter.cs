using ApacheOrcDotNet;
using ApacheOrcDotNet.FluentSerialization;

namespace ApacheOrcDemo
{
    public class GenericOrcWriter<T> where T : class
    {
        private string _filePath;
        public GenericOrcWriter(string fileName)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            _filePath = Path.Combine(projectDirectory, fileName);
        }
        public void Write(IEnumerable<T> list)
        {
            string filePath = _filePath;

            var serializationConfiguration = new SerializationConfiguration()
                                                   .ConfigureType<T>().Build();

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new OrcWriter<T>(fileStream, new WriterConfiguration(), serializationConfiguration)) //Use the default configuration
            {
                writer.AddRows(list);
            }

            }
    }
}

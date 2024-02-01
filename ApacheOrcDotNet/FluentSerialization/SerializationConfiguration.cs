using System;
using System.Collections.Generic;
using System.Text;

namespace ApacheOrcDotNet.FluentSerialization
{
    public class SerializationConfiguration
    {
		readonly Dictionary<Type, ISerializationTypeConfiguration> _types = new Dictionary<Type, ISerializationTypeConfiguration>();

		public IReadOnlyDictionary<Type, ISerializationTypeConfiguration> Types { get => _types; }

		public SerializationTypeConfiguration<T> ConfigureType<T>()
		{
			if(!_types.TryGetValue(typeof(T), out var typeConfiguration))
			{
				typeConfiguration = new SerializationTypeConfiguration<T>(this);
				_types.Add(typeof(T), typeConfiguration);
			}
			return (SerializationTypeConfiguration<T>)typeConfiguration;
		}
    }


	//public class NonGenericSerializationConfiguration
	//{
	//	private readonly SerializationConfiguration _serializationConfiguration = new SerializationConfiguration();
	//	public SerializationTypeConfiguration ConfigureType(Type type)
	//	{
	//		var baseMethod = typeof(SerializationConfiguration).GetMethod(nameof(SerializationConfiguration.ConfigureType))!;
	//		var genericMethod = baseMethod.MakeGenericMethod(type)!;
	//		return (SerializationTypeConfiguration)genericMethod.Invoke(_serializationConfiguration, Array.Empty<object>());
	//	}
	//}
}

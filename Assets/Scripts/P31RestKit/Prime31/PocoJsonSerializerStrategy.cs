using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Prime31.Reflection;

namespace Prime31
{
	public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
	{
		internal CacheResolver cacheResolver;

		private static readonly string[] Iso8601Format = new string[3] { "yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z", "yyyy-MM-dd\\THH:mm:ss\\Z", "yyyy-MM-dd\\THH:mm:ssK" };

		public PocoJsonSerializerStrategy()
		{
			cacheResolver = new CacheResolver(buildMap);
		}

		protected virtual void buildMap(Type type, SafeDictionary<string, CacheResolver.MemberMap> memberMaps)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				memberMaps.add(propertyInfo.Name, new CacheResolver.MemberMap(propertyInfo));
			}
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				memberMaps.add(fieldInfo.Name, new CacheResolver.MemberMap(fieldInfo));
			}
		}

		public virtual bool serializeNonPrimitiveObject(object input, out object output)
		{
			return trySerializeKnownTypes(input, out output) || trySerializeUnknownTypes(input, out output);
		}

		public virtual object deserializeObject(object value, Type type)
		{
			object obj = null;
			if (value is string)
			{
				string text = value as string;
				obj = ((string.IsNullOrEmpty(text) || (type != typeof(DateTime) && (!ReflectionUtils.isNullableType(type) || Nullable.GetUnderlyingType(type) != typeof(DateTime)))) ? text : ((object)DateTime.ParseExact(text, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal)));
			}
			else if (value is bool)
			{
				obj = value;
			}
			else if (value == null)
			{
				obj = null;
			}
			else if ((value is long && type == typeof(long)) || (value is double && type == typeof(double)))
			{
				obj = value;
			}
			else
			{
				if ((!(value is double) || type == typeof(double)) && (!(value is long) || type == typeof(long)))
				{
					if (value is IDictionary<string, object>)
					{
						IDictionary<string, object> dictionary = (IDictionary<string, object>)value;
						if (ReflectionUtils.isTypeDictionary(type))
						{
							Type type2 = type.GetGenericArguments()[0];
							Type type3 = type.GetGenericArguments()[1];
							Type type4 = typeof(Dictionary<, >).MakeGenericType(type2, type3);
							IDictionary dictionary2 = (IDictionary)CacheResolver.getNewInstance(type4);
							foreach (KeyValuePair<string, object> item in dictionary)
							{
								dictionary2.Add(item.Key, deserializeObject(item.Value, type3));
							}
							obj = dictionary2;
						}
						else
						{
							obj = CacheResolver.getNewInstance(type);
							SafeDictionary<string, CacheResolver.MemberMap> safeDictionary = cacheResolver.loadMaps(type);
							if (safeDictionary == null)
							{
								obj = value;
							}
							else
							{
								foreach (KeyValuePair<string, CacheResolver.MemberMap> item2 in safeDictionary)
								{
									CacheResolver.MemberMap value2 = item2.Value;
									if (value2.Setter != null)
									{
										string key = item2.Key;
										if (dictionary.ContainsKey(key))
										{
											object value3 = deserializeObject(dictionary[key], value2.Type);
											value2.Setter(obj, value3);
										}
									}
								}
							}
						}
					}
					else if (value is IList<object>)
					{
						IList<object> list = (IList<object>)value;
						IList list2 = null;
						if (type.IsArray)
						{
							list2 = (IList)Activator.CreateInstance(type, list.Count);
							int num = 0;
							foreach (object item3 in list)
							{
								list2[num++] = deserializeObject(item3, type.GetElementType());
							}
						}
						else if (ReflectionUtils.isTypeGenericeCollectionInterface(type) || typeof(IList).IsAssignableFrom(type))
						{
							Type type5 = type.GetGenericArguments()[0];
							Type type6 = typeof(List<>).MakeGenericType(type5);
							list2 = (IList)CacheResolver.getNewInstance(type6);
							foreach (object item4 in list)
							{
								list2.Add(deserializeObject(item4, type5));
							}
						}
						obj = list2;
					}
					return obj;
				}
				obj = ((value is long && type == typeof(DateTime)) ? ((object)new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((long)value)) : ((!type.IsEnum) ? ((!typeof(IConvertible).IsAssignableFrom(type)) ? value : Convert.ChangeType(value, type, CultureInfo.InvariantCulture)) : Enum.ToObject(type, value)));
			}
			if (ReflectionUtils.isNullableType(type))
			{
				return ReflectionUtils.toNullableType(obj, type);
			}
			return obj;
		}

		protected virtual object serializeEnum(Enum p)
		{
			return Convert.ToDouble(p, CultureInfo.InvariantCulture);
		}

		protected virtual bool trySerializeKnownTypes(object input, out object output)
		{
			bool result = true;
			if (input is DateTime)
			{
				output = ((DateTime)input).ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is Guid)
			{
				output = ((Guid)input).ToString("D");
			}
			else if (input is Uri)
			{
				output = input.ToString();
			}
			else if (input is Enum)
			{
				output = serializeEnum((Enum)input);
			}
			else
			{
				result = false;
				output = null;
			}
			return result;
		}

		protected virtual bool trySerializeUnknownTypes(object input, out object output)
		{
			output = null;
			Type type = input.GetType();
			if (type.FullName == null)
			{
				return false;
			}
			IDictionary<string, object> dictionary = new JsonObject();
			SafeDictionary<string, CacheResolver.MemberMap> safeDictionary = cacheResolver.loadMaps(type);
			foreach (KeyValuePair<string, CacheResolver.MemberMap> item in safeDictionary)
			{
				if (item.Value.Getter != null)
				{
					dictionary.Add(item.Key, item.Value.Getter(input));
				}
			}
			output = dictionary;
			return true;
		}
	}
}

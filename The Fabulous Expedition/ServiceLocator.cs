using System.Diagnostics;

public static class ServiceLocator
{
	private static Dictionary<Type, object> listServices = new Dictionary<Type, object>();

	public static void AddService<T>(T service)
	{
		if (listServices.ContainsKey(typeof(T)))
			listServices.Remove(typeof(T));
		listServices.Add(typeof(T), service!);
	}

	public static T GetService<T>()
	{
		try
		{
			listServices.ContainsKey(typeof(T));
			return (T)listServices[typeof(T)];
		}
		catch (Exception e)
		{
			Debug.WriteLine("Error on trying to load service: " + typeof(T) + " not found\n" + e);
		}
		return default!;
	}
}

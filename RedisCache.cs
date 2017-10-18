using StackExchange.Redis;
using Newtonsoft.Json;
using System;

public sealed class RedisCache
{
	private readonly Lazy<ConnectionMultiplexer> _redis = new Lazy<ConnectionMultiplexer>(() =>
	{
		ConfigurationOptions options = ConfigurationOptions.Parse("localhost");
		options.AbortOnConnectFail = false;

		return ConnectionMultiplexer.Connect(options.ToString());
	});

	private ConnectionMultiplexer redis
	{
		get
		{
			return _redis.Value;
		}
	}

	public RedisCache() { }

	public T Get<T>(string key)
	{
		if (!redis.IsConnected)
		{
			return default(T);
		}

		try
		{
			var db = redis.GetDatabase();
			var result = db.StringGet(key);
			if (result.IsNull)
			{
				return default(T);
			}

			return JsonConvert.DeserializeObject<T>(result);
		}
		catch (System.Exception)
		{
			// Log problem
			return default(T);
		}
	}

	public void Set<T>(string key, T value, TimeSpan ttl)
	{
		if (!redis.IsConnected)
		{
			return;
		}

		try
		{
			var serializedValue = JsonConvert.SerializeObject(value);
			var db = redis.GetDatabase();
			db.StringSet(key, serializedValue, ttl);
		}
		catch (System.Exception)
		{
			// Log problem
		}
	}

	public void Remove(string key)
	{
		if (!redis.IsConnected)
		{
			return;
		}

		try
		{
			var db = redis.GetDatabase();
			db.KeyDelete(key);
		}
		catch (System.Exception)
		{
			// Log problem
		}
	}
}
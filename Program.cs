using System;
using System.Collections.Generic;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace cache
{
    class Program
    {
        static void Main(string[] args)
        {
			var redisCache = new RedisCache();
			List<string> test = new List<string>();
			test.Add("test1");
			test.Add("test2");
			test.Add("test3");

			while (true)
			{
				Console.WriteLine("-----------------");
				redisCache.Set("mykey", test, TimeSpan.FromSeconds(180));

				var result = redisCache.Get<List<string>>("mykey");
				if (result == null) {
					Console.WriteLine("It is null");
				} else {
					foreach (var item in result)
					{
						Console.WriteLine(item);
					}
				}

				redisCache.Remove("mykey");

				result = redisCache.Get<List<string>>("mykey");
				if (result == null) {
					Console.WriteLine("It is null");
				} else {
					foreach (var item in result)
					{
						Console.WriteLine(item);
					}
				}
				Task.Delay(5000).Wait();
			}
        }
    }
}
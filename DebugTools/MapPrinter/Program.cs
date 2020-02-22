using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MapPrinter
{
	class Program
	{
		static int[,] BuildMap(string filePath, bool extended)
		{
			var addition = extended ? 1 : 0;
			
			var data = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath));
			var heigth = data["Height"].Value<byte>() + addition * 2;
			var width = data["Width"].Value<byte>() + addition * 2;
			var map = new int[width, heigth];

			foreach (var mapObject in data["MapObjects"].Children())
			{
				var x = mapObject["Coordinates"]["X"].Value<int>();
				var y = mapObject["Coordinates"]["Y"].Value<int>();
				switch (mapObject["CellContentType"].Value<string>())
				{
					case "Barrier":
						map[x + addition, y + addition] = 1;
						break;

					case "NotDestroyable":
						map[x + addition, y + addition] = 2;
						break;
				}
			}

			if (extended)
			{
				ExtendMap(map);
			}
			

			return map;
		}

		static void ExtendMap(int[,] map)
		{
			for (int i = map.GetLowerBound(0); i <= map.GetUpperBound(0); ++i)
			{
				map[i, map.GetLowerBound(1)] = 2;
				map[i, map.GetUpperBound(1)] = 2;
			}
			
			for (int i = map.GetLowerBound(1); i <= map.GetUpperBound(1); ++i)
			{
				map[map.GetLowerBound(0), i] = 2;
				map[map.GetUpperBound(0), i] = 2;
			}
		}

		static void Print(int[,] map)
		{
			for (int y = map.GetUpperBound(1); y >= map.GetLowerBound(1); --y)
			{
				for (int x = map.GetLowerBound(0); x <= map.GetUpperBound(0); ++x)
				{
					char print;
					switch (map[x, y])
					{
						case 1:
							print = 'b';
							break;

						case 2:
							print = 'n';
							break;

						default:
							print = ' ';
							break;
					}

					Console.Write(print);
				}

				Console.WriteLine();
			}
		}

		static void Main(string[] args)
		{
			var baseMap = BuildMap(@"d:/maps/1/objects.json", false);
			Print(baseMap);

			Console.WriteLine();
			Console.WriteLine("--------------------------------------");
			Console.WriteLine();
			
			
			var extendedMap = BuildMap(@"d:/maps/1/objects.json", true);
			Print(extendedMap);

			Console.ReadKey();
		}
	}
}
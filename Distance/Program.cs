using System;

namespace Distance
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var firstPoint = new Vector2(234.34, 3434);

			var secondPoint = new Vector2(2323.54, 934.2);

			Console.WriteLine($"Distance: {GetDistance(firstPoint, secondPoint)}");
		}

		private static double GetDistance(Vector2 start, Vector2 end)
		{
			return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
		}

		private struct Vector2
		{
			public readonly double X;

			public readonly double Y;

			public Vector2(double x, double y)
			{
				X = x;
				Y = y;
			}
		}
	}
}
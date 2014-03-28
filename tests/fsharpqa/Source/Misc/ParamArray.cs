using System;

namespace CSharpAssembly
{
	[AttributeUsage(AttributeTargets.All)]
	public class AttributeWithParamArray : Attribute
	{
		public object[] Parameters;

		public AttributeWithParamArray(params object[] x)
		{

			Parameters = x;
		}
	}

	public class Foo
	{
		static int Main(string[] args)
		{
			Console.WriteLine(Foo.Method());
			Console.WriteLine(Foo.Method(1));
			Console.WriteLine(Foo.Method(1, 2));
			Console.WriteLine(Foo.Method(1, 2, 3));
			Console.WriteLine(Foo.Method(1, 2, 3, 4));

			return 0;
		}

		static int Method(params int[] allArgs)
		{
			int total = 0;
			foreach (int i in allArgs)
				total += i;

			return total;
		}
	}
}
//
// Test C# source file that defines interesting EMs
// Feature test for Bug51669
// Compile with: csc /t:library ...

using BaseEmFs;

namespace EmLibCs
{
	public static class EmOnFs
	{
		public static void M1A(this FooA foo) 
		{
		}

		public static void M1B(this FooB foo) 
		{
		}
		
		public static void M2A(this FooA foo, string name)
		{
		}

		public static void M2B(this FooB foo, string name)
		{
		}
		
		public static int M3A(this FooA foo, int x)
		{
			return (x);
		}

		public static int M3B(this FooB foo, int x)
		{
			return (foo.Value + x);
		}
	}
}

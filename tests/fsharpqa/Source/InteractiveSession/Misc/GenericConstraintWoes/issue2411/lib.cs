using System;
public delegate void FezHandler<T>(T obj) where T : IFez;
public delegate void BarHandler<T>(T obj) where T : IBar;
public interface IFez { }

public interface IBar { }
public class Fez
{
	public static void Do<T>(string s, FezHandler<T>a) where T : IFez { System.Console.WriteLine("dooifez"); }
	public static void Do<T>(string s, BarHandler<T> b) where T : IBar { System.Console.WriteLine("dooibar"); }
}
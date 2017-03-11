using System;
public delegate void FooHandler<T>(T obj) where T : IFoo;
public delegate void BarHandler<T>(T obj) where T : IBar;
public interface IFoo { }

public interface IBar { }
public class Foo
{
	public static void Do<T>(string s, FooHandler<T>a) where T : IFoo { System.Console.WriteLine("dooifoo"); }
	public static void Do<T>(string s, BarHandler<T> b) where T : IBar { System.Console.WriteLine("dooibar"); }
}
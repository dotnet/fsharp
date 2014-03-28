using System;
namespace ClassLibrary1
{
    public class Class1
    {
        public virtual void F()
        {
            Console.WriteLine("class1");
        }
    }
    public class Class2 : Class1
    {
        public override sealed void F()
        {
            Console.WriteLine("class2");
        }
    }
/*
    public class Class3 : Class2
    {
        public override void F()
        {
            Console.WriteLine("class2");
        }
    }
 */
}


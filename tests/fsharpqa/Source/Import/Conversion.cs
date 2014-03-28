//  csc /t:library CSharpTypes.cs
namespace CSharpTypes
{
    public class T
    {
        static public explicit operator int(T t)
        {
            return 1;
        }

        static public explicit operator double(T t)
        {
            return 2.0;
        }

        static public implicit operator char(T t)
        {
            return 'a';
        }

        static public implicit operator byte(T t)
        {
            return 1;
        }


    }
}
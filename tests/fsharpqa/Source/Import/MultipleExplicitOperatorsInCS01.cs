namespace Yadda
{
    public class Bar<T> { }
    public class Blah<T,U>
    {
        public static explicit operator Bar<T>(Blah<T,U> whatever) { return null; }
        public static explicit operator Bar<U>(Blah<T,U> whatever) { return null; }
    }
}

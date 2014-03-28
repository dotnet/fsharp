namespace Yadda
{
    public class Bar<T> { }
    public class Blah<T,U>
    {
        public static implicit operator Bar<T>(Blah<T,U> whatever) { return null; }
        public static implicit operator Bar<U>(Blah<T,U> whatever) { return null; }
    }
}

class Test
{
    static int Main()
    {
        bool res = true;
        T0<string> t0str  = new T0<string>();

        T1<char> t1char = new T1<char>();

        T2<int> t2int = new T2<int>();
        if (t2int.Me() != 0)
            res = false;

        T3<uint> t3uint = new T3<uint>();
        if (t3uint.Home(0) != 0)
            res = false;

        if (res = true)
            return 0;

        return 1;
    }
}

class T0<T> : TestModule.I_000<T>
{
}

class T1<T> : TestModule.I_001<T>
{
}

class T2<T> : TestModule.I_002<T>
{
    public T Me () { return default (T); }
    //  abstract Me: unit -> 'a
}

class T3<T> : TestModule.I_003<T>
{
    public T Home(T t) { return t; }
}

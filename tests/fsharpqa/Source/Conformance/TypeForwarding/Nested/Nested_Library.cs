using System;

#if (FORWARD)

[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(Foo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N002.Foo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N003.Foo))]
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(N0041.N0042.Foo))]

# else

public class Foo
{
    public int getValue()
    {
        return 0;
    }

    public class Bar
    {
        public int getValue()
        {
            return 0;
        }
    }
}

namespace N002
{
    public class Foo
    {
        public int getValue()
        {
            return 0;
        }
    }
}

namespace N003
{
    public class Foo
    {
        public int getValue()
        {
            return 0;
        }
        public class Bar
        {
            public int getValue()
            {
                return 0;
            }
        }
    }
}


namespace N0041
{
    namespace N0042
    {
        public class Foo
        {
            public int getValue()
            {
                return 0;
            }
            public class Bar
            {
                public int getValue()
                {
                    return 0;
                }
            }
        }
    }
}

#endif


public class Baz
{
    public int getValue()
    {
        return 0;
    }
}




namespace N002
{
    public class Baz
    {
        public int getValue()
        {
            return 0;
        }
    }
}




namespace N003
{
    public class Baz
    {
        public int getValue()
        {
            return 0;
        }
    }
}





namespace N0041
{
    namespace N0042
    {
        public class Baz
        {
            public int getValue()
            {
                return 0;
            }
        }
    }
}


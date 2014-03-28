public class Foo
{
    public int getValue()
    {
        return 1;
    }

    public class Bar
    {
        public int getValue()
        {
            return -2;
        }
    }
}

namespace N002
{
    public class Foo
    {
        public int getValue()
        {
            return -1;
        }
    }
}

namespace N003
{
    public class Foo
    {
        public int getValue()
        {
            return -2;
        }
        public class Bar
        {
            public int getValue()
            {
                return 1;
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
                return -2;
            }
            public class Bar
            {
                public int getValue()
                {
                    return 1;
                }
            }
        }
    }
}

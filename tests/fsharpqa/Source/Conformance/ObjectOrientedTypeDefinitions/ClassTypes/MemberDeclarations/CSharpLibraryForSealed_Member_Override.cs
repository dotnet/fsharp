namespace CSLib
{
    abstract public class B0
    {
        abstract public int M(object o);
        abstract public int M(int a);
    }

    abstract public class B1 : B0
    {
        /// <summary>
        /// You cannot override this one!
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public override sealed int M(int a)
        {
            return a + 1;
        }

        /// <summary>
        /// This one can be overridden
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override int M(object o)
        {
            return 0xB0;
        }
    }
}

namespace CSLib2
{
    abstract public class B0
    {
        abstract public int M(object o);
        abstract public int M(int a);
    }

    abstract public class B1 : B0
    {
        /// <summary>
        /// You cannot override this one!
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public override sealed int M(int a)
        {
            return a + 1;
        }

        /// <summary>
        /// This one can be overridden
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override int M(object o)
        {
            return 0xB0;
        }
    }

    abstract public class B2 : B1
    {
        /// <summary>
        /// This one can be overridden
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override int M(object o)
        {
            return 0xB0;
        }
    }


}

namespace CSLib3
{
    abstract public class B0
    {
        abstract public int M(object o);
        abstract public int M(int a);
        abstract public int M(char c);
        abstract public int M(float f);
    }

    abstract public class B1 : B0
    {
        /// <summary>
        /// You cannot override this one!
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public override sealed int M(int a)
        {
            return a + 1;
        }

        /// <summary>
        /// This one can be overridden
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override int M(object o)
        {
            return 0xB0;
        }
        public override int M(char c)
        {
            return 80;
        }
        public override int M(float f)
        {
            return 90;
        }
    }
}

namespace CSLib4
{
    abstract public class B0
    {
        abstract public int M(object o);
        abstract public int M(int a);
        abstract public int M(char c);
        abstract public int M(float f);

        abstract public int N(object o);
        abstract public int N(int a);
        abstract public int N(char c);
        abstract public int N(float f);
    }

    abstract public class B1 : B0
    {
        /// <summary>
        /// You cannot override this one!
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public override sealed int M(int a) { return a + 1; }
        public override sealed int N(int a) { return a + 1; }

        /// <summary>
        /// This one can be overridden
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override int M(object o) { return 0xB0; }
        public override int M(char c) { return 80; }
        public override int M(float f) { return 90; }

        public override int N(object o) { return 0xB0; }
        public override int N(char c) { return 80; }
        public override int N(float f) { return 90; }

    }
}

namespace CSLib5
{
    abstract public class B0
    {
        abstract public int M<T>(T o);
        abstract public int M(int a);
        abstract public int M(char c, int a);

        abstract public int N<T>(T o);
        abstract public int N(int a);
        abstract public int N(char c, int a);

    }

    abstract public class B1 : B0
    {
        /// <summary>
        /// You cannot override this one!
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public override sealed int M(int a) { return a + 1; }
        public override sealed int N(int a) { return a + 1; }

        /// <summary>
        /// This one can be overridden
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override int M<T>(T o) { return 0xB0; }
        public override int N<T>(T o) { return 0xB0; }

    }
}

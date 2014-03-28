// Nothing special here: just a simple class with a static method
// This class will be consumed by the ReferenceArchAndPlatformArchMismatch test
// after being compiled for different architectures.

namespace MyLib
{
    public class Class1
    {
        public static int GetAnswer() { return 42; }
    }
}

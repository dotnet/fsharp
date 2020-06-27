using System;

namespace CSharpLib3
{
    public static class Extensions
    {
        public static void Test(this in DateTime dt)
        {

        }

        public static ref readonly DateTime Test2(this ref DateTime dt)
        {
            return ref dt;
        }
    }
}
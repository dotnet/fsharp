using System;

namespace CSharpException
{
    public class CSharpException : Exception
    {
        public override string Message
        {
            get
            {
                return "C#Exception";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TypeProviderInCSharp
{
    static class Helpers
    {
        public static void TraceCall()
        {
            //var st = new StackTrace();
            //var caller_sf = st.GetFrames()[1];
            //var caller_method = caller_sf.GetMethod();
            
            //// var caller_params = caller_method.GetParameters();
            //Console.WriteLine("Called {0}.{1}.{2}", caller_method.DeclaringType.Namespace, caller_method.DeclaringType.Name, caller_method.Name);
        }
    }
}

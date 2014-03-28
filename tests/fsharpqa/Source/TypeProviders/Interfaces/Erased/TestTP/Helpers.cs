using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;
using System.Linq;

namespace TypeProviderInCSharp
{
    static class Helpers
    {
        public static void TraceCall()
        {
#if TRACE

            var st = new StackTrace();
            var caller_sf = st.GetFrames()[1];
            var caller_method = caller_sf.GetMethod();

            //if (caller_sf.ToString() == lastCall)
            //    return;
            //else
            //    lastCall = caller_sf.ToString();


            var caller_method_from_compiler_non_tainted = st.GetFrames().Skip(2).SkipWhile(x => x.GetMethod().DeclaringType.FullName.Contains("Microsoft.FSharp.Compiler.Tainted")).FirstOrDefault();

            // var caller_params = caller_method.GetParameters();
            Console.WriteLine("Called {0}.{1}.{2} [from {3}]", 
                                    caller_method.DeclaringType.Namespace, 
                                    caller_method.DeclaringType.Name, 
                                    caller_method.Name, 
                                    caller_method_from_compiler_non_tainted.GetMethod().Name);
#else
#endif
        }


        internal class TypeProviderCustomAttributeData : CustomAttributeData
        {
            protected readonly Attribute _a;
            public TypeProviderCustomAttributeData(Attribute a)
            {
                _a = a;
            }

            public override ConstructorInfo Constructor
            {
                get
                {
                    return _a.GetType().GetConstructors()[0];
                }
            }

            public override IList<CustomAttributeTypedArgument> ConstructorArguments
            {
                get
                {
                    if (_a is TypeProviderXmlDocAttribute)
                        return new List<CustomAttributeTypedArgument>() { new CustomAttributeTypedArgument(typeof(string), ((TypeProviderXmlDocAttribute)_a).CommentText) };
                    else // if (_a is TypeProviderDefinitionLocationAttribute || _a is TypeProviderEditorHideMethodsAttribute)
                        return new List<CustomAttributeTypedArgument>();
                }
            }

            public override IList<CustomAttributeNamedArgument> NamedArguments
            {
                get
                {
                    if (_a is TypeProviderDefinitionLocationAttribute)
                    {
                        var t = _a.GetType();
                        return new List<CustomAttributeNamedArgument>() { new CustomAttributeNamedArgument(t.GetProperty("Column"), ((TypeProviderDefinitionLocationAttribute)_a).Column),
                                                                      new CustomAttributeNamedArgument(t.GetProperty("FilePath"), ((TypeProviderDefinitionLocationAttribute)_a).FilePath),
                                                                      new CustomAttributeNamedArgument(t.GetProperty("Line"), ((TypeProviderDefinitionLocationAttribute)_a).Line) };
                    }
                    else // if (_a is TypeProviderXmlDocAttribute || _a is TypeProviderEditorHideMethodsAttribute)
                        return new List<CustomAttributeNamedArgument>();
                }
            }
        }

    }
}

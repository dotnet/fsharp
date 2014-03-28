using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    class ArtificialMethodInfo : MethodInfo
    {
        string _Name;
        Type _DeclaringType;
        Type _ReturnType;
        MethodAttributes _MethodAttributes;
        ParameterInfo[] _ParameterInfo;

        bool isGenericMethod;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="DeclaringType"></param>
        /// <param name="ReturnType"></param>
        /// <param name="MethodAttributes"></param>
        /// <param name="Parameters"></param>
        /// <param name="isGenericMethod">Spec mandate this to be 'False'</param>
        /// <param name="isVirtual"></param>
        /// <param name="isAbstract"></param>
        /// <param name="isConstructor">Should be false</param>
        /// <param name="isFinal"></param>
        /// <param name="isHideBySig"></param>
        /// <param name="isStatic"></param>
        public ArtificialMethodInfo(string Name, Type DeclaringType, Type ReturnType, MethodAttributes MethodAttributes, ParameterInfo[] Parameters, 
                                    bool isGenericMethod

                                    )
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _ReturnType = ReturnType;
            _MethodAttributes = MethodAttributes;
            _ParameterInfo = Parameters;

            this.isGenericMethod = isGenericMethod;


        }

        public override string Name
        {
            get 
            { 
                Helpers.TraceCall();
                return _Name;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return _DeclaringType;
            }
        }

        // Make the method Public and Static - 
        // TODO: should be configurable in the ctor...
        public override MethodAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                return _MethodAttributes;

            }
        }

        // No params
        // TODO: should be configurable in the ctor...
        public override ParameterInfo[] GetParameters()
        {
            Helpers.TraceCall();
            return (_ParameterInfo == null) ? new ParameterInfo[] {  } : _ParameterInfo;
        }

        public override ParameterInfo ReturnParameter
        {
            get
            {
                //Helpers.TraceCall();
                //var retvalpi = new ArtificialParamInfo(typeof(List<>), true);
                //return retvalpi;
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();

            }
        }

        public override Type ReturnType
        {
            get
            {
                Helpers.TraceCall();
                return _ReturnType;
            }
        }

        public override MethodInfo GetBaseDefinition()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get 
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get 
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override Type ReflectedType
        {
            get 
            { 
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            Helpers.TraceCall();
            var attrs = new List<CustomAttributeData>();

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute("This is a synthetic *method* created by me!")));

            var f = System.IO.Path.GetTempFileName() + ".fs";
            System.IO.File.WriteAllText(f, string.Format("// This is a fake definition file to test TypeProviderDefinitionLocationAttribute for method {0}.{1}.{2}\nnamespace {0}\ntype {1} = static member {2}() = [|1,2,3|]", _DeclaringType.Namespace, _DeclaringType.Name, _Name));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 22 + _DeclaringType.Name.Length, FilePath = f, Line = 3 }));

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));

            return attrs;


        }

        public override CallingConventions CallingConvention
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool ContainsGenericParameters
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override Delegate CreateDelegate(Type delegateType)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Delegate CreateDelegate(Type delegateType, object target)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type[] GetGenericArguments()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MethodInfo GetGenericMethodDefinition()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        // TODO: update when bug 199859 is fixed.
        // We should never be here (I think)
        public override int GetHashCode()
        {
            Helpers.TraceCall();
            return base.GetHashCode();
            //Debug.Assert(false, "NYI");
            //throw new NotImplementedException();
        }

        public override MethodBody GetMethodBody()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override bool IsGenericMethod
        {
            get
            {
                Helpers.TraceCall();
                return isGenericMethod;
            }
        }

        public override bool IsGenericMethodDefinition
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool IsSecurityCritical
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool IsSecuritySafeCritical
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool IsSecurityTransparent
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MemberTypes MemberType
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override int MetadataToken
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override Module Module
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }
    }
}

module Neg41Module

id (fun x -> x) id 
 ()
let x = 1

open System
open System.Reflection

type MyType() = 
    inherit Type()
    override this.GUID : Guid= failwith "Not implemented"
    override this.InvokeMember(name: string, invokeAttr: BindingFlags, binder: Binder, target: obj, args: obj [], modifiers: ParameterModifier [], culture: System.Globalization.CultureInfo, namedParameters: string []) : obj = failwith "Not implemented"
    override this.Assembly : Assembly = failwith "Not implemented"
    override this.FullName : string = failwith "Not implemented"
    override this.Namespace : string = failwith "Not implemented"
    override this.AssemblyQualifiedName : string = failwith "Not implemented"
    override this.BaseType : Type = typeof<obj>
    override this.GetConstructorImpl(bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier []) : ConstructorInfo= failwith "Not implemented"
    override this.GetConstructors(bindingAttr: BindingFlags) : ConstructorInfo []= failwith "Not implemented"
    override this.GetMethodImpl(name: string, bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier []) : MethodInfo= failwith "Not implemented"
    override this.GetMethods(bindingAttr: BindingFlags) : MethodInfo []= failwith "Not implemented"
    override this.GetField(name: string, bindingAttr: BindingFlags) : FieldInfo= failwith "Not implemented"
    override this.GetFields(bindingAttr: BindingFlags) : FieldInfo []= failwith "Not implemented"
    override this.GetInterface(name: string, ignoreCase: bool) : Type = failwith "Not implemented"
    override this.GetInterfaces() : Type[] = failwith "Not implemented"
    override this.GetEvent(name: string, bindingAttr: BindingFlags) : EventInfo= failwith "Not implemented"
    override this.GetEvents(bindingAttr: BindingFlags) : EventInfo []= failwith "Not implemented"
    override this.GetPropertyImpl(name: string, bindingAttr: BindingFlags, binder: Binder, returnType: System.Type, types: System.Type [], modifiers: ParameterModifier []) : PropertyInfo = failwith "Not implemented"
    override this.GetProperties(bindingAttr: BindingFlags) : PropertyInfo []= failwith "Not implemented"
    override this.GetNestedTypes(bindingAttr: BindingFlags) : Type [] = failwith "Not implemented"
    override this.GetNestedType(name: string, bindingAttr: BindingFlags) : Type = failwith "Not implemented"
    override this.GetMembers(bindingAttr: BindingFlags) : MemberInfo []= failwith "Not implemented"
    override this.GetAttributeFlagsImpl() : TypeAttributes= failwith "Not implemented"
    override this.IsArrayImpl() : bool= failwith "Not implemented"
    override this.IsByRefImpl() : bool= failwith "Not implemented"
    override this.IsPointerImpl() : bool= failwith "Not implemented"
    override this.IsPrimitiveImpl() : bool= failwith "Not implemented"
    override this.IsCOMObjectImpl() : bool= failwith "Not implemented"
    override this.GetElementType() : Type= failwith "Not implemented" 
    override this.HasElementTypeImpl() : bool= failwith "Not implemented"
    override this.UnderlyingSystemType : Type = failwith "Not implemented"
    override this.Name : string = failwith "Not implemented"
    override this.Module : Module = failwith "Not implemented"
    override this.GetCustomAttributes(``inherit``: bool) : obj []= failwith "Not implemented"
    override this.GetCustomAttributes(attributeType: Type, ``inherit``: bool) : obj []= failwith "Not implemented"
    override this.IsDefined(attributeType: Type, ``inherit``: bool) : bool= failwith "Not implemented"

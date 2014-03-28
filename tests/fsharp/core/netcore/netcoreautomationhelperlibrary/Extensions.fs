// extension members put in place to fill in gaps in the portable profile, so that test code does not need to be updated
// extensions either call out to hooks, or if the member is very simple, just implement the functionality locally
[<AutoOpen>]
module PortableExtensions
open System
open System.Collections.Generic
open System.Reflection
open System.Linq

#if NetCore

open System.Threading
open System.Threading.Tasks

type Assembly with

    member this.GetCustomAttributes (attributeType : Type, ``inherit`` : bool) =
        CustomAttributeExtensions.GetCustomAttributes(this, attributeType).ToArray()

type ConstructorInfo with
    member this.GetCustomAttributes(attributeType : Type, ``inherit`` : bool) =
        CustomAttributeExtensions.GetCustomAttributes(this, attributeType, ``inherit``).ToArray()

type PropertyInfo with
    member this.GetGetMethod(visibility : bool) =
        this.GetMethod

    member this.GetSetMethod(visibility : bool) =
        this.SetMethod

type MethodInfo with
    member this.GetCustomAttributes(attributeType : Type, ``inherit`` : bool) =
        CustomAttributeExtensions.GetCustomAttributes(this, attributeType, ``inherit``).ToArray()

type PropertyInfo with

    member this.GetCustomAttributes(``inherit`` : bool) =
        CustomAttributeExtensions.GetCustomAttributes(this, ``inherit``).ToArray()

type System.Type with

    member this.Assembly : Assembly =
        this.GetTypeInfo().Assembly
    
    // does not consider base types, could need a fuller implementation if this causes tests to fail
    member this.GetConstructors() : ConstructorInfo[] =
        this.GetTypeInfo().DeclaredConstructors.ToArray()

    // does not consider base types, could need a fuller implementation if this causes tests to fail
    member this.GetConstructor(types : Type[]) =
        this.GetTypeInfo().DeclaredConstructors.Where( fun ci -> ci.GetParameters().Select<ParameterInfo, Type>( fun pi -> pi.ParameterType ).SequenceEqual(types) ).FirstOrDefault()

    member this.GetCustomAttributes (attributeType : Type, ``inherit`` : bool) =
        CustomAttributeExtensions.GetCustomAttributes(this.GetTypeInfo() :> MemberInfo, attributeType, ``inherit``).ToArray()

    // does not consider base types, could need a fuller implementation if this causes tests to fail
    member this.GetField(name : string) =
        this.GetTypeInfo().GetDeclaredField(name)

    // does consider base types
    member this.GetMethods() =
        let rec getMeths (tyInfo : TypeInfo) includeStatic = 
            [|
                yield! tyInfo.DeclaredMethods |> Seq.where (fun mi -> mi.IsPublic && (includeStatic || not mi.IsStatic))

                match tyInfo.BaseType with
                | null -> ()
                | b -> yield! getMeths (b.GetTypeInfo()) false
            |]

        getMeths (this.GetTypeInfo()) true

    // does consider base types
    member this.GetMethod(name : string) =
        this.GetMethods()
        |> Seq.where (fun mi -> mi.Name = name)
        |> Seq.exactlyOne

    // does not consider base types, could need a fuller implementation if this causes tests to fail
    member this.GetProperty(name : string) : PropertyInfo =
        this.GetTypeInfo().GetDeclaredProperty(name)

    member this.IsValueType : bool =
        this.GetTypeInfo().IsValueType

    member this.IsGenericType : bool =
        this.GetTypeInfo().IsGenericType
        
    member this.GetGenericArguments() : Type [] =
        this.GetTypeInfo().GenericTypeArguments

    member this.GetGenericParameterConstraints() : Type [] =
        this.GetTypeInfo().GetGenericParameterConstraints()

#else
type System.Threading.Thread with 
    static member Sleep(millisecondsTimeout) =
        Hooks.sleep.Invoke(millisecondsTimeout)
#endif

type System.Threading.WaitHandle with
    member this.WaitOne(millisecondsTimeout : int, exitContext : bool) =
        this.WaitOne(millisecondsTimeout)

type System.Array with
    static member FindIndex<'a>( arr : 'a array, pred) =
        Array.findIndex pred arr

type System.IO.MemoryStream with
    member this.Close() = ()

type System.IO.Stream with
    member this.Write(str : string) =
        let bytes = System.Text.Encoding.UTF8.GetBytes(str)
        this.Write(bytes, 0, bytes.Length)

    member this.Close() = ()

type System.IO.StreamReader with
    member this.Close() = ()

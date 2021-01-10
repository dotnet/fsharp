// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.FSharp.Collections

open System
open System.Collections
open System.Numerics
open System.Reflection
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

module internal MapSetDefaultComparison = 
    type CompareHelper<'T when 'T : comparison>() =
        static let c = LanguagePrimitives.FastGenericComparer
        
        // A constrained call to IComparable<'T>.CompareTo                
        static member private CompareCG<'U when  'U :> IComparable<'U>>(l:'U, r:'U):int = l.CompareTo(r)

        // A call to IComparable.CompareTo
        static member private CompareC<'U when  'U :> IComparable>(l:'U, r:'U):int = l.CompareTo(r)

        static member val CompareToDlg : Func<'T,'T,int> =
                let dlg =
                    let ty = typeof<'T>
                    try 
                        let normalCmp =
                            not (typeof<IStructuralComparable>.IsAssignableFrom(ty))
                            && isNull (Attribute.GetCustomAttribute(ty, typeof<NoComparisonAttribute>))
                            && isNull (Attribute.GetCustomAttribute(ty, typeof<StructuralComparisonAttribute>))
                            && not (ty.IsArray)
                             
                        // See #816, IComparable<'T> actually does not satisfy comparison constraint, but it should be preferred 
                        if typeof<IComparable<'T>>.IsAssignableFrom(ty) then 
                            let m =
                                typeof<CompareHelper<'T>>.GetMethod("CompareCG", BindingFlags.NonPublic ||| BindingFlags.Static)
                                    .MakeGenericMethod([|ty|])
                            Delegate.CreateDelegate(typeof<Func<'T,'T,int>>, m) :?> Func<'T,'T,int>
                        elif typeof<IComparable>.IsAssignableFrom(ty) && normalCmp then 
                            let m =
                                typeof<CompareHelper<'T>>.GetMethod("CompareC", BindingFlags.NonPublic ||| BindingFlags.Static)
                                    .MakeGenericMethod([|typeof<'T>|])
                            Delegate.CreateDelegate(typeof<Func<'T,'T,int>>, m) :?> Func<'T,'T,int>
                        else null
                    with _ -> null
                dlg
            with get
            
        // If backed by static readonly field that will be JIT-time constant
        static member val IsIComparable = not(isNull CompareHelper<'T>.CompareToDlg) with get
            
        [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
        static member Compare(l:'T, r:'T):int =
            // Should use IsIComparable when it's backed by static readonly field
            if isNull CompareHelper<'T>.CompareToDlg then
                c.Compare(l, r)
            else
                CompareHelper<'T>.CompareToDlg.Invoke(l,r)
            
    // Constructors are not inlined by F#, but JIT could inline them.
    // This is what we need here, because LanguagePrimitives.FastGenericComparer.Compare
    // has a .tail prefix that breaks the typeof(T)==typeof(...) JIT optimization in cmp
    // A struct with a single int field should be lowered by JIT.
    [<Struct>]
    [<NoEquality; NoComparison>] 
    type Comparison<'T when 'T : comparison> =
        struct
            val Value: int
            [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
            new(l:'T,r:'T) = { Value = CompareHelper<'T>.Compare(l, r) }
        end
        
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    let cmp<'T when 'T : comparison> (l:'T) (r:'T) : int =
        // See the pattern explanation: https://github.com/dotnet/runtime/blob/4b8d10154c39b1f56424d4ba2068a3150d90d475/src/libraries/System.Private.CoreLib/src/System/Numerics/Vector_1.cs#L14
        // All types that implement IComparable<'T> and are accessible here without additional dependencies should be in the list 
        if Type.op_Equality(typeof<'T>, typeof<sbyte>) then unbox<sbyte>(box(l)).CompareTo(unbox<sbyte>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<int16>) then unbox<int16>(box(l)).CompareTo(unbox<int16>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<int32>) then unbox<int32>(box(l)).CompareTo(unbox<int32>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<int64>) then unbox<int64>(box(l)).CompareTo(unbox<int64>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<byte>) then unbox<byte>(box(l)).CompareTo(unbox<byte>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<uint16>) then unbox<uint16>(box(l)).CompareTo(unbox<uint16>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<uint32>) then unbox<uint32>(box(l)).CompareTo(unbox<uint32>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<uint64>) then unbox<uint64>(box(l)).CompareTo(unbox<uint64>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<nativeint>) then
            unbox<nativeint>(box(l)).ToInt64().CompareTo( (unbox<nativeint>(box(r))).ToInt64())
        else if Type.op_Equality(typeof<'T>, typeof<unativeint>) then
            unbox<unativeint>(box(l)).ToUInt64().CompareTo( (unbox<unativeint>(box(r))).ToUInt64())
        else if Type.op_Equality(typeof<'T>, typeof<bool>) then unbox<bool>(box(l)).CompareTo(unbox<bool>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<char>) then unbox<char>(box(l)).CompareTo(unbox<char>(box(r)))
        
        // F# rules for floats
        else if Type.op_Equality(typeof<'T>, typeof<float>) then
            let l = unbox<float>(box(l))
            let r = unbox<float>(box(r))
            if  l < r then (-1)
            elif l > r then (1)
            elif l = r then (0)
            elif r = r then (-1)
            elif l = l then (1)
            else 0
        else if Type.op_Equality(typeof<'T>, typeof<float32>) then
            let l = unbox<float32>(box(l))
            let r = unbox<float32>(box(r))
            if  l < r then (-1)
            elif l > r then (1)
            elif l = r then (0)
            elif r = r then (-1)
            elif l = l then (1)
            else 0
        else if Type.op_Equality(typeof<'T>, typeof<decimal>) then unbox<decimal>(box(l)).CompareTo(unbox<decimal>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<DateTime>) then unbox<DateTime>(box(l)).CompareTo(unbox<DateTime>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<DateTimeOffset>) then unbox<DateTimeOffset>(box(l)).CompareTo(unbox<DateTimeOffset>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<TimeSpan>) then unbox<TimeSpan>(box(l)).CompareTo(unbox<TimeSpan>(box(r)))
        
        else if Type.op_Equality(typeof<'T>, typeof<BigInteger>) then unbox<BigInteger>(box(l)).CompareTo(unbox<BigInteger>(box(r)))
        else if Type.op_Equality(typeof<'T>, typeof<Guid>) then unbox<Guid>(box(l)).CompareTo(unbox<Guid>(box(r)))
        
        else if Type.op_Equality(typeof<'T>, typeof<string>) then
            // same as in GenericComparisonFast
            String.CompareOrdinal(unbox<string>(box(l)),(unbox<string>(box(r))))
        
        else Comparison(l,r).Value
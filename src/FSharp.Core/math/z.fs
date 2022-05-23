// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "44" // This construct is deprecated. This function is for use by compiled F# code and should not be used directly

namespace Microsoft.FSharp.Math

// Deliberately left empty
//
//  FSharp.Core previously exposed the namespace Microsoft.FSharp.Math even though there were no types in it.
//  This retains that.
//  Existing programs could, and did contain the line:
//  open FSharp.Math
//

namespace Microsoft.FSharp.Core

type bigint = System.Numerics.BigInteger

open System
open System.Diagnostics.CodeAnalysis
open System.Globalization
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open System.Numerics

[<AutoOpen>]
module NumericLiterals =

    module NumericLiteralI = 

        let tab64 = new System.Collections.Generic.Dictionary<int64,obj>()
        let tabParse = new System.Collections.Generic.Dictionary<string,obj>()
        
        let FromInt64Dynamic (value:int64) : obj = 
            lock tab64 (fun () -> 
                let mutable res = Unchecked.defaultof<_> 
                let ok = tab64.TryGetValue(value,&res)
                if ok then res else 
                res <- BigInteger(value)
                tab64.[value] <- res
                res)                 

        let inline get32 (x32:int32) =  FromInt64Dynamic (int64 x32)

        let inline isOX s = not (System.String.IsNullOrEmpty(s)) && s.Length > 2 && s.[0] = '0' && s.[1] = 'x'
        
        let FromZero () : 'T = 
            (get32 0 :?> 'T)
            when 'T : BigInteger = BigInteger.Zero 

        let FromOne () : 'T = 
            (get32 1 :?> 'T)
            when 'T : BigInteger = BigInteger.One

        let FromInt32 (value:int32): 'T = 
            (get32 value :?> 'T)
            when 'T : BigInteger = new BigInteger(value)
        
        let FromInt64 (value:int64): 'T = 
            (FromInt64Dynamic value :?> 'T)
            when 'T : BigInteger = new BigInteger(value)
            
        let getParse s = 
            lock tabParse (fun () -> 
            let mutable res = Unchecked.defaultof<_> 
            let ok = tabParse.TryGetValue(s,&res)
            if ok then 
                res 
            else 
                let v = 
                   if  isOX s then 
                      BigInteger.Parse (s.[2..],NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture)
                   else
                      BigInteger.Parse (s,NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture)
                res <-  v
                tabParse.[s] <- res
                res)

        let FromStringDynamic (text:string) : obj = 
            getParse text
            
        let FromString (text:string) : 'T = 
            (FromStringDynamic text :?> 'T)
            when 'T : BigInteger = getParse text


// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities.Diagnostic
open System
open System.Diagnostics
open System.Reflection
open System.Collections.Generic

#if EXTENSIBLE_DUMPER
#if DEBUG

type internal ExtensibleDumper(x:obj) =
    static let mutable dumpers = new  Dictionary<Type,(Type*MethodInfo) option>()

    [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
    member self.Debug = ExtensibleDumper.Dump(x)

    static member Dump(o:obj) : string = 
        if o = null then "null"
        else 
            let dumpeeType = o.GetType()
            
            let DeriveDumperName(dumpeeType:Type) =
                "Internal.Utilities.Diagnostic." + dumpeeType.Name + "Dumper"            

            match dumpers.TryGetValue(dumpeeType) with 
            | true, Some(dumperType, methodInfo) -> 
                try 
                    let dumper = Activator.CreateInstance(dumperType,[| o |])
                    let result = methodInfo.Invoke(dumper, [||]) 
                    downcast result 
                with e -> "Exception during dump: "+e.Message
            | true, None -> 
                "There is no dumper named "+(DeriveDumperName dumpeeType)+" with single constructor that takes "+dumpeeType.Name+" and property named Dump."
            | false, _ -> 
                let TryAdd(dumpeeType:Type) =
                    let dumperDerivedName = DeriveDumperName(dumpeeType)                     
                    let dumperAssembly = dumpeeType.Assembly // Dumper must live in the same assembly as dumpee
                    let dumperType = dumperAssembly.GetType(dumperDerivedName, (*throwOnError*)false)
                    if dumperType <> null then 
                        let dumpMethod = dumperType.GetMethod("ToString")
                        if dumpMethod <> null then 
                            let constructors = dumperType.GetConstructors()
                            if constructors.Length = 1 then
                                let constr = constructors.[0]
                                let parameters = constr.GetParameters()
                                if parameters.Length = 1 then
                                    dumpers.[o.GetType()] <- Some(dumperType,dumpMethod)
                    dumpers.ContainsKey(o.GetType())       
                           
                if (not(TryAdd(o.GetType()))) then
                    if (not(TryAdd(o.GetType().BaseType))) then 
                        dumpers.[dumpeeType] <- None
                ExtensibleDumper.Dump(o) // Show the message                                                    
                                    



#endif
#endif

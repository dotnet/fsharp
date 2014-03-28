// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Pickle 

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.Tast

// Fixup pickled data w.r.t. a set of CCU thunks indexed by name
[<NoEquality; NoComparison>]
type PickledDataWithReferences<'RawData> = 
    { /// The data that uses a collection of CcuThunks internally
      RawData: 'RawData; 
      /// The assumptions that need to be fixed up
      FixupThunks: list<CcuThunk> } 

    member Fixup : (CcuReference -> CcuThunk) -> 'RawData
    /// Like Fixup but loader may return None, in which case there is no fixup.
    member OptionalFixup: (CcuReference -> CcuThunk option) -> 'RawData
    
#if INCLUDE_METADATA_WRITER
type WriterState 

type pickler<'T> = 'T -> WriterState -> unit
val internal p_byte : int -> WriterState -> unit
val internal p_bool : bool -> WriterState -> unit
val internal p_int : int -> WriterState -> unit
val internal p_string : string -> WriterState -> unit
val internal p_lazy : 'T pickler -> Lazy<'T> pickler
val inline  internal p_tup2 : ('T1 pickler) -> ('T2 pickler) -> ('T1 * 'T2) pickler
val inline  internal p_tup3 : ('T1 pickler) -> ('T2 pickler) -> ('T3 pickler) -> ('T1 * 'T2 * 'T3) pickler
val inline  internal p_tup4 : ('T1 pickler) -> ('T2 pickler) -> ('T3 pickler) -> ('T4 pickler) -> ('T1 * 'T2 * 'T3 * 'T4) pickler
val internal p_array : 'T pickler -> 'T[] pickler
val internal p_namemap : 'T pickler -> NameMap<'T> pickler
val internal p_const : Const pickler
val internal p_vref : string -> ValRef pickler
val internal p_tcref : string -> TyconRef pickler
val internal p_ucref : UnionCaseRef pickler
val internal p_expr : Expr pickler
val internal p_typ : TType pickler
val internal pickleModuleOrNamespace : pickler<ModuleOrNamespace>
val internal pickleModuleInfo : pickler<PickledModuleInfo>
val pickleObjWithDanglingCcus : string -> Env.TcGlobals -> scope:CcuThunk -> ('T pickler) -> 'T -> byte[]
#else
#endif

type ReaderState 

type unpickler<'T> = ReaderState -> 'T
val internal u_byte : ReaderState -> int
val internal u_bool : ReaderState -> bool
val internal u_int : ReaderState -> int
val internal u_string : ReaderState -> string
val internal u_lazy : 'T unpickler -> Lazy<'T> unpickler
val inline  internal u_tup2 : ('T2 unpickler) -> ('T3 unpickler ) -> ('T2 * 'T3) unpickler
val inline  internal u_tup3 : ('T2 unpickler) -> ('T3 unpickler ) -> ('T4 unpickler ) -> ('T2 * 'T3 * 'T4) unpickler
val inline  internal u_tup4 : ('T2 unpickler) -> ('T3 unpickler ) -> ('T4 unpickler ) -> ('T5 unpickler) -> ('T2 * 'T3 * 'T4 * 'T5) unpickler
val internal u_array : 'T unpickler -> 'T[] unpickler
val internal u_namemap : 'T unpickler -> NameMap<'T> unpickler
val internal u_const : Const unpickler
val internal u_vref : ValRef unpickler
val internal u_tcref : TyconRef unpickler
val internal u_ucref : UnionCaseRef unpickler
val internal u_expr : Expr unpickler
val internal u_typ : TType unpickler
val internal unpickleModuleOrNamespace : ReaderState -> ModuleOrNamespace
val internal unpickleModuleInfo : ReaderState -> PickledModuleInfo
val internal unpickleObjWithDanglingCcus : string -> viewedScope:ILScopeRef -> ilModule:ILModuleDef -> ('T  unpickler) -> byte[] ->  PickledDataWithReferences<'T>




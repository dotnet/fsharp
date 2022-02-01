module M

type ClassPublic() = class end

type private ClassPrivate() =   // CUT
   static member ClassPrivateProperty = 1   // CUT

type InterfacePublic = interface end

type private InterfacePrivate = interface end

// Val 
let private PrivateFunction() = 0xabba    // CUT


//
// type GenericClass<'T when 'T :> InterfacePrivate >() = // NOTE: This is not allowed   
//    class end


[<AttributeUsage(AttributeTargets.All)>]
type PublicAttribute () =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.All)>]
type private PrivateAttribute () =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.All)>]
type PublicWithInternalConstructorAttribute internal () =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.All)>]
type PublicWithInternalSetterPropertyAttribute() =
    inherit Attribute()
    member val internal Prop1 : int = 0 with get, set

[<PublicAttribute()>] // KEEP
[<PrivateAttribute()>] // note: this is allowed! accessibility of attributes is implied by structure of attribute! CUT
[<PublicWithInternalConstructorAttribute()>] // note: this is allowed!  accessibility of attributes is implied by structure of attribute! CUT
[<PublicWithInternalSetterPropertyAttribute(Prop1=4)>]  // note: this is allowed!  accessibility of attributes is implied by structure of attribute! CUT
type ClassPublicWithPrivateAttributes() = class end

type ClassPublicWithPrivateInterface() =
    interface InterfacePrivate
    static member MPublic1() = 1
    static member private MPrivate1() = ClassPublic()   // CUT
    static member private MPrivate2() = ClassPrivate()  // CUT

type private InterfacePrivateInheritingInterfacePublic = 
   interface
     inherit InterfacePublic
   end

type ClassPublicWithPrivateInterface() =
    interface InterfacePrivateInheritingInterfacePublic
    // TODO: note, restriction should not cut this fully, but rather reduce it to
    //  to InterfacePrivateInheritingInterfacePublic
    // Question: What does C# do here, e.g.
    //    - internal interface extends a public interface
    //    - a public class implements this interface
    //    - what ends up in the reference assembly? 



type private ClassPrivateUsedInPrivateFieldOfPublicStruct = // This must be kept!
  member private x.P = 1

[<Struct>]
type private StructPrivateUsedInPrivateFieldOfPublicStruct = // This must be kept!
  val private X: int   // This must be kept!  Computation of "has default value" and "unmanaged" depend on this!

[<Struct>]
type S = 
   val private X1: ClassPrivateUsedInPrivateFieldOfPublicStruct
   val private X2: StructPrivateUsedInPrivateFieldOfPublicStruct



type RecordPublic =
    { FPublic: ClassPublic }   

type RecordPublicPrivate =                         // CUT
    private { FRecordPublicPrivate: ClassPrivate }   // CUT

type private RecordPrivate =              // CUT
    { FRecordPrivate: ClassPrivate() }        // CUT

[<Struct>]
type StructRecordPublic =
    { FStructRecordPublic: ClassPublic() }   

[<Struct>]
type StructRecordPublicPrivate =
    private { FStructRecordPublicPrivate: ClassPrivate() }    // CUT

[<Struct>]
type private StructRecordPrivate =                // CUT
    { FStructRecordPrivate: ClassPrivate() }          // CUT

type UnionPublic =
    | UnionPublicCase1 of ClassPublic
    | UnionPublicCase2 of ClassPublic

type UnionPublicPrivate =
    private                                           // CUT
    | UnionPublicPrivateCase1 of ClassPrivate             // CUT
    | UnionPublicPrivateCase2 of ClassPrivate             // CUT

type private UnionPrivate =                           // CUT
    | UnionPrivateCase1 of ClassPrivate                   // CUT
    | UnionPrivateCase2 of ClassPrivate                   // CUT

[<Struct>]
type StructUnionPublic =
    | StructUnionPublicCase1 of ClassPublic

[<Struct>]
type StructUnionPublicPrivate =
    private                                            // CUT
    | StructUnionPublicPrivateCase1 of ClassPrivate       // CUT

[<Struct>]
type private StructUnionPrivate =                     // CUT
    | StructUnionPrivateCase1 of ClassPrivate             // CUT

type private InterfacePrivate = 
   interface
       abstract M: int -> int
   end

type GenericInterfacePublic<'T> = 
   interface
       abstract M: int -> int
   end

type ClassPublicImplementingPrivateInterface() =
    interface InterfacePrivate with // allowed, must be trimmed
        member _.M(x:int) = x
    interface GenericInterfacePublic<InterfacePrivate> with // allowed, must be trimmed
        member _.M(x:int) = x

// type private BaseClassPrivate() = 
//     class
//     end
// type ClassPublicWithPrivateBaseClass() =
//     inherit BaseClassPrivate()  // not allowed


exception ExceptionPublic of ClassPublic
exception ExceptionAbbrevPublic =  ExceptionPublic

// Note, existing checks disallow this, for no good reason
//exception private ExceptionPrivate of ClassPrivate // Trimmed
exception private ExceptionPrivate of int // CUT!
exception private ExceptionAbbrevPrivate = ExceptionPrivate // CUT!

// Implied private
module private ModulePrivate =
    type CImplicitlyPrivate() = class end // CUT
     // + duplicate out all of the above
     



let test1() = PrivateFunction()

let test2() = C() |> ignore

let test3() = C.P


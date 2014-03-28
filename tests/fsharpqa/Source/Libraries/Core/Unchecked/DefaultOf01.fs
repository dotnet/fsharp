// #Regression #Libraries #Unchecked 
#light

// 1760, Implement Unchecked.defaultof<_> (delete LanguagePrimitives.DefaultValueUnchecked)

// Test the 'defaultof<_>' function

// Reference types ---------------------------
type DUType = 
    | A
    | B of int
    | C of DUType * DUType
    
type RecordType = { A : int; B : string; C : DUType }

type ClassType = string
   
type InterfaceType =
    abstract DoStuff : unit -> unit

// Stack types -------------------------------    
type EnumType =
    | A = 1
    | B = 2
    | C = 4
    
type StructType = struct
    val m_ivalue : int
    val m_svalue : string
    member this.IValue = this.m_ivalue
    member this.SValue = this.m_svalue
end

// Test reference types
if Unchecked.defaultof<ClassType>          <> null then exit 1
// This behaivor for DU, Records, and Interfaces is bey design (need to box to get null)
if box(Unchecked.defaultof<DUType>)        <> null then exit 1
if box(Unchecked.defaultof<RecordType>)    <> null then exit 1
if box(Unchecked.defaultof<InterfaceType>) <> null then exit 1

let p = Unchecked.defaultof<EnumType>

// Test stack types
if Unchecked.defaultof<int>   <> 0   then exit 1
if Unchecked.defaultof<float> <> 0.0 then exit 1
if Unchecked.defaultof<EnumType> <> enum 0   then exit 1

if (Unchecked.defaultof<StructType>).IValue <> 0    then exit 1
if (Unchecked.defaultof<StructType>).SValue <> null then exit 1

exit 0

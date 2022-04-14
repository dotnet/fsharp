// #Conformance #TypeInference #Recursion 
#light

// Define a set of mutually recursive types. Verify compiles and works OK

type ClassType<'a>(x:'a) =         
    member self.Value = x
    
and RecordType =
    { field1 : int;
      field2 : ClassType<int> }
    member self.GetField2() = self.field2
    
and UnionType =
    | Case1 of string * AbbrevType1
    | Case2 of string * AbbrevType2
    | Case3 of string * RecordType

and AbbrevType1 = ClassType<int>

and AbbrevType2 = ClassType<string>

and AnotherClassType<'a>(x:'a) = 
    member self.X = x
    interface InterfaceType<'a> with 
        member self.Ident = x
    
and InterfaceType<'a> = 
    abstract Ident : 'a

let test1 = new ClassType<int>(3)
if test1.Value <> 3 then failwith "Failed: 1"

let test2 = { field1 = 41; field2 = test1 }
if test2.GetField2() <> test1 then failwith "Failed: 1"

let test3 = Case3("du type", test2)
if test3 = Case1("foo", new AbbrevType1(4)) then failwith "Failed: 3"

let test4 = new AnotherClassType<float>(3.141)
if test4.X <> 3.141 then failwith "Failed: 4"

let test5 = test4 :> InterfaceType<float>
if test5.Ident <> 3.141 then failwith "Failed: 5"

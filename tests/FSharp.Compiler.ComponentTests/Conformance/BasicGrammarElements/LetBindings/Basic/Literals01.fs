// #Regression #Conformance #DeclarationElements #LetBindings 
#light  

// FSB 1124, Implement constant literals

// Test the ability to define literals, which we can validate by using attributes 
// since only const literals are allowable on attribute constructors.

type EnumType =
    | A = 1
    | B = 2

type CustomAttrib(a:int, b:string, c:float, d:EnumType) =
    inherit System.Attribute()
    
[<Literal>]    
let lit01 = 42

[<Literal>]
let lit02 = "str"

[<Literal>]
let lit03 = 3.141

[<Literal>]
let lit04 = EnumType.A

[<Literal>]
let lit05 = "2" + "3" + "4"

[<Literal>]
let lit06 = "2" + (@"3" + """4""")

[<Literal>]
let lit07 = "2" + lit06

[<Literal>]
let lit08 = System.Reflection.BindingFlags.CreateInstance ||| System.Reflection.BindingFlags.DeclaredOnly

[<Literal>]
let lit09 = 3 ||| 4

[<Literal>]
let lit10 = 3

[<Literal>]
let lit11 = lit10

// If a-d weren't const literals, this wouldn't be legal.
[<CustomAttrib(lit01, lit02, lit03, lit04)>]
type SomeClass() =
    override this.ToString() = "SomeClass"
  
[<System.ObsoleteAttribute("fail" + "me")>]
let foo1 x = ()

[<Literal>]
let lit12 = "2" + "3" + "4" + "5" + "6" + "7"

[<System.ObsoleteAttribute(lit12)>]
let foo2 x = ()

[<Literal>]
let lit13 = 3L ||| (4L ||| 20000L)

[<Literal>]
let Case1 = "x"

[<Literal>]
let Case2 = "a" + "b"

match "ab" with
| Case1 -> failwith "Bad pattern match - case1"
| Case2 -> ()
| _ -> failwith "Bad pattern match"

type EnumInt64 = E = 1L
[<Literal>]
let valueOfEnumInt64 : EnumInt64 = LanguagePrimitives.EnumOfValue 1L

(* why doesn't this work? 
match "ab" with
| ("a" + "b") -> ()
| _ -> failwith "Bad pattern match"
*)

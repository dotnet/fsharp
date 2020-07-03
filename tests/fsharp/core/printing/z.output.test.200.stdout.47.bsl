
> val it : unit = ()

> > val repeatId : string = "A"

> val repeatId : string = "B"

namespace FSI_0005
  val x1 : int
  val x2 : string
  val x3 : 'a option
  val x4 : int option
  val x5 : 'a list
  val x6 : int list
  val x7 : System.Windows.Forms.Form
  val x8 : int [,]
  val x9 : Lazy<string>

namespace FSI_0006
  val x1 : int
  val x2 : string
  val x3 : 'a option
  val x4 : int option
  val x5 : 'a list
  val x6 : int list
  val x7 : System.Windows.Forms.Form
  val x8 : int [,]
  val x9 : Lazy<string>

namespace FSI_0006
  val x1 : int
  val x2 : string
  val x3 : 'a option
  val x4 : int option
  val x5 : 'a list
  val x6 : int list
  val x7 : System.Windows.Forms.Form
  val x8 : int [,]
  val x9 : Lazy<string>

> val x1 : seq<string>
val x2 : seq<string>
val x3 : seq<string>
val f1 : System.Windows.Forms.Form = System.Windows.Forms.Form, Text: f1 form
val fs : System.Windows.Forms.Form [] =
  [|System.Windows.Forms.Form, Text: fs #0;
    System.Windows.Forms.Form, Text: fs #1;
    System.Windows.Forms.Form, Text: fs #2;
    System.Windows.Forms.Form, Text: fs #3;
    System.Windows.Forms.Form, Text: fs #4;
    System.Windows.Forms.Form, Text: fs #5;
    System.Windows.Forms.Form, Text: fs #6;
    System.Windows.Forms.Form, Text: fs #7;
    System.Windows.Forms.Form, Text: fs #8;
    System.Windows.Forms.Form, Text: fs #9;
    System.Windows.Forms.Form, Text: fs #10;
    System.Windows.Forms.Form, Text: fs #11;
    System.Windows.Forms.Form, Text: fs #12;
    System.Windows.Forms.Form, Text: fs #13;
    System.Windows.Forms.Form, Text: fs #14;
    System.Windows.Forms.Form, Text: fs #15;
    System.Windows.Forms.Form, Text: fs #16;
    System.Windows.Forms.Form, Text: fs #17;
    System.Windows.Forms.Form, Text: fs #18;
    System.Windows.Forms.Form, Text: fs #19; ...|]
val xs : string list =
  ["0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"; "11"; "12"; "13";
   "14"; "15"; "16"; "17"; "18"; "19"; ...]
val xa : string [] =
  [|"0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"; "11"; "12"; "13";
    "14"; "15"; "16"; "17"; "18"; "19"; ...|]
val xa2 : string [,] = [["0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"]
                        ["10"; "11"; "12"; "13"; "14"; "15"; "16"; "17"]
                        ["20"; "21"; "22"; "23"; ...]
                        ...]
val sxs0 : Set<string> = set []

> val sxs1 : Set<string> = set ["0"]

> val sxs2 : Set<string> = set ["0"; "1"]

> val sxs3 : Set<string> = set ["0"; "1"; "2"]

> val sxs4 : Set<string> = set ["0"; "1"; "2"; "3"]

> val sxs200 : Set<string> =
  set ["0"; "1"; "10"; "100"; "101"; "102"; "103"; "104"; "105"; ...]

> val msxs0 : Map<int,string> = map []

> val msxs1 : Map<int,string> = map [(0, "0")]

> val msxs2 : Map<int,string> = map [(0, "0"); (1, "1")]

> val msxs3 : Map<int,string> = map [(0, "0"); (1, "1"); (2, "2")]

> val msxs4 : Map<int,string> = map [(0, "0"); (1, "1"); (2, "2"); (3, "3")]

> val msxs200 : Map<int,string> =
  map
    [(0, "0"); (1, "1"); (2, "2"); (3, "3"); (4, "4"); (5, "5"); (6, "6");
     (7, "7"); (8, "8"); ...]

> module M = begin
  val a : string = "sub-binding"
  val b :
    (seq<string> * seq<string> * seq<string> * System.Windows.Forms.Form) option *
    (string list * string list * string [,]) option =
    (Some (<seq>, <seq>, <seq>, System.Windows.Forms.Form, Text: f1 form),
     Some
       (["0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"; "11"; "12";
         "13"; "14"; "15"; "16"; ...], ..., ...))
end
type T =
  new : a: int * b: int -> T
  member AMethod : x: int -> int
  member AProperty : int with get
  static member StaticMethod : x: int -> int
  static member StaticProperty : int with get
val f_as_method : x:int -> int
val f_as_thunk : (int -> int)
val refCell : string ref = { contents = "value" }
module D1 = begin
  val words : System.Collections.Generic.IDictionary<string,int>
  val words2000 : System.Collections.Generic.IDictionary<int,string>
end

> > module D2 = begin
  val words : IDictionary<string,int>
  val words2000 : IDictionary<int,string>
end
val opt1 : 'a option
val opt1b : int option = None
val opt4 : 'a option option option option
val opt4b : int option option option option = Some (Some (Some None))
val opt5 : int list option option option option option list =
  [Some (Some (Some (Some None)));
   Some (Some (Some (Some (Some [1; 2; 3; 4; 5; 6]))));
   Some (Some (Some (Some ...))); ...]
val mkStr : n:int -> string
val strs : string [] =
  [|""; "-"; "--"; "---"; "----"; "-----"; "------"; "-------"; "--------";
    "---------"; "----------"; "-----------"; "------------"; "-------------";
    "--------------"; "---------------"; "----------------";
    "-----------------"; "------------------"; "-------------------"; ...|]
val str7s : string [] =
  [|""; "-------"; "--------------"; "---------------------";
    "----------------------------"; "-----------------------------------";
    "------------------------------------------";
    "-------------------------------------------------";
    "--------------------------------------------------------";
    "---------------------------------------------------------------";
    "----------------------------------------------------------------------";
    "-------------------------------------------------------------"+[16 chars];
    "-------------------------------------------------------------"+[23 chars];
    "-------------------------------------------------------------"+[30 chars];
    "-------------------------------------------------------------"+[37 chars];
    "-------------------------------------------------------------"+[44 chars];
    "-------------------------------------------------------------"+[51 chars];
    "-------------------------------------------------------------"+[58 chars];
    "-------------------------------------------------------------"+[65 chars];
    "-------------------------------------------------------------"+[72 chars];
    ...|]
val grids : string [,] =
  [[""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; "";
    ""; ...]
   ...]

> type tree =
  | L
  | N of tree list
val mkT : w:int -> d:int -> tree
val tree : w:int -> d:int -> tree

> [Building 2 4...done]
val tree_2_4 : tree =
  N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N [L; L]]];
     N [N [N [L; ...]; ...]; ...]; ...]

> [Building 2 6...done]
val tree_2_6 : tree =
  N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N [L; L]]]; N [N ...; ...];
           ...]; ...]; ...]

> [Building 2 8...done]
val tree_2_8 : tree =
  N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N [L; L; ...]; ...];
                    ...]; ...]; ...]; ...]; ...]; ...]

> [Building 2 10...done]
val tree_2_10 : tree =
  N [N [N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N ...; ...];
                          ...]; ...]; ...]; ...]; ...]; ...]; ...]; ...]

> [Building 2 12...done]
val tree_2_12 : tree =
  N [N [N [N [N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; ...]; ...];
                                ...]; ...]; ...]; ...]; ...]; ...]; ...]; ...];
        ...]; ...]

> [Building 2 14...done]
val tree_2_14 : tree =
  N [N [N [N [N [N [N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N ...; ...]; ...];
                                ...]; ...]; ...]; ...]; ...]; ...]; ...]; ...];
        ...]; ...]

> [Building 3 8...done]
val tree_3_8 : tree =
  N [N [N [N [N [N [N [N [L; L; L]; N [L; L; L]; N [L; L; L]]; N ...; ...];
                 ...]; ...]; ...]; ...]; ...]

> [Building 4 8...done]
val tree_4_8 : tree =
  N [N [N [N [N [N [N [N [L; L; L; L]; N [L; L; L; L]; N [L; L; ...]; ...];
                    ...]; ...]; ...]; ...]; ...]; ...]

> [Building 5 8...done]
val tree_5_8 : tree =
  N [N [N [N [N [N [N [N [L; L; L; L; L]; N [L; L; L; L; L]; N ...; ...]; ...];
                 ...]; ...]; ...]; ...]; ...]

> [Building 6 8...done]
val tree_6_8 : tree =
  N [N [N [N [N [N [N [N [L; L; L; L; L; L]; N [L; L; L; L; L; ...]; ...]; ...];
                 ...]; ...]; ...]; ...]; ...]

> [Building 5 3...done]
val tree_5_3 : tree =
  N [N [N [L; L; L; L; L]; N [L; L; L; L; L]; N [L; L; L; L; L; ...]; ...];
     ...]

> > type X =
  | Var of int
  | Bop of int * X * X
val generate : x:int -> X

> val exps : X list =
  [Bop (1, Var 0, Var 0); Var 2;
   Bop (3, Bop (1, Var 0, Var 0), Bop (1, Var 0, ...)); ...]

> module Exprs = begin
  val x1 : X =
    Bop
      (213, Var 106,
       Bop
         (71,
          Bop (35, Bop (17, Var 8, Bop (5, Var 2, Bop (1, Var 0, ...))), ...),
          ...))
  val x2 : X = Var 21342314
  val x3 : X = Var 3214
  val x4 : X = Bop (1231357, Var 615678, Var 410452)
  val x5 : X =
    Bop
      (5234547, Bop (2617273, Var 1308636, Var 872424),
       Bop (1744849, Var 872424, Var 581616))
  val x6 : X =
    Bop
      (923759825, Var 461879912, Bop (307919941, Var 153959970, Var 102639980))
  val x7 : X = Var 2435234
  val x8 : X =
    Bop
      (12396777, Var 6198388,
       Bop
         (4132259,
          Bop
            (2066129, Var 1033064,
             Bop
               (688709, Var 344354,
                Bop (229569, Var 114784, Bop (76523, ..., ...)))), ...))
  val x9 : X =
    Bop
      (3333333, Var 1666666,
       Bop
         (1111111,
          Bop
            (555555, Bop (277777, Var 138888, Var 92592),
             Bop (185185, Var 92592, Var 61728)), ...))
  val x10 : X =
    Bop
      (1312311237, Var 656155618,
       Bop
         (437437079,
          Bop
            (218718539,
             Bop
               (109359269, Var 54679634,
                Bop (36453089, Var 18226544, Bop (12151029, Var 6075514, ...))),
             ...), ...))
  val x11 : X =
    Bop
      (2147483647,
       Bop
         (1073741823,
          Bop
            (536870911,
             Bop
               (268435455,
                Bop
                  (134217727,
                   Bop
                     (67108863,
                      Bop
                        (33554431,
                         Bop
                           (16777215,
                            Bop (8388607, Bop (4194303, ..., ...), ...), ...),
                         ...), ...), ...), ...), ...), ...), ...)
end

> type C =
  new : x: string -> C
  member ToString : unit -> string
val c1 : C = <ToString exception: Trouble_A>
val csA : C [] =
  [|<ToString exception: Trouble_0>; <ToString exception: Trouble_1>;
    <ToString exception: Trouble_2>; <ToString exception: Trouble_3>;
    <ToString exception: Trouble_4>; <ToString exception: Trouble_5>;
    <ToString exception: Trouble_6>; <ToString exception: Trouble_7>;
    <ToString exception: Trouble_8>; <ToString exception: Trouble_9>;
    <ToString exception: Trouble_10>; <ToString exception: Trouble_11>;
    <ToString exception: Trouble_12>; <ToString exception: Trouble_13>;
    <ToString exception: Trouble_14>; <ToString exception: Trouble_15>;
    <ToString exception: Trouble_16>; <ToString exception: Trouble_17>;
    <ToString exception: Trouble_18>; <ToString exception: Trouble_19>; ...|]
val csB : C [] =
  [|<ToString exception: Trouble_0>; <ToString exception: Trouble_1>;
    <ToString exception: Trouble_2>; <ToString exception: Trouble_3>;
    <ToString exception: Trouble_4>; <ToString exception: Trouble_5>;
    <ToString exception: Trouble_6>; <ToString exception: Trouble_7>;
    <ToString exception: Trouble_8>; <ToString exception: Trouble_9>;
    <ToString exception: Trouble_10>; <ToString exception: Trouble_11>;
    <ToString exception: Trouble_12>; <ToString exception: Trouble_13>;
    <ToString exception: Trouble_14>; <ToString exception: Trouble_15>;
    <ToString exception: Trouble_16>; <ToString exception: Trouble_17>;
    <ToString exception: Trouble_18>; <ToString exception: Trouble_19>; ...|]
val csC : C [] =
  [|<ToString exception: Trouble_0>; <ToString exception: Trouble_1>;
    <ToString exception: Trouble_2>; <ToString exception: Trouble_3>;
    <ToString exception: Trouble_4>; <ToString exception: Trouble_5>;
    <ToString exception: Trouble_6>; <ToString exception: Trouble_7>;
    <ToString exception: Trouble_8>; <ToString exception: Trouble_9>;
    <ToString exception: Trouble_10>; <ToString exception: Trouble_11>;
    <ToString exception: Trouble_12>; <ToString exception: Trouble_13>;
    <ToString exception: Trouble_14>; <ToString exception: Trouble_15>;
    <ToString exception: Trouble_16>; <ToString exception: Trouble_17>;
    <ToString exception: Trouble_18>; <ToString exception: Trouble_19>; ...|]

> exception Abc

> exception AbcInt of int

> exception AbcString of string

> exception AbcExn of exn list

> exception AbcException of System.Exception list

> val exA1 : exn = Abc
val exA2 : exn = AbcInt 2
val exA3 : exn = AbcString "3"
val exA4 : exn = AbcExn [Abc; AbcInt 2; AbcString "3"]
val exA5 : exn = AbcException [AbcExn [Abc; AbcInt 2; AbcString "3"]]
exception Ex0
exception ExUnit of unit
exception ExUnits of unit * unit
exception ExUnitOption of unit option
val ex0 : exn = Ex0
val exU : exn = ExUnit ()
val exUs : exn = ExUnits ((), ())
val exUSome : exn = ExUnitOption (Some ())
val exUNone : exn = ExUnitOption None
type 'a T4063 = | AT4063 of 'a

> val valAT3063_12 : int T4063 = AT4063 12

> val valAT3063_True : bool T4063 = AT4063 true

> val valAT3063_text : string T4063 = AT4063 "text"

> val valAT3063_null : System.Object T4063 = AT4063 null

> type M4063<'a> =
  new : x: 'a -> M4063<'a>

> val v4063 : M4063<int>

> type Taaaaa<'a> =
  new : unit -> Taaaaa<'a>

> type Taaaaa2<'a> =
  inherit Taaaaa<'a>
  new : unit -> Taaaaa2<'a>
  member M : unit -> Taaaaa2<'a>

> type Tbbbbb<'a> =
  new : x: 'a -> Tbbbbb<'a>
  member M : unit -> 'a

> type Tbbbbb2 =
  inherit Tbbbbb<string>
  new : x: string -> Tbbbbb2

> val it : (unit -> string) = <fun:it@198>

> module RepeatedModule = begin
  val repeatedByteLiteral : byte [] = [|12uy; 13uy; 14uy|]
end

> module RepeatedModule = begin
  val repeatedByteLiteral : byte [] = [|12uy; 13uy; 14uy|]
end

> val it : string = "Check #help"

> 
  F# Interactive directives:

    #r "file.dll";;                               // Reference (dynamically load) the given DLL
    #i "package source uri";;                     // Include package source uri when searching for packages
    #I "path";;                                   // Add the given search path for referenced DLLs
    #load "file.fs" ...;;                         // Load the given file(s) as if compiled and referenced
    #time ["on"|"off"];;                          // Toggle timing on/off
    #help;;                                       // Display help
    #quit;;                                       // Exit

  F# Interactive command line options:



> val it : string = "Check #time on and then off"

> 
--> Timing now on

> 
--> Timing now off

> val it : string = "Check #unknown command"

> val it : string =
  "Check #I with a known directory (to avoid a warning, which includes the location of this file, which is fragile...)"

> 
--> Added '/' to library include path

> type internal T1 =
  | A
  | B

> type internal T2 =
  { x: int }

> type internal T3 =

> type internal T4 =
  new : unit -> T4

> type T1 =
  internal | A
           | B

> type T2 =
  internal { x: int }

> type private T1 =
  | A
  | B

> type private T2 =
  { x: int }

> type T1 =
  private | A
          | B

> type T2 =
  private { x: int }

> type internal T1 =
  private | A
          | B

> type internal T2 =
  private { x: int }

> type private T3 =

> type private T4 =
  new : unit -> T4

> exception X1 of int

> exception private X2 of int

> exception internal X3 of int

> type T0 =
  new : unit -> T0
type T1Post<'a> =
  new : unit -> T1Post<'a>
type 'a T1Pre =
  new : unit -> 'a T1Pre

> type T0 with
  member M : unit -> T0 list
type T0 with
  member P : T0 * T0
type T0 with
  member E : IEvent<int>

> type T1Post<'a> with
  member M : unit -> T1Post<'a> list
type T1Post<'a> with
  member P : T1Post<'a> * T1Post<'a>
type T1Post<'a> with
  member E : IEvent<obj>

> type 'a T1Pre with
  member M : unit -> 'a T1Pre list
type 'a T1Pre with
  member P : 'a T1Pre * 'a T1Pre
type 'a T1Pre with
  member E : IEvent<obj>

> type T1Post<'a> with
  member M : unit -> T1Post<'a> list
type T1Post<'a> with
  member P : T1Post<'a> * T1Post<'a>
type T1Post<'a> with
  member E : IEvent<obj>

> type 'a T1Pre with
  member M : unit -> 'a T1Pre list
type 'a T1Pre with
  member P : 'a T1Pre * 'a T1Pre
type 'a T1Pre with
  member E : IEvent<obj>

> type r =
  { f0: int
    f1: int
    f2: int
    f3: int
    f4: int
    f5: int
    f6: int
    f7: int
    f8: int
    f9: int }
val r10 : r = { f0 = 0
                f1 = 1
                f2 = 2
                f3 = 3
                f4 = 4
                f5 = 5
                f6 = 6
                f7 = 7
                f8 = 8
                f9 = 9 }
val r10s : r [] = [|{ f0 = 0
                      f1 = 1
                      f2 = 2
                      f3 = 3
                      f4 = 4
                      f5 = 5
                      f6 = 6
                      f7 = 7
                      f8 = 8
                      f9 = 9 }; ...|]
val r10s' : string * r [] = ("one extra node", [|{ f0 = 0
                                                   f1 = 1
                                                   f2 = 2
                                                   f3 = 3
                                                   f4 = 4
                                                   f5 = 5
                                                   f6 = 6
                                                   f7 = 7
                                                   f8 = 8
                                                   f9 = ... }; ...|])

> val x1564_A1 : int = 1


--> Added '\' to library include path

val x1564_A2 : int = 2


--> Added '\' to library include path

val x1564_A3 : int = 3

> type internal Foo2 =
  new : x: int * y: int * z: int -> Foo2 + 3 overloads
  member Prop1 : int with get
  member Prop2 : int with get
  member Prop3 : int with get

> module internal InternalM = begin
  val x : int = 1
  type Foo2 =
    new : x: int * y: int * z: int -> Foo2 + 3 overloads
    member Prop1 : int with get
    member Prop2 : int with get
    member Prop3 : int with get
  type private Foo3 =
    new : x: int * y: int * z: int -> Foo3 + 3 overloads
    member Prop1 : int with get
    member Prop2 : int with get
    member Prop3 : int with get
  type T1 =
    | A
    | B
  type T2 =
    { x: int }
  type T3 =
  type T4 =
    new : unit -> T4
  type T5 =
    | A
    | B
  type T6 =
    { x: int }
  type private T7 =
    | A
    | B
  type private T8 =
    { x: int }
  type T9 =
    private | A
            | B
  type T10 =
    private { x: int }
  type T11 =
    private | A
            | B
  type T12 =
    private { x: int }
  type private T13 =
  type private T14 =
    new : unit -> T14
end
module internal PrivateM = begin
  val private x : int = 1
  type private Foo2 =
    new : x: int * y: int * z: int -> Foo2 + 3 overloads
    member Prop1 : int with get
    member Prop2 : int with get
    member Prop3 : int with get
  type T1 =
    | A
    | B
  type T2 =
    { x: int }
  type T3 =
  type T4 =
    new : unit -> T4
  type T5 =
    | A
    | B
  type T6 =
    { x: int }
  type private T7 =
    | A
    | B
  type private T8 =
    { x: int }
  type T9 =
    private | A
            | B
  type T10 =
    private { x: int }
  type T11 =
    private | A
            | B
  type T12 =
    private { x: int }
  type private T13 =
  type private T14 =
    new : unit -> T14
end

> val it : seq<int * string * int> =
  seq
    [(43, "10/28/2008", 1); (46, "11/18/2008", 1); (56, "1/27/2009", 2);
     (58, "2/10/2009", 1)]

> module Test4343a = begin
  val mk : i:int -> string
  val x100 : string =
    "0123456789012345678901234567890123456789012345678901234567890"+[39 chars]
  val x90 : string =
    "0123456789012345678901234567890123456789012345678901234567890"+[29 chars]
  val x80 : string =
    "0123456789012345678901234567890123456789012345678901234567890"+[19 chars]
  val x75 : string =
    "0123456789012345678901234567890123456789012345678901234567890"+[14 chars]
  val x74 : string =
    "0123456789012345678901234567890123456789012345678901234567890"+[13 chars]
  val x73 : string =
    "0123456789012345678901234567890123456789012345678901234567890"+[12 chars]
  val x72 : string =
    "012345678901234567890123456789012345678901234567890123456789012345678901"
  val x71 : string =
    "01234567890123456789012345678901234567890123456789012345678901234567890"
  val x70 : string =
    "0123456789012345678901234567890123456789012345678901234567890123456789"
end
module Test4343b = begin
  val fA : x:int -> int
  val fB : x:'a -> y:'a -> 'a list
  val gA : (int -> int)
  val gB : ('a -> 'a -> 'a list)
  val gAB : (int -> int) * ('a -> 'a -> 'a list)
  val hB : ('a -> 'a -> 'a list)
  val hA : (int -> int)
end
module Test4343c = begin
  val typename<'a> : string
  val typename2<'a> : string * string
end
module Test4343d = begin
  val xList : int list = [1; 2; 3]
  val xArray : int [] = [|1; 2; 3|]
  val xString : string = "abcdef"
  val xOption : int option = Some 12
  val xArray2 : (int * int) [,] = [[(0, 0); (0, 1)]
                                   [(1, 0); (1, 1)]]
  val xSeq : seq<int>
end
module Test4343e = begin
  type C =
    new : x: int -> C
  val cA : C
  val cB : C
  val cAB : C * C * C list =
    (FSI_0091+Test4343e+C, FSI_0091+Test4343e+C,
     [FSI_0091+Test4343e+C; FSI_0091+Test4343e+C])
  type D =
    new : x: int -> D
    member ToString : unit -> string
  val dA : D = D(1)
  val dB : D = D(2)
  val dAB : D * D * D list = (D(1), D(2), [D(1); D(2)])
  module Generic = begin
    type CGeneric<'a> =
      new : x: 'a -> CGeneric<'a>
    val cA : C
    val cB : C
    val cAB : C * C * C list =
      (FSI_0091+Test4343e+C, FSI_0091+Test4343e+C,
       [FSI_0091+Test4343e+C; FSI_0091+Test4343e+C])
    type D<'a> =
      new : x: 'a -> D<'a>
      member ToString : unit -> string
    val dA : D<int> = D(1)
    val dB : D<int> = D(2)
    val dAB : D<int> * D<int> * D<int> list = (D(1), D(2), [D(1); D(2)])
    val dC : D<bool> = D(True)
    val boxed_dABC : obj list = [D(1); D(2); D(True)]
  end
end
type F1 =
  inherit System.Windows.Forms.Form
  interface System.IDisposable
  static val mutable private sx: F1
  static val mutable private sx2: F1
  val x: F1
  val x2: F1
  abstract member AAA : int with get
  member B : unit -> int
  abstract member BBB : bool with set
  member D : x: int -> int + 2 overloads
  member D2 : int with get, set
  member E : int with get, set
  member MMM : bool -> bool
  member ToString : unit -> string
  abstract member ZZZ : int with get
  val activeControl : System.Windows.Forms.Control
  val autoScaleBaseSize : System.Drawing.Size
  val autoScaleDimensions : System.Drawing.SizeF
  val autoScaleMode : System.Windows.Forms.AutoScaleMode
  val autoValidate : System.Windows.Forms.AutoValidate
  val autoValidateChanged : System.EventHandler
  val cachedLayoutEventArgs : System.Windows.Forms.LayoutEventArgs
  val clientHeight : int
  val clientWidth : int
  val closeReason : System.Windows.Forms.CloseReason
  val controlStyle : System.Windows.Forms.ControlStyles
  val createParams : System.Windows.Forms.CreateParams
  val ctlClient : System.Windows.Forms.MdiClient
  val currentAutoScaleDimensions : System.Drawing.SizeF
  val deviceDpi : int
  val dialogResult : System.Windows.Forms.DialogResult
  val displayRect : System.Drawing.Rectangle
  val dockPadding : System.Windows.Forms.ScrollableControl.DockPaddingEdges
  val events : System.ComponentModel.EventHandlerList
  val focusedControl : System.Windows.Forms.Control
  val formState : System.Collections.Specialized.BitVector32
  val formStateEx : System.Collections.Specialized.BitVector32
  val height : int
  val horizontalScroll : System.Windows.Forms.HScrollProperties
  val icon : System.Drawing.Icon
  val layoutSuspendCount : byte
  val minAutoSize : System.Drawing.Size
  val ownerWindow : System.Windows.Forms.NativeWindow
  val parent : System.Windows.Forms.Control
  val propertyStore : System.Windows.Forms.PropertyStore
  val reflectParent : System.Windows.Forms.Control
  val requestedScrollMargin : System.Drawing.Size
  val requiredScaling : byte
  val resetRTLHScrollValue : bool
  val restoreBounds : System.Drawing.Rectangle
  val restoredWindowBounds : System.Drawing.Rectangle
  val restoredWindowBoundsSpecified : System.Windows.Forms.BoundsSpecified
  val rightToLeftLayout : bool
  val scrollMargin : System.Drawing.Size
  val scrollPosition : System.Drawing.Point
  val scrollState : int
  val securitySite : string
  val securityZone : string
  val site : System.ComponentModel.ISite
  val sizeGripRenderer : System.Windows.Forms.VisualStyles.VisualStyleRenderer
  val smallIcon : System.Drawing.Icon
  val state : System.Collections.Specialized.BitVector32
  val state : int
  val state2 : int
  val tabIndex : int
  val text : string
  val threadCallbackList : System.Collections.Queue
  val toolStripControlHostReference : System.WeakReference<System.Windows.Forms.ToolStripControlHost>
  val trackMouseEvent : System.Windows.Forms.NativeMethods.TRACKMOUSEEVENT
  val uiCuesState : int
  val unvalidatedControl : System.Windows.Forms.Control
  val updateCount : int16
  val userAutoScrollMinSize : System.Drawing.Size
  val userWindowText : string
  val verticalScroll : System.Windows.Forms.VScrollProperties
  val width : int
  val window : System.Windows.Forms.Control.ControlNativeWindow
  val x : int
  val y : int
  static member A : unit -> int
  static val AutoScrolling : System.Diagnostics.TraceSwitch
  static val BufferPinkRect : System.Diagnostics.BooleanSwitch
  static member C : unit -> int
  static val ControlKeyboardRouting : System.Diagnostics.TraceSwitch
  static val EVENT_ACTIVATED : obj
  static val EVENT_CLOSED : obj
  static val EVENT_CLOSING : obj
  static val EVENT_DEACTIVATE : obj
  static val EVENT_DPI_CHANGED : obj
  static val EVENT_FORMCLOSED : obj
  static val EVENT_FORMCLOSING : obj
  static val EVENT_HELPBUTTONCLICKED : obj
  static val EVENT_INPUTLANGCHANGE : obj
  static val EVENT_INPUTLANGCHANGEREQUEST : obj
  static val EVENT_LOAD : obj
  static val EVENT_MAXIMIZEDBOUNDSCHANGED : obj
  static val EVENT_MAXIMUMSIZECHANGED : obj
  static val EVENT_MDI_CHILD_ACTIVATE : obj
  static val EVENT_MENUCOMPLETE : obj
  static val EVENT_MENUSTART : obj
  static val EVENT_MINIMUMSIZECHANGED : obj
  static val EVENT_RESIZEBEGIN : obj
  static val EVENT_RESIZEEND : obj
  static val EVENT_RIGHTTOLEFTLAYOUTCHANGED : obj
  static val EVENT_SCROLL : obj
  static val EVENT_SHOWN : obj
  static val EventAutoSizeChanged : obj
  static val EventBackColor : obj
  static val EventBackgroundImage : obj
  static val EventBackgroundImageLayout : obj
  static val EventBindingContext : obj
  static val EventCausesValidation : obj
  static val EventChangeUICues : obj
  static val EventClick : obj
  static val EventClientSize : obj
  static val EventContextMenu : obj
  static val EventContextMenuStrip : obj
  static val EventControlAdded : obj
  static val EventControlRemoved : obj
  static val EventCursor : obj
  static val EventDisposed : obj
  static val EventDock : obj
  static val EventDoubleClick : obj
  static val EventDpiChangedAfterParent : obj
  static val EventDpiChangedBeforeParent : obj
  static val EventDragDrop : obj
  static val EventDragEnter : obj
  static val EventDragLeave : obj
  static val EventDragOver : obj
  static val EventEnabled : obj
  static val EventEnabledChanged : obj
  static val EventEnter : obj
  static val EventFont : obj
  static val EventForeColor : obj
  static val EventGiveFeedback : obj
  static val EventGotFocus : obj
  static val EventHandleCreated : obj
  static val EventHandleDestroyed : obj
  static val EventHelpRequested : obj
  static val EventImeModeChanged : obj
  static val EventInvalidated : obj
  static val EventKeyDown : obj
  static val EventKeyPress : obj
  static val EventKeyUp : obj
  static val EventLayout : obj
  static val EventLeave : obj
  static val EventLocation : obj
  static val EventLostFocus : obj
  static val EventMarginChanged : obj
  static val EventMouseCaptureChanged : obj
  static val EventMouseClick : obj
  static val EventMouseDoubleClick : obj
  static val EventMouseDown : obj
  static val EventMouseEnter : obj
  static val EventMouseHover : obj
  static val EventMouseLeave : obj
  static val EventMouseMove : obj
  static val EventMouseUp : obj
  static val EventMouseWheel : obj
  static val EventMove : obj
  static val EventPaddingChanged : obj
  static val EventPaint : obj
  static val EventParent : obj
  static val EventPreviewKeyDown : obj
  static val EventQueryAccessibilityHelp : obj
  static val EventQueryContinueDrag : obj
  static val EventRegionChanged : obj
  static val EventResize : obj
  static val EventRightToLeft : obj
  static val EventSize : obj
  static val EventStyleChanged : obj
  static val EventSystemColorsChanged : obj
  static val EventTabIndex : obj
  static val EventTabStop : obj
  static val EventText : obj
  static val EventValidated : obj
  static val EventValidating : obj
  static val EventVisible : obj
  static val EventVisibleChanged : obj
  static val FocusTracing : System.Diagnostics.TraceSwitch
  static val FormStateAllowTransparency : System.Collections.Specialized.BitVector32.Section
  static val FormStateAutoScaling : System.Collections.Specialized.BitVector32.Section
  static val FormStateBorderStyle : System.Collections.Specialized.BitVector32.Section
  static val FormStateControlBox : System.Collections.Specialized.BitVector32.Section
  static val FormStateExAutoSize : System.Collections.Specialized.BitVector32.Section
  static val FormStateExCalledClosing : System.Collections.Specialized.BitVector32.Section
  static val FormStateExCalledCreateControl : System.Collections.Specialized.BitVector32.Section
  static val FormStateExCalledMakeVisible : System.Collections.Specialized.BitVector32.Section
  static val FormStateExCalledOnLoad : System.Collections.Specialized.BitVector32.Section
  static val FormStateExInModalSizingLoop : System.Collections.Specialized.BitVector32.Section
  static val FormStateExInScale : System.Collections.Specialized.BitVector32.Section
  static val FormStateExInUpdateMdiControlStrip : System.Collections.Specialized.BitVector32.Section
  static val FormStateExMnemonicProcessed : System.Collections.Specialized.BitVector32.Section
  static val FormStateExSettingAutoScale : System.Collections.Specialized.BitVector32.Section
  static val FormStateExShowIcon : System.Collections.Specialized.BitVector32.Section
  static val FormStateExUpdateMenuHandlesDeferred : System.Collections.Specialized.BitVector32.Section
  static val FormStateExUpdateMenuHandlesSuspendCount : System.Collections.Specialized.BitVector32.Section
  static val FormStateExUseMdiChildProc : System.Collections.Specialized.BitVector32.Section
  static val FormStateExWindowBoundsHeightIsClientSize : System.Collections.Specialized.BitVector32.Section
  static val FormStateExWindowBoundsWidthIsClientSize : System.Collections.Specialized.BitVector32.Section
  static val FormStateExWindowClosing : System.Collections.Specialized.BitVector32.Section
  static val FormStateHelpButton : System.Collections.Specialized.BitVector32.Section
  static val FormStateIconSet : System.Collections.Specialized.BitVector32.Section
  static val FormStateIsActive : System.Collections.Specialized.BitVector32.Section
  static val FormStateIsRestrictedWindow : System.Collections.Specialized.BitVector32.Section
  static val FormStateIsRestrictedWindowChecked : System.Collections.Specialized.BitVector32.Section
  static val FormStateIsTextEmpty : System.Collections.Specialized.BitVector32.Section
  static val FormStateIsWindowActivated : System.Collections.Specialized.BitVector32.Section
  static val FormStateKeyPreview : System.Collections.Specialized.BitVector32.Section
  static val FormStateLayered : System.Collections.Specialized.BitVector32.Section
  static val FormStateMaximizeBox : System.Collections.Specialized.BitVector32.Section
  static val FormStateMdiChildMax : System.Collections.Specialized.BitVector32.Section
  static val FormStateMinimizeBox : System.Collections.Specialized.BitVector32.Section
  static val FormStateRenderSizeGrip : System.Collections.Specialized.BitVector32.Section
  static val FormStateSWCalled : System.Collections.Specialized.BitVector32.Section
  static val FormStateSetClientSize : System.Collections.Specialized.BitVector32.Section
  static val FormStateShowWindowOnCreate : System.Collections.Specialized.BitVector32.Section
  static val FormStateSizeGripStyle : System.Collections.Specialized.BitVector32.Section
  static val FormStateStartPos : System.Collections.Specialized.BitVector32.Section
  static val FormStateTaskBar : System.Collections.Specialized.BitVector32.Section
  static val FormStateTopMost : System.Collections.Specialized.BitVector32.Section
  static val FormStateWindowState : System.Collections.Specialized.BitVector32.Section
  static val HighOrderBitMask : byte
  static val ImeCharsToIgnoreDisabled : int
  static val ImeCharsToIgnoreEnabled : int
  static val PaintLayerBackground : int16
  static val PaintLayerForeground : int16
  static val PaletteTracing : System.Diagnostics.TraceSwitch
  static val PropAcceptButton : int
  static val PropAccessibility : int
  static val PropAccessibleDefaultActionDescription : int
  static val PropAccessibleDescription : int
  static val PropAccessibleHelpProvider : int
  static val PropAccessibleName : int
  static val PropAccessibleRole : int
  static val PropActiveMdiChild : int
  static val PropActiveXImpl : int
  static val PropAmbientPropertiesService : int
  static val PropAutoScrollOffset : int
  static val PropAxContainer : int
  static val PropBackBrush : int
  static val PropBackColor : int
  static val PropBackgroundImage : int
  static val PropBackgroundImageLayout : int
  static val PropBindingManager : int
  static val PropBindings : int
  static val PropCacheTextCount : int
  static val PropCacheTextField : int
  static val PropCancelButton : int
  static val PropContextMenu : int
  static val PropContextMenuStrip : int
  static val PropControlVersionInfo : int
  static val PropControlsCollection : int
  static val PropCurMenu : int
  static val PropCurrentAmbientFont : int
  static val PropCursor : int
  static val PropDefaultButton : int
  static val PropDialogOwner : int
  static val PropDisableImeModeChangedCount : int
  static val PropDummyMenu : int
  static val PropFont : int
  static val PropFontHandleWrapper : int
  static val PropFontHeight : int
  static val PropForeColor : int
  static val PropFormMdiParent : int
  static val PropFormerlyActiveMdiChild : int
  static val PropImeMode : int
  static val PropImeWmCharsToIgnore : int
  static val PropLastCanEnableIme : int
  static val PropMainMenu : int
  static val PropMainMenuStrip : int
  static val PropMaxTrackSizeHeight : int
  static val PropMaxTrackSizeWidth : int
  static val PropMaximizedBounds : int
  static val PropMdiChildFocusable : int
  static val PropMdiControlStrip : int
  static val PropMdiWindowListStrip : int
  static val PropMergedMenu : int
  static val PropMinTrackSizeHeight : int
  static val PropMinTrackSizeWidth : int
  static val PropName : int
  static val PropNcAccessibility : int
  static val PropOpacity : int
  static val PropOwnedForms : int
  static val PropOwnedFormsCount : int
  static val PropOwner : int
  static val PropPaintingException : int
  static val PropRegion : int
  static val PropRightToLeft : int
  static val PropSecurityTip : int
  static val PropTransparencyKey : int
  static val PropUseCompatibleTextRendering : int
  static val PropUserData : int
  static val RequiredScalingEnabledMask : byte
  static val RequiredScalingMask : byte
  static val STATE2_BECOMINGACTIVECONTROL : int
  static val STATE2_CLEARLAYOUTARGS : int
  static val STATE2_CURRENTLYBEINGSCALED : int
  static val STATE2_HAVEINVOKED : int
  static val STATE2_INPUTCHAR : int
  static val STATE2_INPUTKEY : int
  static val STATE2_INTERESTEDINUSERPREFERENCECHANGED : int
  static val STATE2_ISACTIVEX : int
  static val STATE2_LISTENINGTOUSERPREFERENCECHANGED : int
  static val STATE2_MAINTAINSOWNCAPTUREMODE : int
  static val STATE2_SETSCROLLPOS : int
  static val STATE2_TOPMDIWINDOWCLOSING : int
  static val STATE2_UICUES : int
  static val STATE2_USEPREFERREDSIZECACHE : int
  static val STATE_ALLOWDROP : int
  static val STATE_CAUSESVALIDATION : int
  static val STATE_CHECKEDHOST : int
  static val STATE_CREATED : int
  static val STATE_CREATINGHANDLE : int
  static val STATE_DISPOSED : int
  static val STATE_DISPOSING : int
  static val STATE_DOUBLECLICKFIRED : int
  static val STATE_DROPTARGET : int
  static val STATE_ENABLED : int
  static val STATE_EXCEPTIONWHILEPAINTING : int
  static val STATE_HOSTEDINDIALOG : int
  static val STATE_ISACCESSIBLE : int
  static val STATE_LAYOUTDEFERRED : int
  static val STATE_LAYOUTISDIRTY : int
  static val STATE_MIRRORED : int
  static val STATE_MODAL : int
  static val STATE_MOUSEENTERPENDING : int
  static val STATE_MOUSEPRESSED : int
  static val STATE_NOZORDER : int
  static val STATE_OWNCTLBRUSH : int
  static val STATE_PARENTRECREATING : int
  static val STATE_RECREATE : int
  static val STATE_SIZELOCKEDBYOS : int
  static val STATE_TABSTOP : int
  static val STATE_THREADMARSHALLPENDING : int
  static val STATE_TOPLEVEL : int
  static val STATE_TRACKINGMOUSEEVENT : int
  static val STATE_USEWAITCURSOR : int
  static val STATE_VALIDATIONCANCELLED : int
  static val STATE_VISIBLE : int
  static val ScrollStateAutoScrolling : int
  static val ScrollStateFullDrag : int
  static val ScrollStateHScrollVisible : int
  static val ScrollStateUserHasScrolled : int
  static val ScrollStateVScrollVisible : int
  static val SizeGripSize : int
  static val UISTATE_FOCUS_CUES_HIDDEN : int
  static val UISTATE_FOCUS_CUES_MASK : int
  static val UISTATE_FOCUS_CUES_SHOW : int
  static val UISTATE_KEYBOARD_CUES_HIDDEN : int
  static val UISTATE_KEYBOARD_CUES_MASK : int
  static val UISTATE_KEYBOARD_CUES_SHOW : int
  static val UseCompatibleTextRenderingDefault : bool
  static val WM_GETCONTROLNAME : int
  static val WM_GETCONTROLTYPE : int
  static val checkForIllegalCrossThreadCalls : bool
  static val currentHelpInfo : System.Windows.Forms.HelpInfo
  static val defaultFont : System.Drawing.Font
  static val defaultFontHandleWrapper : System.Windows.Forms.Control.FontHandleWrapper
  static val defaultIcon : System.Drawing.Icon
  static val defaultRestrictedIcon : System.Drawing.Icon
  static val fontMeasureString : string
  static val ignoreWmImeNotify : bool
  static val inCrossThreadSafeCall : bool
  static val internalSyncObject : obj
  static val invokeMarshaledCallbackHelperDelegate : System.Threading.ContextCallback
  static val lastLanguageChinese : bool
  static val mouseWheelInit : bool
  static val mouseWheelMessage : int
  static val mouseWheelRoutingNeeded : bool
  static val needToLoadComCtl : bool
  static val propagatingImeMode : System.Windows.Forms.ImeMode
  static val stateParentChanged : int
  static val stateProcessingMnemonic : int
  static val stateScalingChild : int
  static val stateScalingNeededOnLayout : int
  static val stateValidating : int
  static val tempKeyboardStateArray : byte []
  static val threadCallbackMessage : int
type IP =
  new : x: int * y: int -> IP
  static val mutable private AA: IP
module Regression4643 = begin
  type RIP =
    new : x: int -> RIP
    static val mutable private y: RIP
  type arg_unused_is_RIP =
    new : x: RIP -> arg_unused_is_RIP
  type arg_used_is_RIP =
    new : x: RIP -> arg_used_is_RIP
    member X : RIP with get
  type field_is_RIP =
    val x: RIP
end
type Either<'a,'b> =
  | This of 'a
  | That of 'b
val catch : f:(unit -> 'a) -> Either<'a,(string * string)>
val seqFindIndexFailure : Either<int,(string * string)> =
  That
    ("System.Collections.Generic.KeyNotFoundException",
     "An index satisfying the predicate was not found in the collection.")
val seqFindFailure : Either<int,(string * string)> =
  That
    ("System.Collections.Generic.KeyNotFoundException",
     "An index satisfying the predicate was not found in the collection.")
val seqPickFailure : Either<int,(string * string)> =
  That
    ("System.Collections.Generic.KeyNotFoundException",
     "An index satisfying the predicate was not found in the collection.")
module Regression5218 = begin
  val t1 : int = 1
  val t2 : int * int = (1, 2)
  val t3 : int * int * int = (1, 2, 3)
  val t4 : int * int * int * int = (1, 2, 3, 4)
  val t5 : int * int * int * int * int = (1, 2, 3, 4, 5)
  val t6 : int * int * int * int * int * int = (1, 2, 3, 4, 5, 6)
  val t7 : int * int * int * int * int * int * int = (1, 2, 3, 4, 5, 6, 7)
  val t8 : int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8)
  val t9 : int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9)
  val t10 : int * int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
  val t11 : int * int * int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)
  val t12 :
    int * int * int * int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)
  val t13 :
    int * int * int * int * int * int * int * int * int * int * int * int *
    int = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)
  val t14 :
    int * int * int * int * int * int * int * int * int * int * int * int *
    int * int = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)
  val t15 :
    int * int * int * int * int * int * int * int * int * int * int * int *
    int * int * int = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)
end

> module Regression3739 = begin
  type IB =
    member AbstractMember : int -> int
  type C<'a when 'a :> IB> =
    new : unit -> C<'a>
    static member StaticMember : x: 'a -> int
end

> module Regression3739 = begin
  type IB =
    member AbstractMember : int -> int
  type C<'a when 'a :> IB> =
    new : unit -> C<'a>
    static member StaticMember : x: 'a -> int
end

> module Regression3740 = begin
  type Writer<'a> =
    member get_path : unit -> string
  type MyClass =
    interface Writer<int>
    val path: string
end

> type Regression4319_T2 =
  static member op_PlusMinusPlusMinusPlus<'a,'b> : x: 'a * y: 'b -> string

> type Regression4319_T0 =
  static member op_PlusMinusPlusMinusPlus : string with get

> type Regression4319_T1 =
  static member op_PlusMinusPlusMinusPlus<'a> : x: 'a -> string

> type Regression4319_T1b =
  static member op_PlusMinusPlusMinusPlus<'a> : x: 'a -> string

> type Regression4319_T1c =
  static member op_PlusMinusPlusMinusPlus<'a,'b> : x: 'a * 'b -> string

> type Regression4319_T1d =
  static member op_PlusMinusPlusMinusPlus : x: int * int -> string

> type Regression4319_T3 =
  static member op_PlusMinusPlusMinusPlus<'a,'b,'c> : x: 'a * y: 'b * z: 'c -> string

> type Regression4319_U1 =
  static member op_PlusMinusPlusMinusPlus<'a,'b> : x: 'a -> moreArgs: 'b -> string

> type Regression4319_U1b =
  static member op_PlusMinusPlusMinusPlus<'a,'b> : x: 'a -> moreArgs: 'b -> string

> type Regression4319_U2 =
  static member op_PlusMinusPlusMinusPlus<'a,'b,'c> : x: 'a * y: 'b ->
                                                      moreArgs: 'c -> string

> type Regression4319_U3 =
  static member op_PlusMinusPlusMinusPlus<'a,'b,'c,'d> : x: 'a * y: 'b * z: 'c ->
                                                         moreArgs: 'd -> string

> type Regression4319_check =
  static member op_Amp : string with get
  static member op_AmpHat : string with get
  static member op_Append : string with get
  static member op_BangEquals : string with get
  static member op_ColonEquals : string with get
  static member op_Concatenate : string with get
  static member op_Division : string with get
  static member op_Dollar : string with get
  static member op_DotDotDotAt : string with get
  static member op_DotDotDotBangEquals : string with get
  static member op_DotDotDotDivide : string with get
  static member op_DotDotDotEquals : string with get
  static member op_DotDotDotGreater : string with get
  static member op_DotDotDotHat : string with get
  static member op_DotDotDotLess : string with get
  static member op_DotDotDotMultiply : string with get
  static member op_DotDotDotPercent : string with get
  static member op_Equality : string with get
  static member op_Exponentiation : string with get
  static member op_GreaterThan : string with get
  static member op_LessThan : string with get
  static member op_Modulus : string with get
  static member op_Multiply : string with get
  static member op_Subtraction : string with get

> Expect ABC = ABC
type Regression4469 =
  new : unit -> Regression4469
  member ToString : unit -> string
val r4469 : Regression4469 = FSI_0107+Regression4469
val it : unit = ()

> Expect ABC = ABC
val it : unit = ()

> module Regression1019_short = begin
  val double_nan : float = nan
  val double_infinity : float = infinity
  val single_nan : float32 = nanf
  val single_infinity : float32 = infinityf
end
module Regression1019_long = begin
  val double_nan : float = nan
  val double_infinity : float = infinity
  val single_nan : float32 = nanf
  val single_infinity : float32 = infinityf
end

> val it : int ref = { contents = 1 }

> val x : int ref = { contents = 1 }
val f : (unit -> int)

> val it : int = 1

> val it : unit = ()

> val it : int = 3

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : int [] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it : 'a list

> val it : 'a list list

> val it : 'a option

> val it : 'a list * 'b list

> val it : x:'a -> 'a

> val fff : x:'a -> 'a

> val it : ('a -> 'a)

> val note_ExpectDupMethod : string =
  "Regression4927: Expect error due to duplicate methods in the "+[20 chars]

> > val note_ExpectDupProperty : string =
  "Regression4927: Expect error due to duplicate properties in t"+[23 chars]

> > > val it : string = "NOTE: Expect IAPrivate less accessible IBPublic"

> > val it : string = "NOTE: Expect IAPrivate less accessible IBInternal"

> > module Regression5265_PriPri = begin
  type private IAPrivate =
    abstract member P : int with get
  type private IBPrivate =
    inherit IAPrivate
    abstract member Q : int with get
end

> val it : string = "NOTE: Expect IAInternal less accessible IBPublic"

> > module Regression5265_IntInt = begin
  type internal IAInternal =
    abstract member P : int with get
  type internal IBInternal =
    inherit IAInternal
    abstract member Q : int with get
end

> module Regression5265_IntPri = begin
  type internal IAInternal =
    abstract member P : int with get
  type private IBPrivate =
    inherit IAInternal
    abstract member Q : int with get
end

> module Regression5265_PubPub = begin
  type IAPublic =
    abstract member P : int with get
  type IBPublic =
    inherit IAPublic
    abstract member Q : int with get
end

> module Regression5265_PubInt = begin
  type IAPublic =
    abstract member P : int with get
  type internal IBInternal =
    inherit IAPublic
    abstract member Q : int with get
end

> module Regression5265_PubPri = begin
  type IAPublic =
    abstract member P : int with get
  type private IBPrivate =
    inherit IAPublic
    abstract member Q : int with get
end

> val it : string =
  "Regression4232: Expect an error about duplicate virtual methods from parent type"

> > val it : string =
  "** Expect AnAxHostSubClass to be accepted. AxHost has a newslot virtual RightToLeft property outscope RightToLeft on Control"

> type AnAxHostSubClass =
  inherit System.Windows.Forms.AxHost
  new : x: string -> AnAxHostSubClass
  val REGMSG_MSG : int
  val aboutBoxDelegate : System.Windows.Forms.AxHost.AboutBoxDelegate
  val attribsStash : System.Attribute []
  val axContainer : System.Windows.Forms.AxHost.AxContainer
  val axState : System.Collections.Specialized.BitVector32
  val cachedLayoutEventArgs : System.Windows.Forms.LayoutEventArgs
  val clientHeight : int
  val clientWidth : int
  val clsid : System.Guid
  val container : System.Windows.Forms.AxHost.AxContainer
  val containingControl : System.Windows.Forms.ContainerControl
  val controlStyle : System.Windows.Forms.ControlStyles
  val createParams : System.Windows.Forms.CreateParams
  val deviceDpi : int
  val editMode : int
  val editor : System.Windows.Forms.AxHost.AxComponentEditor
  val events : System.ComponentModel.EventHandlerList
  val flags : int
  val freezeCount : int
  val height : int
  val hwndFocus : nativeint
  val iCategorizeProperties : System.Windows.Forms.NativeMethods.ICategorizeProperties
  val iOleControl : System.Windows.Forms.UnsafeNativeMethods.IOleControl
  val iOleInPlaceActiveObject : System.Windows.Forms.UnsafeNativeMethods.IOleInPlaceActiveObject
  val iOleInPlaceActiveObjectExternal : System.Windows.Forms.UnsafeNativeMethods.IOleInPlaceActiveObject
  val iOleInPlaceObject : System.Windows.Forms.UnsafeNativeMethods.IOleInPlaceObject
  val iOleObject : System.Windows.Forms.UnsafeNativeMethods.IOleObject
  val iPerPropertyBrowsing : System.Windows.Forms.NativeMethods.IPerPropertyBrowsing
  val iPersistPropBag : System.Windows.Forms.UnsafeNativeMethods.IPersistPropertyBag
  val iPersistStorage : System.Windows.Forms.UnsafeNativeMethods.IPersistStorage
  val iPersistStream : System.Windows.Forms.UnsafeNativeMethods.IPersistStream
  val iPersistStreamInit : System.Windows.Forms.UnsafeNativeMethods.IPersistStreamInit
  val ignoreDialogKeys : bool
  val instance : obj
  val isMaskEdit : bool
  val layoutSuspendCount : byte
  val licenseKey : string
  val miscStatusBits : int
  val newParent : System.Windows.Forms.ContainerControl
  val noComponentChange : int
  val objectDefinedCategoryNames : System.Collections.Hashtable
  val ocState : int
  val ocxState : System.Windows.Forms.AxHost.State
  val oleSite : System.Windows.Forms.AxHost.OleInterfaces
  val onContainerVisibleChanged : System.EventHandler
  val parent : System.Windows.Forms.Control
  val properties : System.Collections.Hashtable
  val propertyInfos : System.Collections.Hashtable
  val propertyStore : System.Windows.Forms.PropertyStore
  val propsStash : System.ComponentModel.PropertyDescriptorCollection
  val reflectParent : System.Windows.Forms.Control
  val requiredScaling : byte
  val selectionChangeHandler : System.EventHandler
  val selectionStyle : int
  val site : System.ComponentModel.ISite
  val state : int
  val state2 : int
  val storageType : int
  val tabIndex : int
  val text : string
  val text : string
  val threadCallbackList : System.Collections.Queue
  val toolStripControlHostReference : System.WeakReference<System.Windows.Forms.ToolStripControlHost>
  val trackMouseEvent : System.Windows.Forms.NativeMethods.TRACKMOUSEEVENT
  val uiCuesState : int
  val updateCount : int16
  val width : int
  val window : System.Windows.Forms.Control.ControlNativeWindow
  val wndprocAddr : nativeint
  val x : int
  val y : int
  static val AxAlwaysSaveSwitch : System.Diagnostics.BooleanSwitch
  static val AxHTraceSwitch : System.Diagnostics.TraceSwitch
  static val AxHostSwitch : System.Diagnostics.TraceSwitch
  static val AxIgnoreTMSwitch : System.Diagnostics.BooleanSwitch
  static val AxPropTraceSwitch : System.Diagnostics.TraceSwitch
  static val BufferPinkRect : System.Diagnostics.BooleanSwitch
  static val ControlKeyboardRouting : System.Diagnostics.TraceSwitch
  static val EDITM_HOST : int
  static val EDITM_NONE : int
  static val EDITM_OBJECT : int
  static val E_FAIL : System.Runtime.InteropServices.COMException
  static val E_INVALIDARG : System.Runtime.InteropServices.COMException
  static val E_NOINTERFACE : System.Runtime.InteropServices.COMException
  static val E_NOTIMPL : System.Runtime.InteropServices.COMException
  static val EventAutoSizeChanged : obj
  static val EventBackColor : obj
  static val EventBackgroundImage : obj
  static val EventBackgroundImageLayout : obj
  static val EventBindingContext : obj
  static val EventCausesValidation : obj
  static val EventChangeUICues : obj
  static val EventClick : obj
  static val EventClientSize : obj
  static val EventContextMenu : obj
  static val EventContextMenuStrip : obj
  static val EventControlAdded : obj
  static val EventControlRemoved : obj
  static val EventCursor : obj
  static val EventDisposed : obj
  static val EventDock : obj
  static val EventDoubleClick : obj
  static val EventDpiChangedAfterParent : obj
  static val EventDpiChangedBeforeParent : obj
  static val EventDragDrop : obj
  static val EventDragEnter : obj
  static val EventDragLeave : obj
  static val EventDragOver : obj
  static val EventEnabled : obj
  static val EventEnabledChanged : obj
  static val EventEnter : obj
  static val EventFont : obj
  static val EventForeColor : obj
  static val EventGiveFeedback : obj
  static val EventGotFocus : obj
  static val EventHandleCreated : obj
  static val EventHandleDestroyed : obj
  static val EventHelpRequested : obj
  static val EventImeModeChanged : obj
  static val EventInvalidated : obj
  static val EventKeyDown : obj
  static val EventKeyPress : obj
  static val EventKeyUp : obj
  static val EventLayout : obj
  static val EventLeave : obj
  static val EventLocation : obj
  static val EventLostFocus : obj
  static val EventMarginChanged : obj
  static val EventMouseCaptureChanged : obj
  static val EventMouseClick : obj
  static val EventMouseDoubleClick : obj
  static val EventMouseDown : obj
  static val EventMouseEnter : obj
  static val EventMouseHover : obj
  static val EventMouseLeave : obj
  static val EventMouseMove : obj
  static val EventMouseUp : obj
  static val EventMouseWheel : obj
  static val EventMove : obj
  static val EventPaddingChanged : obj
  static val EventPaint : obj
  static val EventParent : obj
  static val EventPreviewKeyDown : obj
  static val EventQueryAccessibilityHelp : obj
  static val EventQueryContinueDrag : obj
  static val EventRegionChanged : obj
  static val EventResize : obj
  static val EventRightToLeft : obj
  static val EventSize : obj
  static val EventStyleChanged : obj
  static val EventSystemColorsChanged : obj
  static val EventTabIndex : obj
  static val EventTabStop : obj
  static val EventText : obj
  static val EventValidated : obj
  static val EventValidating : obj
  static val EventVisible : obj
  static val EventVisibleChanged : obj
  static val FocusTracing : System.Diagnostics.TraceSwitch
  static val HMperInch : int
  static val HighOrderBitMask : byte
  static val INPROC_SERVER : int
  static val ImeCharsToIgnoreDisabled : int
  static val ImeCharsToIgnoreEnabled : int
  static val OC_INPLACE : int
  static val OC_LOADED : int
  static val OC_OPEN : int
  static val OC_PASSIVE : int
  static val OC_RUNNING : int
  static val OC_UIACTIVE : int
  static val OLEIVERB_HIDE : int
  static val OLEIVERB_INPLACEACTIVATE : int
  static val OLEIVERB_PRIMARY : int
  static val OLEIVERB_PROPERTIES : int
  static val OLEIVERB_SHOW : int
  static val OLEIVERB_UIACTIVATE : int
  static val PaintLayerBackground : int16
  static val PaintLayerForeground : int16
  static val PaletteTracing : System.Diagnostics.TraceSwitch
  static val PropAccessibility : int
  static val PropAccessibleDefaultActionDescription : int
  static val PropAccessibleDescription : int
  static val PropAccessibleHelpProvider : int
  static val PropAccessibleName : int
  static val PropAccessibleRole : int
  static val PropActiveXImpl : int
  static val PropAmbientPropertiesService : int
  static val PropAutoScrollOffset : int
  static val PropBackBrush : int
  static val PropBackColor : int
  static val PropBackgroundImage : int
  static val PropBackgroundImageLayout : int
  static val PropBindingManager : int
  static val PropBindings : int
  static val PropCacheTextCount : int
  static val PropCacheTextField : int
  static val PropContextMenu : int
  static val PropContextMenuStrip : int
  static val PropControlVersionInfo : int
  static val PropControlsCollection : int
  static val PropCurrentAmbientFont : int
  static val PropCursor : int
  static val PropDisableImeModeChangedCount : int
  static val PropFont : int
  static val PropFontHandleWrapper : int
  static val PropFontHeight : int
  static val PropForeColor : int
  static val PropImeMode : int
  static val PropImeWmCharsToIgnore : int
  static val PropLastCanEnableIme : int
  static val PropName : int
  static val PropNcAccessibility : int
  static val PropPaintingException : int
  static val PropRegion : int
  static val PropRightToLeft : int
  static val PropUseCompatibleTextRendering : int
  static val PropUserData : int
  static val REGMSG_RETVAL : int
  static val RequiredScalingEnabledMask : byte
  static val RequiredScalingMask : byte
  static val STATE2_BECOMINGACTIVECONTROL : int
  static val STATE2_CLEARLAYOUTARGS : int
  static val STATE2_CURRENTLYBEINGSCALED : int
  static val STATE2_HAVEINVOKED : int
  static val STATE2_INPUTCHAR : int
  static val STATE2_INPUTKEY : int
  static val STATE2_INTERESTEDINUSERPREFERENCECHANGED : int
  static val STATE2_ISACTIVEX : int
  static val STATE2_LISTENINGTOUSERPREFERENCECHANGED : int
  static val STATE2_MAINTAINSOWNCAPTUREMODE : int
  static val STATE2_SETSCROLLPOS : int
  static val STATE2_TOPMDIWINDOWCLOSING : int
  static val STATE2_UICUES : int
  static val STATE2_USEPREFERREDSIZECACHE : int
  static val STATE_ALLOWDROP : int
  static val STATE_CAUSESVALIDATION : int
  static val STATE_CHECKEDHOST : int
  static val STATE_CREATED : int
  static val STATE_CREATINGHANDLE : int
  static val STATE_DISPOSED : int
  static val STATE_DISPOSING : int
  static val STATE_DOUBLECLICKFIRED : int
  static val STATE_DROPTARGET : int
  static val STATE_ENABLED : int
  static val STATE_EXCEPTIONWHILEPAINTING : int
  static val STATE_HOSTEDINDIALOG : int
  static val STATE_ISACCESSIBLE : int
  static val STATE_LAYOUTDEFERRED : int
  static val STATE_LAYOUTISDIRTY : int
  static val STATE_MIRRORED : int
  static val STATE_MODAL : int
  static val STATE_MOUSEENTERPENDING : int
  static val STATE_MOUSEPRESSED : int
  static val STATE_NOZORDER : int
  static val STATE_OWNCTLBRUSH : int
  static val STATE_PARENTRECREATING : int
  static val STATE_RECREATE : int
  static val STATE_SIZELOCKEDBYOS : int
  static val STATE_TABSTOP : int
  static val STATE_THREADMARSHALLPENDING : int
  static val STATE_TOPLEVEL : int
  static val STATE_TRACKINGMOUSEEVENT : int
  static val STATE_USEWAITCURSOR : int
  static val STATE_VALIDATIONCANCELLED : int
  static val STATE_VISIBLE : int
  static val STG_STORAGE : int
  static val STG_STREAM : int
  static val STG_STREAMINIT : int
  static val STG_UNKNOWN : int
  static val UISTATE_FOCUS_CUES_HIDDEN : int
  static val UISTATE_FOCUS_CUES_MASK : int
  static val UISTATE_FOCUS_CUES_SHOW : int
  static val UISTATE_KEYBOARD_CUES_HIDDEN : int
  static val UISTATE_KEYBOARD_CUES_MASK : int
  static val UISTATE_KEYBOARD_CUES_SHOW : int
  static val UseCompatibleTextRenderingDefault : bool
  static val WM_GETCONTROLNAME : int
  static val WM_GETCONTROLTYPE : int
  static val addedSelectionHandler : int
  static val assignUniqueID : int
  static val categoryNames : System.ComponentModel.CategoryAttribute []
  static val checkForIllegalCrossThreadCalls : bool
  static val checkedCP : int
  static val checkedIppb : int
  static val comctlImageCombo_Clsid : System.Guid
  static val currentHelpInfo : System.Windows.Forms.HelpInfo
  static val dataSource_Guid : System.Guid
  static val defaultFont : System.Drawing.Font
  static val defaultFontHandleWrapper : System.Windows.Forms.Control.FontHandleWrapper
  static val disposed : int
  static val editorRefresh : int
  static val fFakingWindow : int
  static val fNeedOwnWindow : int
  static val fOwnWindow : int
  static val fSimpleFrame : int
  static val fontTable : System.Collections.Hashtable
  static val handlePosRectChanged : int
  static val icf2_Guid : System.Guid
  static val ifontDisp_Guid : System.Guid
  static val ifont_Guid : System.Guid
  static val ignoreWmImeNotify : bool
  static val inCrossThreadSafeCall : bool
  static val inTransition : int
  static val invokeMarshaledCallbackHelperDelegate : System.Threading.ContextCallback
  static val ioleobject_Guid : System.Guid
  static val ipictureDisp_Guid : System.Guid
  static val ipicture_Guid : System.Guid
  static val ivbformat_Guid : System.Guid
  static val lastLanguageChinese : bool
  static val listeningToIdle : int
  static val logPixelsX : int
  static val logPixelsY : int
  static val manualUpdate : int
  static val maskEdit_Clsid : System.Guid
  static val mouseWheelInit : bool
  static val mouseWheelMessage : int
  static val mouseWheelRoutingNeeded : bool
  static val needLicenseKey : int
  static val needToLoadComCtl : bool
  static val ocxStateSet : int
  static val ownDisposing : int
  static val processingKeyUp : int
  static val propagatingImeMode : System.Windows.Forms.ImeMode
  static val refreshProperties : int
  static val rejectSelection : int
  static val renameEventHooked : int
  static val sinkAttached : int
  static val siteProcessedInputKey : int
  static val tempKeyboardStateArray : byte []
  static val threadCallbackMessage : int
  static val valueChanged : int
  static val windowsMediaPlayer_Clsid : System.Guid

> val it : string =
  "** Expect error because the active pattern result contains free type variables"

> > val it : string =
  "** Expect error because the active pattern result contains free type variables (match value generic)"

> > val it : string =
  "** Expect error because the active pattern result contains free type variables (when active pattern also has parameters)"

> > val it : string =
  "** Expect OK, since error message says constraint should work!"

> val ( |A|B| ) : x:int -> Choice<int,unit>

> val it : string = "** Expect error since active pattern is not a function!"

> > val it : string =
  "** Expect OK since active pattern result is not too generic, typars depend on match val"

> val ( |A|B| ) : p:bool -> 'a * 'b -> Choice<'a,'b>

> val it : string =
  "** Expect OK since active pattern result is not too generic, typars depend on parameters"

> val ( |A|B| ) : aval:'a -> bval:'b -> x:bool -> Choice<'a,'b>

> val it : string =
  "** Expect OK since active pattern result is generic, but it typar from closure, so OK"

> val outer : x:'a -> (int -> 'a option)

> val it : string =
  "** Expect OK, BUG 472278: revert unintended breaking change to Active Patterns in F# 3.0"

> val ( |Check1| ) : a:int -> int * 'a option

> > module ReflectionEmit = begin
  type IA =
    member M<'a when 'a :> IB> : 'a -> int
  and IB =
    member M<'b when 'b :> IA> : 'b -> int
  type IA2<'a when 'a :> IB2<'a> and 'a :> IA2<'a>> =
    abstract member M : int with get
  and IB2<'b when 'b :> IA2<'b> and 'b :> IB2<'b>> =
    abstract member M : int with get
end

> val it : string =
  "Regression_139182: Expect the follow code to be accepted without error"

> type S =
  member TheMethod : unit -> int64
val theMethod : s:S -> int64
type T =
  new : unit -> T
  member Prop5 : int64 with get
  static member .cctor : unit -> unit
  static member Prop1 : int64 with get
  static member Prop2 : int64 with get
  static member Prop3 : int64 with get
  static member Prop4 : string with get

> val it : System.Threading.ThreadLocal<int> list = [0 {IsValueCreated = false;
                                                      Values = ?;}]

> type MyDU =
  | Case1 of Val1: int * Val2: string
  | Case2 of string * V2: bool * float
  | Case3 of int
  | Case4 of Item1: bool
  | Case5 of bool * string
  | Case6 of Val1: int * bool * string
  | Case7 of Big Name: int
val namedFieldVar1 : MyDU = Case1 (5, "")
val namedFieldVar2 : MyDU = Case7 25

> exception MyNamedException1 of Val1: int * Val2: string
exception MyNamedException2 of string * V2: bool * float
exception MyNamedException3 of Data: int
exception MyNamedException4 of bool
exception MyNamedException5 of int * string
exception MyNamedException6 of Val1: int * bool * string * Data8: float
exception MyNamedException7 of Big Named Field: int
val namedEx1 : exn = MyNamedException1 (5, "")
val namedEx2 : exn = MyNamedException7 25

> type optionRecord =
  { x: int option }
val x : optionRecord = { x = None }

> type optionRecord =
  { x: obj }
val x : optionRecord = { x = null }

> > > 

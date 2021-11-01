
> val it: unit = ()

> val repeatId: string = "A"

> val repeatId: string = "B"

namespace FSI_0005
  val x1: int
  val x2: string
  val x3: 'a option
  val x4: int option
  val x5: 'a list
  val x6: int list
  val x7: System.Windows.Forms.Form
  val x8: int[,]
  val x9: Lazy<string>

namespace FSI_0006
  val x1: int
  val x2: string
  val x3: 'a option
  val x4: int option
  val x5: 'a list
  val x6: int list
  val x7: System.Windows.Forms.Form
  val x8: int[,]
  val x9: Lazy<string>

namespace FSI_0006
  val x1: int
  val x2: string
  val x3: 'a option
  val x4: int option
  val x5: 'a list
  val x6: int list
  val x7: System.Windows.Forms.Form
  val x8: int[,]
  val x9: Lazy<string>

> val x1: seq<string>
val x2: seq<string>
val x3: seq<string>
val f1: System.Windows.Forms.Form = System.Windows.Forms.Form, Text: f1 form
val fs: System.Windows.Forms.Form[] =
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
val xs: string list =
  ["0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"; "11"; "12"; "13";
   "14"; "15"; "16"; "17"; "18"; "19"; ...]
val xa: string[] =
  [|"0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"; "11"; "12"; "13";
    "14"; "15"; "16"; "17"; "18"; "19"; ...|]
val xa2: string[,] = [["0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"]
                      ["10"; "11"; "12"; "13"; "14"; "15"; "16"; "17"]
                      ["20"; "21"; "22"; "23"; ...]
                      ...]
val sxs0: Set<string> = set []

> val sxs1: Set<string> = set ["0"]

> val sxs2: Set<string> = set ["0"; "1"]

> val sxs3: Set<string> = set ["0"; "1"; "2"]

> val sxs4: Set<string> = set ["0"; "1"; "2"; "3"]

> val sxs200: Set<string> =
  set ["0"; "1"; "10"; "100"; "101"; "102"; "103"; "104"; "105"; ...]

> val msxs0: Map<int,string> = map []

> val msxs1: Map<int,string> = map [(0, "0")]

> val msxs2: Map<int,string> = map [(0, "0"); (1, "1")]

> val msxs3: Map<int,string> = map [(0, "0"); (1, "1"); (2, "2")]

> val msxs4: Map<int,string> = map [(0, "0"); (1, "1"); (2, "2"); (3, "3")]

> val msxs200: Map<int,string> =
  map
    [(0, "0"); (1, "1"); (2, "2"); (3, "3"); (4, "4"); (5, "5"); (6, "6");
     (7, "7"); (8, "8"); ...]

> module M =
  val a: string = "sub-binding"
  val b:
    (seq<string> * seq<string> * seq<string> * System.Windows.Forms.Form) option *
    (string list * string list * string[,]) option =
    (Some (<seq>, <seq>, <seq>, System.Windows.Forms.Form, Text: f1 form),
     Some
       (["0"; "1"; "2"; "3"; "4"; "5"; "6"; "7"; "8"; "9"; "10"; "11"; "12";
         "13"; "14"; "15"; "16"; ...], ..., ...))
type T =
  new: a: int * b: int -> T
  member AMethod: x: int -> int
  static member StaticMethod: x: int -> int
  member AProperty: int
  static member StaticProperty: int
val f_as_method: x: int -> int
val f_as_thunk: (int -> int)
val refCell: string ref = { contents = "value" }
module D1 =
  val words: System.Collections.Generic.IDictionary<string,int>
  val words2000: System.Collections.Generic.IDictionary<int,string>

> > module D2 =
  val words: IDictionary<string,int>
  val words2000: IDictionary<int,string>
val opt1: 'a option
val opt1b: int option = None
val opt4: 'a option option option option
val opt4b: int option option option option = Some (Some (Some None))
val opt5: int list option option option option option list =
  [Some (Some (Some (Some None)));
   Some (Some (Some (Some (Some [1; 2; 3; 4; 5; 6]))));
   Some (Some (Some (Some ...))); ...]
val mkStr: n: int -> string
val strs: string[] =
  [|""; "-"; "--"; "---"; "----"; "-----"; "------"; "-------"; "--------";
    "---------"; "----------"; "-----------"; "------------"; "-------------";
    "--------------"; "---------------"; "----------------";
    "-----------------"; "------------------"; "-------------------"; ...|]
val str7s: string[] =
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
val grids: string[,] =
  [[""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; ""; "";
    ""; ...]
   ...]

> type tree =
  | L
  | N of tree list
val mkT: w: int -> d: int -> tree
val tree: w: int -> d: int -> tree

> [Building 2 4...done]
val tree_2_4: tree =
  N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N [L; L]]];
     N [N [N [L; ...]; ...]; ...]; ...]

> [Building 2 6...done]
val tree_2_6: tree =
  N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N [L; L]]]; N [N ...; ...];
           ...]; ...]; ...]

> [Building 2 8...done]
val tree_2_8: tree =
  N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N [L; L; ...]; ...];
                    ...]; ...]; ...]; ...]; ...]; ...]

> [Building 2 10...done]
val tree_2_10: tree =
  N [N [N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; L]; N ...; ...];
                          ...]; ...]; ...]; ...]; ...]; ...]; ...]; ...]

> [Building 2 12...done]
val tree_2_12: tree =
  N [N [N [N [N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N [N [L; ...]; ...];
                                ...]; ...]; ...]; ...]; ...]; ...]; ...]; ...];
        ...]; ...]

> [Building 2 14...done]
val tree_2_14: tree =
  N [N [N [N [N [N [N [N [N [N [N [N [N [N [L; L]; N [L; L]]; N ...; ...]; ...];
                                ...]; ...]; ...]; ...]; ...]; ...]; ...]; ...];
        ...]; ...]

> [Building 3 8...done]
val tree_3_8: tree =
  N [N [N [N [N [N [N [N [L; L; L]; N [L; L; L]; N [L; L; L]]; N ...; ...];
                 ...]; ...]; ...]; ...]; ...]

> [Building 4 8...done]
val tree_4_8: tree =
  N [N [N [N [N [N [N [N [L; L; L; L]; N [L; L; L; L]; N [L; L; ...]; ...];
                    ...]; ...]; ...]; ...]; ...]; ...]

> [Building 5 8...done]
val tree_5_8: tree =
  N [N [N [N [N [N [N [N [L; L; L; L; L]; N [L; L; L; L; L]; N ...; ...]; ...];
                 ...]; ...]; ...]; ...]; ...]

> [Building 6 8...done]
val tree_6_8: tree =
  N [N [N [N [N [N [N [N [L; L; L; L; L; L]; N [L; L; L; L; L; ...]; ...]; ...];
                 ...]; ...]; ...]; ...]; ...]

> [Building 5 3...done]
val tree_5_3: tree =
  N [N [N [L; L; L; L; L]; N [L; L; L; L; L]; N [L; L; L; L; L; ...]; ...];
     ...]

> > type X =
  | Var of int
  | Bop of int * X * X
val generate: x: int -> X

> val exps: X list =
  [Bop (1, Var 0, Var 0); Var 2;
   Bop (3, Bop (1, Var 0, Var 0), Bop (1, Var 0, ...)); ...]

> module Exprs =
  val x1: X =
    Bop
      (213, Var 106,
       Bop
         (71,
          Bop (35, Bop (17, Var 8, Bop (5, Var 2, Bop (1, Var 0, ...))), ...),
          ...))
  val x2: X = Var 21342314
  val x3: X = Var 3214
  val x4: X = Bop (1231357, Var 615678, Var 410452)
  val x5: X =
    Bop
      (5234547, Bop (2617273, Var 1308636, Var 872424),
       Bop (1744849, Var 872424, Var 581616))
  val x6: X =
    Bop
      (923759825, Var 461879912, Bop (307919941, Var 153959970, Var 102639980))
  val x7: X = Var 2435234
  val x8: X =
    Bop
      (12396777, Var 6198388,
       Bop
         (4132259,
          Bop
            (2066129, Var 1033064,
             Bop
               (688709, Var 344354,
                Bop (229569, Var 114784, Bop (76523, ..., ...)))), ...))
  val x9: X =
    Bop
      (3333333, Var 1666666,
       Bop
         (1111111,
          Bop
            (555555, Bop (277777, Var 138888, Var 92592),
             Bop (185185, Var 92592, Var 61728)), ...))
  val x10: X =
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
  val x11: X =
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

> type C =
  new: x: string -> C
  override ToString: unit -> string
val c1: C = <ToString exception: Trouble_A>
val csA: C[] =
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
val csB: C[] =
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
val csC: C[] =
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

> val exA1: exn = Abc
val exA2: exn = AbcInt 2
val exA3: exn = AbcString "3"
val exA4: exn = AbcExn [Abc; AbcInt 2; AbcString "3"]
val exA5: exn = AbcException [AbcExn [Abc; AbcInt 2; AbcString "3"]]
exception Ex0
exception ExUnit of unit
exception ExUnits of unit * unit
exception ExUnitOption of unit option
val ex0: exn = Ex0
val exU: exn = ExUnit ()
val exUs: exn = ExUnits ((), ())
val exUSome: exn = ExUnitOption (Some ())
val exUNone: exn = ExUnitOption None
type 'a T4063 = | AT4063 of 'a

> val valAT3063_12: int T4063 = AT4063 12

> val valAT3063_True: bool T4063 = AT4063 true

> val valAT3063_text: string T4063 = AT4063 "text"

> val valAT3063_null: System.Object T4063 = AT4063 null

> type M4063<'a> =
  new: x: 'a -> M4063<'a>

> val v4063: M4063<int>

> type Taaaaa<'a> =
  new: unit -> Taaaaa<'a>

> type Taaaaa2<'a> =
  inherit Taaaaa<'a>
  new: unit -> Taaaaa2<'a>
  member M: unit -> Taaaaa2<'a>

> type Tbbbbb<'a> =
  new: x: 'a -> Tbbbbb<'a>
  member M: unit -> 'a

> type Tbbbbb2 =
  inherit Tbbbbb<string>
  new: x: string -> Tbbbbb2

> val it: (unit -> string) = <fun:it@197>

> module RepeatedModule =
  val repeatedByteLiteral: byte[] = [|12uy; 13uy; 14uy|]

> module RepeatedModule =
  val repeatedByteLiteral: byte[] = [|12uy; 13uy; 14uy|]

> val it: string = "Check #help"

> 
  F# Interactive directives:

    #r "file.dll";;                               // Reference (dynamically load) the given DLL
    #i "package source uri";;                     // Include package source uri when searching for packages
    #I "path";;                                   // Add the given search path for referenced DLLs
    #load "file.fs" ...;;                         // Load the given file(s) as if compiled and referenced
    #time ["on"|"off"];;                          // Toggle timing on/off
    #help;;                                       // Display help
    #r "nuget:FSharp.Data, 3.1.2";;               // Load Nuget Package 'FSharp.Data' version '3.1.2'
    #r "nuget:FSharp.Data";;                      // Load Nuget Package 'FSharp.Data' with the highest version
    #quit;;                                       // Exit

  F# Interactive command line options:



> val it: string = "Check #time on and then off"

> 
--> Timing now on

> 
--> Timing now off

> val it: string = "Check #unknown command"

> val it: string =
  "Check #I with a known directory (to avoid a warning, which includes the location of this file, which is fragile...)"

> 
--> Added '/' to library include path

> type internal T1 =
  | A
  | B

> type internal T2 =
  { x: int }

> type internal T3

> type internal T4 =
  new: unit -> T4

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

> type private T3

> type private T4 =
  new: unit -> T4

> exception X1 of int

> exception private X2 of int

> exception internal X3 of int

> type T0 =
  new: unit -> T0
type T1Post<'a> =
  new: unit -> T1Post<'a>
type 'a T1Pre =
  new: unit -> 'a T1Pre

> type T0 with
  member M: unit -> T0 list
type T0 with
  member P: T0 * T0
type T0 with
  member E: IEvent<int>

> type T1Post<'a> with
  member M: unit -> T1Post<'a> list
type T1Post<'a> with
  member P: T1Post<'a> * T1Post<'a>
type T1Post<'a> with
  member E: IEvent<obj>

> type 'a T1Pre with
  member M: unit -> 'a T1Pre list
type 'a T1Pre with
  member P: 'a T1Pre * 'a T1Pre
type 'a T1Pre with
  member E: IEvent<obj>

> type T1Post<'a> with
  member M: unit -> T1Post<'a> list
type T1Post<'a> with
  member P: T1Post<'a> * T1Post<'a>
type T1Post<'a> with
  member E: IEvent<obj>

> type 'a T1Pre with
  member M: unit -> 'a T1Pre list
type 'a T1Pre with
  member P: 'a T1Pre * 'a T1Pre
type 'a T1Pre with
  member E: IEvent<obj>

> type r =
  {
    f0: int
    f1: int
    f2: int
    f3: int
    f4: int
    f5: int
    f6: int
    f7: int
    f8: int
    f9: int
  }
val r10: r = { f0 = 0
               f1 = 1
               f2 = 2
               f3 = 3
               f4 = 4
               f5 = 5
               f6 = 6
               f7 = 7
               f8 = 8
               f9 = 9 }
val r10s: r[] = [|{ f0 = 0
                    f1 = 1
                    f2 = 2
                    f3 = 3
                    f4 = 4
                    f5 = 5
                    f6 = 6
                    f7 = 7
                    f8 = 8
                    f9 = 9 }; ...|]
val r10s': string * r[] = ("one extra node", [|{ f0 = 0
                                                 f1 = 1
                                                 f2 = 2
                                                 f3 = 3
                                                 f4 = 4
                                                 f5 = 5
                                                 f6 = 6
                                                 f7 = 7
                                                 f8 = 8
                                                 f9 = ... }; ...|])

> val x1564_A1: int = 1


--> Added '\' to library include path

val x1564_A2: int = 2


--> Added '\' to library include path

val x1564_A3: int = 3

> type internal Foo2 =
  private new: x: int * y: int * z: int -> Foo2 + 3 overloads
  member Prop1: int
  member Prop2: int
  member private Prop3: int

> module internal InternalM =
  val x: int = 1
  type Foo2 =
    private new: x: int * y: int * z: int -> Foo2 + 3 overloads
    member Prop1: int
    member Prop2: int
    member private Prop3: int
  type private Foo3 =
    new: x: int * y: int * z: int -> Foo3 + 3 overloads
    member Prop1: int
    member Prop2: int
    member Prop3: int
  type T1 =
    | A
    | B
  type T2 =
    { x: int }
  type T3
  type T4 =
    new: unit -> T4
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
  type private T13
  type private T14 =
    new: unit -> T14
module internal PrivateM =
  val private x: int = 1
  type private Foo2 =
    new: x: int * y: int * z: int -> Foo2 + 3 overloads
    member Prop1: int
    member Prop2: int
    member Prop3: int
  type T1 =
    | A
    | B
  type T2 =
    { x: int }
  type T3
  type T4 =
    new: unit -> T4
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
  type private T13
  type private T14 =
    new: unit -> T14

> val it: seq<int * string * int> =
  seq
    [(43, "10/28/2008", 1); (46, "11/18/2008", 1); (56, "1/27/2009", 2);
     (58, "2/10/2009", 1)]

> module Test4343a =
  val mk: i: int -> string
  val x100: string =
    "0123456789012345678901234567890123456789012345678901234567890"+[39 chars]
  val x90: string =
    "0123456789012345678901234567890123456789012345678901234567890"+[29 chars]
  val x80: string =
    "0123456789012345678901234567890123456789012345678901234567890"+[19 chars]
  val x75: string =
    "0123456789012345678901234567890123456789012345678901234567890"+[14 chars]
  val x74: string =
    "0123456789012345678901234567890123456789012345678901234567890"+[13 chars]
  val x73: string =
    "0123456789012345678901234567890123456789012345678901234567890"+[12 chars]
  val x72: string =
    "012345678901234567890123456789012345678901234567890123456789012345678901"
  val x71: string =
    "01234567890123456789012345678901234567890123456789012345678901234567890"
  val x70: string =
    "0123456789012345678901234567890123456789012345678901234567890123456789"
module Test4343b =
  val fA: x: int -> int
  val fB: x: 'a -> y: 'a -> 'a list
  val gA: (int -> int)
  val gB: ('a -> 'a -> 'a list)
  val gAB: (int -> int) * ('a -> 'a -> 'a list)
  val hB: ('a -> 'a -> 'a list)
  val hA: (int -> int)
module Test4343c =
  val typename<'a> : string
  val typename2<'a> : string * string
module Test4343d =
  val xList: int list = [1; 2; 3]
  val xArray: int[] = [|1; 2; 3|]
  val xString: string = "abcdef"
  val xOption: int option = Some 12
  val xArray2: (int * int)[,] = [[(0, 0); (0, 1)]
                                 [(1, 0); (1, 1)]]
  val xSeq: seq<int>
module Test4343e =
  type C =
    new: x: int -> C
  val cA: C
  val cB: C
  val cAB: C * C * C list =
    (FSI_0091+Test4343e+C, FSI_0091+Test4343e+C,
     [FSI_0091+Test4343e+C; FSI_0091+Test4343e+C])
  type D =
    new: x: int -> D
    override ToString: unit -> string
  val dA: D = D(1)
  val dB: D = D(2)
  val dAB: D * D * D list = (D(1), D(2), [D(1); D(2)])
  module Generic =
    type CGeneric<'a> =
      new: x: 'a -> CGeneric<'a>
    val cA: C
    val cB: C
    val cAB: C * C * C list =
      (FSI_0091+Test4343e+C, FSI_0091+Test4343e+C,
       [FSI_0091+Test4343e+C; FSI_0091+Test4343e+C])
    type D<'a> =
      new: x: 'a -> D<'a>
      override ToString: unit -> string
    val dA: D<int> = D(1)
    val dB: D<int> = D(2)
    val dAB: D<int> * D<int> * D<int> list = (D(1), D(2), [D(1); D(2)])
    val dC: D<bool> = D(True)
    val boxed_dABC: obj list = [D(1); D(2); D(True)]
type F1 =
  inherit System.Windows.Forms.Form
  interface System.IDisposable
  val x: F1
  val x2: F1
  member B: unit -> int
  member D: x: int -> int + 2 overloads
  abstract MMM: bool -> bool
  override ToString: unit -> string
  static member A: unit -> int
  static member C: unit -> int
  abstract AAA: int
  abstract BBB: bool with set
  member D2: int
  member E: int
  abstract ZZZ: int
  static val mutable private sx: F1
  static val mutable private sx2: F1
[<Struct>]
type IP =
  new: x: int * y: int -> IP
  static val mutable private AA: IP
module Regression4643 =
  [<Struct>]
  type RIP =
    new: x: int -> RIP
    static val mutable private y: RIP
  [<Struct>]
  type arg_unused_is_RIP =
    new: x: RIP -> arg_unused_is_RIP
  [<Struct>]
  type arg_used_is_RIP =
    new: x: RIP -> arg_used_is_RIP
    member X: RIP
  [<Struct>]
  type field_is_RIP =
    val x: RIP
type Either<'a,'b> =
  | This of 'a
  | That of 'b
val catch: f: (unit -> 'a) -> Either<'a,(string * string)>
val seqFindIndexFailure: Either<int,(string * string)> =
  That
    ("System.Collections.Generic.KeyNotFoundException",
     "An index satisfying the predicate was not found in the collection.")
val seqFindFailure: Either<int,(string * string)> =
  That
    ("System.Collections.Generic.KeyNotFoundException",
     "An index satisfying the predicate was not found in the collection.")
val seqPickFailure: Either<int,(string * string)> =
  That
    ("System.Collections.Generic.KeyNotFoundException",
     "An index satisfying the predicate was not found in the collection.")
module Regression5218 =
  val t1: int = 1
  val t2: int * int = (1, 2)
  val t3: int * int * int = (1, 2, 3)
  val t4: int * int * int * int = (1, 2, 3, 4)
  val t5: int * int * int * int * int = (1, 2, 3, 4, 5)
  val t6: int * int * int * int * int * int = (1, 2, 3, 4, 5, 6)
  val t7: int * int * int * int * int * int * int = (1, 2, 3, 4, 5, 6, 7)
  val t8: int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8)
  val t9: int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9)
  val t10: int * int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
  val t11: int * int * int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)
  val t12:
    int * int * int * int * int * int * int * int * int * int * int * int =
    (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)
  val t13:
    int * int * int * int * int * int * int * int * int * int * int * int *
    int = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13)
  val t14:
    int * int * int * int * int * int * int * int * int * int * int * int *
    int * int = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14)
  val t15:
    int * int * int * int * int * int * int * int * int * int * int * int *
    int * int * int = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)

> module Regression3739 =
  type IB =
    abstract AbstractMember: int -> int
  type C<'a when 'a :> IB> =
    new: unit -> C<'a>
    static member StaticMember: x: 'a -> int

> module Regression3739 =
  type IB =
    abstract AbstractMember: int -> int
  type C<'a when 'a :> IB> =
    new: unit -> C<'a>
    static member StaticMember: x: 'a -> int

> module Regression3740 =
  type Writer<'a> =
    abstract get_path: unit -> string
  type MyClass =
    interface Writer<int>
    val path: string

> type Regression4319_T2 =
  static member (+-+-+) : x: 'a * y: 'b -> string

> type Regression4319_T0 =
  static member (+-+-+) : string

> type Regression4319_T1 =
  static member (+-+-+) : x: 'a -> string

> type Regression4319_T1b =
  static member (+-+-+) : x: 'a -> string

> type Regression4319_T1c =
  static member (+-+-+) : x: ('a * 'b) -> string

> type Regression4319_T1d =
  static member (+-+-+) : x: (int * int) -> string

> type Regression4319_T3 =
  static member (+-+-+) : x: 'a * y: 'b * z: 'c -> string

> type Regression4319_U1 =
  static member (+-+-+) : x: 'a -> moreArgs: 'b -> string

> type Regression4319_U1b =
  static member (+-+-+) : x: 'a -> moreArgs: 'b -> string

> type Regression4319_U2 =
  static member (+-+-+) : x: 'a * y: 'b -> moreArgs: 'c -> string

> type Regression4319_U3 =
  static member (+-+-+) : x: 'a * y: 'b * z: 'c -> moreArgs: 'd -> string

> type Regression4319_check =
  static member (&) : string
  static member (&^) : string
  static member (@) : string
  static member (!=) : string
  static member (:=) : string
  static member (^) : string
  static member (/) : string
  static member ($) : string
  static member (...@) : string
  static member (...!=) : string
  static member (.../) : string
  static member (...=) : string
  static member (...>) : string
  static member (...^) : string
  static member (...<) : string
  static member ( ...* ) : string
  static member (...%) : string
  static member (=) : string
  static member ( ** ) : string
  static member (>) : string
  static member (<) : string
  static member (%) : string
  static member ( * ) : string
  static member (-) : string

> Expect ABC = ABC
type Regression4469 =
  new: unit -> Regression4469
  member ToString: unit -> string
val r4469: Regression4469 = FSI_0107+Regression4469
val it: unit = ()

> Expect ABC = ABC
val it: unit = ()

> module Regression1019_short =
  val double_nan: float = nan
  val double_infinity: float = infinity
  val single_nan: float32 = nanf
  val single_infinity: float32 = infinityf
module Regression1019_long =
  val double_nan: float = nan
  val double_infinity: float = infinity
  val single_nan: float32 = nanf
  val single_infinity: float32 = infinityf

> val it: int ref = { contents = 1 }

> val x: int ref = { contents = 1 }
val f: (unit -> int)

> val it: int = 1

> val it: unit = ()

> val it: int = 3

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: int[] =
  [|0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
    ...|]

> val it: 'a list

> val it: 'a list list

> val it: 'a option

> val it: 'a list * 'b list

> val it: x: 'a -> 'a

> val fff: x: 'a -> 'a

> val it: ('a -> 'a)

> val note_ExpectDupMethod: string =
  "Regression4927: Expect error due to duplicate methods in the "+[20 chars]

> > val note_ExpectDupProperty: string =
  "Regression4927: Expect error due to duplicate properties in t"+[23 chars]

> > > val it: string = "NOTE: Expect IAPrivate less accessible IBPublic"

> > val it: string = "NOTE: Expect IAPrivate less accessible IBInternal"

> > module Regression5265_PriPri =
  type private IAPrivate =
    abstract P: int
  type private IBPrivate =
    inherit IAPrivate
    abstract Q: int

> val it: string = "NOTE: Expect IAInternal less accessible IBPublic"

> > module Regression5265_IntInt =
  type internal IAInternal =
    abstract P: int
  type internal IBInternal =
    inherit IAInternal
    abstract Q: int

> module Regression5265_IntPri =
  type internal IAInternal =
    abstract P: int
  type private IBPrivate =
    inherit IAInternal
    abstract Q: int

> module Regression5265_PubPub =
  type IAPublic =
    abstract P: int
  type IBPublic =
    inherit IAPublic
    abstract Q: int

> module Regression5265_PubInt =
  type IAPublic =
    abstract P: int
  type internal IBInternal =
    inherit IAPublic
    abstract Q: int

> module Regression5265_PubPri =
  type IAPublic =
    abstract P: int
  type private IBPrivate =
    inherit IAPublic
    abstract Q: int

> val it: string =
  "Regression4232: Expect an error about duplicate virtual methods from parent type"

> > val it: string =
  "** Expect AnAxHostSubClass to be accepted. AxHost has a newslot virtual RightToLeft property outscope RightToLeft on Control"

> type AnAxHostSubClass =
  inherit System.Windows.Forms.AxHost
  new: x: string -> AnAxHostSubClass

> val it: string =
  "** Expect error because the active pattern result contains free type variables"

> > val it: string =
  "** Expect error because the active pattern result contains free type variables (match value generic)"

> > val it: string =
  "** Expect error because the active pattern result contains free type variables (when active pattern also has parameters)"

> > val it: string =
  "** Expect OK, since error message says constraint should work!"

> val (|A|B|) : x: int -> Choice<int,unit>

> val it: string = "** Expect error since active pattern is not a function!"

> > val it: string =
  "** Expect OK since active pattern result is not too generic, typars depend on match val"

> val (|A|B|) : p: bool -> 'a * 'b -> Choice<'a,'b>

> val it: string =
  "** Expect OK since active pattern result is not too generic, typars depend on parameters"

> val (|A|B|) : aval: 'a -> bval: 'b -> x: bool -> Choice<'a,'b>

> val it: string =
  "** Expect OK since active pattern result is generic, but it typar from closure, so OK"

> val outer: x: 'a -> (int -> 'a option)

> val it: string =
  "** Expect OK, BUG 472278: revert unintended breaking change to Active Patterns in F# 3.0"

> val (|Check1|) : a: int -> int * 'a option

> > module ReflectionEmit =
  type IA =
    abstract M: #IB -> int
  and IB =
    abstract M: #IA -> int
  type IA2<'a when 'a :> IB2<'a> and 'a :> IA2<'a>> =
    abstract M: int
  and IB2<'b when 'b :> IA2<'b> and 'b :> IB2<'b>> =
    abstract M: int

> val it: string =
  "Regression_139182: Expect the follow code to be accepted without error"

> [<Struct>]
type S =
  member TheMethod: unit -> int64
val theMethod: s: S -> int64
type T =
  new: unit -> T
  member Prop5: int64
  static member Prop1: int64
  static member Prop2: int64
  static member Prop3: int64
  static member Prop4: string

> val it: System.Threading.ThreadLocal<int> list = [0 {IsValueCreated = false;
                                                     Values = ?;}]

> type MyDU =
  | Case1 of Val1: int * Val2: string
  | Case2 of string * V2: bool * float
  | Case3 of int
  | Case4 of Item1: bool
  | Case5 of bool * string
  | Case6 of Val1: int * bool * string
  | Case7 of ``Big Name`` : int
val namedFieldVar1: MyDU = Case1 (5, "")
val namedFieldVar2: MyDU = Case7 25

> exception MyNamedException1 of Val1: int * Val2: string
exception MyNamedException2 of string * V2: bool * float
exception MyNamedException3 of Data: int
exception MyNamedException4 of bool
exception MyNamedException5 of int * string
exception MyNamedException6 of Val1: int * bool * string * Data8: float
exception MyNamedException7 of ``Big Named Field`` : int
val namedEx1: exn = MyNamedException1 (5, "")
val namedEx2: exn = MyNamedException7 25

> type optionRecord =
  { x: int option }
val x: optionRecord = { x = None }

> type optionRecord =
  { x: obj }
val x: optionRecord = { x = null }

> type RecordWithMembers =
  { x: obj }
  member Method: unit -> int
  member Property: int

> type UnionWithMembers =
  | Case1
  | Case2 of int
  member Method: unit -> int
  member Property: int

> type OneFieldRecordNoXmlDoc =
  { OneField: obj }

> type OneFieldRecordXmlDoc =
  {
    OneField: obj
  }

> type TwoFieldRecordNoXmlDoc =
  {
    TwoFields1: obj
    TwoFields2: obj
  }

> type TwoFieldRecordXmlDoc =
  {
    TwoFields1: obj
    TwoFields2: obj
  }

> type Int32 with
  member ExtrinsicExtensionProperty: int
type Int32 with
  member ExtrinsicExtensionMethod: unit -> int

> val ``value with spaces in name`` : bool = true

> val functionWhichTakesLongNameMixedParameters:
  aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: int *
  bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb: int
  -> ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc: int *
     dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd: int
    -> int

> val functionWhichTakesLongNameTupledParameters:
  aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: int *
  bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb: int *
  ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc: int *
  ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd: int
    -> int

> val functionWhichTakesLongNameCurriedParameters:
  aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: int
  -> bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb: int
  -> cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc: int
  -> dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd: int
    -> int

> val functionWhichTakesMixedLengthCurriedParametersA:
  a: 'a -> b: 'b -> c: 'c -> ddddddddddddddddddddddddddddddddddddddddddddd: 'd
    -> int

> val functionWhichTakesMixedLengthCurriedParametersB:
  aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: 'a -> b: 'b -> c: 'c -> d: 'd -> int

> val f: ``parameter with spaces in name`` : int -> int

> val functionWhichTakesAParameterPeeciselyPlusButNotOpAddition:
  ``+`` : (int -> int -> int) -> int

> val functionWhichTakesAParameterOpAddition: (+) : (int -> int -> int) -> int

> val functionWhichTakesAParameterCalled_land:
  ``land`` : (int -> int -> int) -> int

> type RecordWithStrangeNames =
  {
    ``funky name`` : obj
    op_Addition: obj
    ``+`` : obj
    ``land`` : obj
    ``base`` : obj
  }

> type UnionWithSpacesInNamesOfCases =
  | ``Funky name``
  | ``Funky name 2``

> type ``Type with spaces in name`` =
  | A
  | B

> type op_Addition =
  | A
  | B

> type ``land`` =
  | A
  | B

> module ``Module with spaces in name`` =
  val x: int = 1

> module op_Addition =
  val x: int = 1

> module ``land`` =
  val x: int = 1

> val ``+`` : x: 'a -> y: 'b -> int

> val (+) : x: int -> y: int -> int

> val ``base`` : int = 2

> val (mod) : int = 2

> val ``or`` : int = 2

> val ``land`` : int = 2

> val ``.ctor`` : int = 2

> val ``.cctor`` : int = 2

> [<Literal>]
val SomeLiteralWithASomewhatLongName: string
  = "SomeVeryLongLiteralValueWithLotsOfCharacters"
[<Literal>]
val SomeLiteralWithASomewhatLongName2: string
  =
  "SomeVeryLongLiteralValueWithLotsOfCharactersSomeVeryLongLiteralValueWithLotsOfCharactersSomeVeryLongLiteralValueWithLotsOfCharacters"
[<Literal>]
val ShortName: string = "hi"

> > > 

val it: System.DayOfWeek = Tuesday {value__ = 2;}
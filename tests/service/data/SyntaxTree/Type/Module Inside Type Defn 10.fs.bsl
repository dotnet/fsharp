ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 10.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([A], [], [None])),
                        (3,9--3,10)), (3,9--3,10)), [], None, (3,5--3,10),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--3,10));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (B, None), Fields [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,7), { BarRange = Some (6,4--6,5) })],
                        (6,4--6,7)), (6,4--6,7)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]; []],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent
                              ([this; M1], [(7,15--7,16)], [None; None]), None,
                            None, Pats [], None, (7,11--7,18)), None,
                         Const (Int32 1, (7,21--7,22)), (7,11--7,18),
                         NoneAtInvisible, { LeadingKeyword = Member (7,4--7,10)
                                            InlineKeyword = None
                                            EqualsRange = Some (7,19--7,20) }),
                      (7,4--7,22));
                   Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]; []],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent
                              ([this; M2], [(8,15--8,16)], [None; None]), None,
                            None, Pats [], None, (8,11--8,18)), None,
                         Const (Int32 2, (8,21--8,22)), (8,11--8,18),
                         NoneAtInvisible, { LeadingKeyword = Member (8,4--8,10)
                                            InlineKeyword = None
                                            EqualsRange = Some (8,19--8,20) }),
                      (8,4--8,22));
                   Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]; []],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent
                              ([this; M3], [(9,15--9,16)], [None; None]), None,
                            None, Pats [], None, (9,11--9,18)), None,
                         Const (Int32 3, (9,21--9,22)), (9,11--9,18),
                         NoneAtInvisible, { LeadingKeyword = Member (9,4--9,10)
                                            InlineKeyword = None
                                            EqualsRange = Some (9,19--9,20) }),
                      (9,4--9,22))], None, (5,5--9,22),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,7--5,8)
                    WithKeyword = None })], (5,0--9,22));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M1],
                 PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (10,4--10,13)), false, [], false, (10,4--10,25),
              { ModuleKeyword = Some (10,4--10,10)
                EqualsRange = Some (10,14--10,15) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M2],
                 PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (11,4--11,13)), false, [], false, (11,4--11,25),
              { ModuleKeyword = Some (11,4--11,10)
                EqualsRange = Some (11,14--11,15) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M3],
                 PreXmlDoc ((12,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (12,4--12,13)), false, [], false, (12,4--12,25),
              { ModuleKeyword = Some (12,4--12,10)
                EqualsRange = Some (12,14--12,15) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [M4],
                     PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (13,9--13,11)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [], Const (Unit, (13,11--13,13)), None,
                         PreXmlDoc ((13,11), FSharp.Compiler.Xml.XmlDocCollector),
                         (13,9--13,11), { AsKeyword = None })], (13,16--13,25)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (13,11--13,13)), None,
                        PreXmlDoc ((13,11), FSharp.Compiler.Xml.XmlDocCollector),
                        (13,9--13,11), { AsKeyword = None })), (13,9--13,25),
                  { LeadingKeyword = Type (13,4--13,8)
                    EqualsRange = Some (13,14--13,15)
                    WithKeyword = None })], (13,4--13,25));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [M5],
                     PreXmlDoc ((14,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (14,9--14,11)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [], Const (Unit, (14,11--14,13)), None,
                         PreXmlDoc ((14,11), FSharp.Compiler.Xml.XmlDocCollector),
                         (14,9--14,11), { AsKeyword = None })], (14,16--14,25)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (14,11--14,13)), None,
                        PreXmlDoc ((14,11), FSharp.Compiler.Xml.XmlDocCollector),
                        (14,9--14,11), { AsKeyword = None })), (14,9--14,25),
                  { LeadingKeyword = Type (14,4--14,8)
                    EqualsRange = Some (14,14--14,15)
                    WithKeyword = None })], (14,4--14,25));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [M6],
                     PreXmlDoc ((15,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (15,9--15,11)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [], Const (Unit, (15,11--15,13)), None,
                         PreXmlDoc ((15,11), FSharp.Compiler.Xml.XmlDocCollector),
                         (15,9--15,11), { AsKeyword = None })], (15,16--15,25)),
                  [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (15,11--15,13)), None,
                        PreXmlDoc ((15,11), FSharp.Compiler.Xml.XmlDocCollector),
                        (15,9--15,11), { AsKeyword = None })), (15,9--15,25),
                  { LeadingKeyword = Type (15,4--15,8)
                    EqualsRange = Some (15,14--15,15)
                    WithKeyword = None })], (15,4--15,25));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((17,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (17,5--17,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([C], [], [None])),
                        (17,9--17,10)), (17,9--17,10)), [], None, (17,5--17,10),
                  { LeadingKeyword = Type (17,0--17,4)
                    EqualsRange = Some (17,7--17,8)
                    WithKeyword = None })], (17,0--17,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--17,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(10,4)-(10,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(11,4)-(11,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(12,4)-(12,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(13,4)-(13,8) parse warning Nested type definitions are not allowed. Types must be defined at module or namespace level.
(14,4)-(14,8) parse warning Nested type definitions are not allowed. Types must be defined at module or namespace level.
(15,4)-(15,8) parse warning Nested type definitions are not allowed. Types must be defined at module or namespace level.

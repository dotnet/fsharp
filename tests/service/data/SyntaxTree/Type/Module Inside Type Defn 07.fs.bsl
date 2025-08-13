ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 07.fs", false,
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
           NestedModule
             (SynComponentInfo
                ([], None, [], [M1],
                 PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,0--5,9)), false, [], false, (5,0--5,21),
              { ModuleKeyword = Some (5,0--5,6)
                EqualsRange = Some (5,10--5,11) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M2],
                 PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,0--6,9)), false, [], false, (6,0--6,21),
              { ModuleKeyword = Some (6,0--6,6)
                EqualsRange = Some (6,10--6,11) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M3],
                 PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,0--7,9)), false, [], false, (7,0--7,21),
              { ModuleKeyword = Some (7,0--7,6)
                EqualsRange = Some (7,10--7,11) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((9,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (9,5--9,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (B, None), Fields [],
                            PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (10,6--10,7), { BarRange = Some (10,4--10,5) })],
                        (10,4--10,7)), (10,4--10,7)), [], None, (9,5--10,7),
                  { LeadingKeyword = Type (9,0--9,4)
                    EqualsRange = Some (9,7--9,8)
                    WithKeyword = None })], (9,0--10,7));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M4],
                 PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (11,4--11,13)), false, [], false, (11,4--11,25),
              { ModuleKeyword = Some (11,4--11,10)
                EqualsRange = Some (11,14--11,15) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M5],
                 PreXmlDoc ((12,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (12,4--12,13)), false, [], false, (12,4--12,25),
              { ModuleKeyword = Some (12,4--12,10)
                EqualsRange = Some (12,14--12,15) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M6],
                 PreXmlDoc ((13,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (13,4--13,13)), false, [], false, (13,4--13,25),
              { ModuleKeyword = Some (13,4--13,10)
                EqualsRange = Some (13,14--13,15) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((15,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (15,5--15,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([C], [], [None])),
                        (15,9--15,10)), (15,9--15,10)), [], None, (15,5--15,10),
                  { LeadingKeyword = Type (15,0--15,4)
                    EqualsRange = Some (15,7--15,8)
                    WithKeyword = None })], (15,0--15,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--15,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(11,4)-(11,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(12,4)-(12,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(13,4)-(13,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.

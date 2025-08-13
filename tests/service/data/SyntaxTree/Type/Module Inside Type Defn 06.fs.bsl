ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 06.fs", false,
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
                        (6,4--6,7)), (6,4--6,7)), [], None, (5,5--6,7),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,7--5,8)
                    WithKeyword = None })], (5,0--6,7));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M1],
                 PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,4--7,13)), false, [], false, (7,4--7,25),
              { ModuleKeyword = Some (7,4--7,10)
                EqualsRange = Some (7,14--7,15) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M2],
                 PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (8,4--8,13)), false, [], false, (8,4--8,25),
              { ModuleKeyword = Some (8,4--8,10)
                EqualsRange = Some (8,14--8,15) });
           NestedModule
             (SynComponentInfo
                ([], None, [], [M3],
                 PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (9,4--9,13)), false, [], false, (9,4--9,25),
              { ModuleKeyword = Some (9,4--9,10)
                EqualsRange = Some (9,14--9,15) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [C],
                     PreXmlDoc ((11,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (11,5--11,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([C], [], [None])),
                        (11,9--11,10)), (11,9--11,10)), [], None, (11,5--11,10),
                  { LeadingKeyword = Type (11,0--11,4)
                    EqualsRange = Some (11,7--11,8)
                    WithKeyword = None })], (11,0--11,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--11,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(7,4)-(7,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(8,4)-(8,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(9,4)-(9,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.

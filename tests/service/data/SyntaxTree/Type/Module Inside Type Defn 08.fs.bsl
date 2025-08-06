ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 08.fs", false,
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
                    WithKeyword = Some (11,4--11,8) })], (9,0--10,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--10,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(12,8)-(12,14) parse error Unexpected keyword 'module' in type definition. Expected incomplete structured construct at or before this point, 'end' or other token.
(13,8)-(13,14) parse error Unexpected keyword 'module' in implementation file

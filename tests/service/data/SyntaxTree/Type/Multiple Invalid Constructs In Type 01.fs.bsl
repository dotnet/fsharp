ImplFile
  (ParsedImplFileInput
     ("/root/Type/Multiple Invalid Constructs In Type 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MultiTest],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,14)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Case1, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,11), { BarRange = Some (5,4--5,5) });
                         SynUnionCase
                           ([], SynIdent (Case2, None), Fields [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,11), { BarRange = Some (6,4--6,5) })],
                        (5,4--6,11)), (5,4--6,11)), [], None, (4,5--6,11),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,15--4,16)
                    WithKeyword = None })], (4,0--6,11));
           NestedModule
             (SynComponentInfo
                ([], None, [], [NestedModule],
                 PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,4--7,23)), false, [], false, (7,4--7,35),
              { ModuleKeyword = Some (7,4--7,10)
                EqualsRange = Some (7,24--7,25) });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [NestedType],
                     PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (8,9--8,19)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (8,22--8,25)), (8,22--8,25)), [], None, (8,9--8,25),
                  { LeadingKeyword = Type (8,4--8,8)
                    EqualsRange = Some (8,20--8,21)
                    WithKeyword = None })], (8,4--8,25));
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (NestedExc, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([string], [], [None])),
                             false,
                             PreXmlDoc ((9,27), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (9,27--9,33), { LeadingKeyword = None
                                                   MutableKeyword = None })],
                       PreXmlDocEmpty, None, (9,14--9,33), { BarRange = None }),
                    None, PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (9,4--9,33)), None, [], (9,4--9,33)), (9,4--9,33));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections], [(10,15--10,16)], [None; None]),
                 (10,9--10,27)), (10,4--10,27))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--10,27), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,68)] }, set []))

(7,4)-(7,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(8,4)-(8,8) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.
(9,4)-(9,13) parse error Exceptions must be defined at module level, not inside types.
(10,4)-(10,8) parse error 'open' declarations must appear at module level, not inside types.

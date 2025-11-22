ImplFile
  (ParsedImplFileInput
     ("/root/Type/Deeply Indented Type 01.fs", false,
      QualifiedNameOfFile OuterModule, [],
      [SynModuleOrNamespace
         ([OuterModule], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [InnerModule],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,0--4,18)), false,
              [NestedModule
                 (SynComponentInfo
                    ([], None, [], [DeeplyNested],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,23)), false,
                  [Types
                     ([SynTypeDefn
                         (SynComponentInfo
                            ([], None, [], [IndentedType],
                             PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (6,13--6,25)),
                          Simple (None (6,13--6,27), (6,13--6,27)), [], None,
                          (6,13--6,27), { LeadingKeyword = Type (6,8--6,12)
                                          EqualsRange = Some (6,26--6,27)
                                          WithKeyword = None })], (6,8--6,27));
                   Types
                     ([SynTypeDefn
                         (SynComponentInfo
                            ([], None, [], [NestedType],
                             PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (7,17--7,27)),
                          Simple
                            (TypeAbbrev
                               (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                                (7,30--7,33)), (7,30--7,33)), [], None,
                          (7,17--7,33), { LeadingKeyword = Type (7,12--7,16)
                                          EqualsRange = Some (7,28--7,29)
                                          WithKeyword = None })], (7,12--7,33));
                   NestedModule
                     (SynComponentInfo
                        ([], None, [], [NestedModule],
                         PreXmlDoc ((8,12), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (8,12--8,31)), false,
                      [Let
                         (false,
                          [SynBinding
                             (None, Normal, false, false, [],
                              PreXmlDoc ((9,16), FSharp.Compiler.Xml.XmlDocCollector),
                              SynValData
                                (None,
                                 SynValInfo ([], SynArgInfo ([], false, None)),
                                 None),
                              Named
                                (SynIdent (x, None), false, None, (9,20--9,21)),
                              None, Const (Int32 1, (9,24--9,25)), (9,20--9,21),
                              Yes (9,16--9,25),
                              { LeadingKeyword = Let (9,16--9,19)
                                InlineKeyword = None
                                EqualsRange = Some (9,22--9,23) })],
                          (9,16--9,25), { InKeyword = None })], false,
                      (8,12--9,25), { ModuleKeyword = Some (8,12--8,18)
                                      EqualsRange = Some (8,32--8,33) });
                   Exception
                     (SynExceptionDefn
                        (SynExceptionDefnRepr
                           ([],
                            SynUnionCase
                              ([], SynIdent (NestedExc, None), Fields [],
                               PreXmlDocEmpty, None, (10,22--10,31),
                               { BarRange = None }), None,
                            PreXmlDoc ((10,12), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (10,12--10,31)), None, [], (10,12--10,31)),
                      (10,12--10,31))], false, (5,4--10,31),
                  { ModuleKeyword = Some (5,4--5,10)
                    EqualsRange = Some (5,24--5,25) })], false, (4,0--10,31),
              { ModuleKeyword = Some (4,0--4,6)
                EqualsRange = Some (4,19--4,20) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--10,31), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,52)] }, set []))

(7,12)-(7,16) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.
(6,13)-(6,27) parse error A type definition requires one or more members or other declarations. If you intend to define an empty class, struct or interface, then use 'type ... = class end', 'interface end' or 'struct end'.
(8,12)-(8,18) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(10,12)-(10,21) parse error Exceptions must be defined at module level, not inside types.

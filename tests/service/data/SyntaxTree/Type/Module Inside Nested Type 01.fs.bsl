ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Nested Type 01.fs", false,
      QualifiedNameOfFile Level1, [],
      [SynModuleOrNamespace
         ([Level1], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [Level2],
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,0--4,13)), false,
              [NestedModule
                 (SynComponentInfo
                    ([], None, [], [Level3],
                     PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,4--5,17)), false,
                  [Types
                     ([SynTypeDefn
                         (SynComponentInfo
                            ([], None, [], [MyType],
                             PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                             false, None, (6,13--6,19)),
                          Simple
                            (Union
                               (None,
                                [SynUnionCase
                                   ([], SynIdent (A, None), Fields [],
                                    PreXmlDoc ((7,12), FSharp.Compiler.Xml.XmlDocCollector),
                                    None, (7,14--7,15),
                                    { BarRange = Some (7,12--7,13) })],
                                (7,12--7,15)), (7,12--7,15)), [], None,
                          (6,13--7,15), { LeadingKeyword = Type (6,8--6,12)
                                          EqualsRange = Some (6,20--6,21)
                                          WithKeyword = None })], (6,8--7,15));
                   NestedModule
                     (SynComponentInfo
                        ([], None, [], [InvalidModule],
                         PreXmlDoc ((8,12), FSharp.Compiler.Xml.XmlDocCollector),
                         false, None, (8,12--8,32)), false,
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
                                      EqualsRange = Some (8,33--8,34) })], false,
                  (5,4--9,25), { ModuleKeyword = Some (5,4--5,10)
                                 EqualsRange = Some (5,18--5,19) })], false,
              (4,0--9,25), { ModuleKeyword = Some (4,0--4,6)
                             EqualsRange = Some (4,14--4,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--9,25), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,50)] }, set []))

(8,12)-(8,18) parse error Modules cannot be nested inside types. Define modules at module or namespace level.

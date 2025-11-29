ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module With Semicolon Delimiter 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [TypeA],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,10)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,7), { BarRange = Some (5,4--5,5) })],
                        (5,4--5,7)), (5,4--5,7)), [], None, (4,5--5,7),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,11--4,12)
                    WithKeyword = None })], (4,0--5,7));
           NestedModule
             (SynComponentInfo
                ([], None, [], [ModuleAfterDelimiter],
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,31)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (7,12--7,13)),
                      None, Const (Int32 1, (7,16--7,17)), (7,12--7,13),
                      Yes (7,8--7,17), { LeadingKeyword = Let (7,8--7,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (7,14--7,15) })],
                  (7,8--7,17), { InKeyword = None })], false, (6,4--7,17),
              { ModuleKeyword = Some (6,4--6,10)
                EqualsRange = Some (6,32--6,33) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,44)] }, set []))

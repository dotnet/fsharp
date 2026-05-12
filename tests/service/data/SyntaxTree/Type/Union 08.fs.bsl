ImplFile
  (ParsedImplFileInput
     ("/root/Type/Union 08.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,7), { BarRange = Some (4,4--4,5) })],
                        (4,4--4,7)), (4,4--4,7)), [], None, (3,5--4,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,7));
           NestedModule
             (SynComponentInfo
                ([], None, [], [ThisIsFine],
                 PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,0--6,17)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([f], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (7,10--7,12)), (7,10--7,12))],
                         None, (7,8--7,12)), None, Const (Unit, (7,15--7,17)),
                      (7,8--7,12), NoneAtLet,
                      { LeadingKeyword = Let (7,4--7,7)
                        InlineKeyword = None
                        EqualsRange = Some (7,13--7,14) })], (7,4--7,17),
                  { InKeyword = None })], false, (6,0--7,17),
              { ModuleKeyword = Some (6,0--6,6)
                EqualsRange = Some (6,18--6,19) });
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
                    WithKeyword = None })], (9,0--10,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--10,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

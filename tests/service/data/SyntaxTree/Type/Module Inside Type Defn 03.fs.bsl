ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Type Defn 03.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [U],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (4,6--4,7), { BarRange = Some (4,4--4,5) });
                         SynUnionCase
                           ([], SynIdent (B, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,7), { BarRange = Some (5,4--5,5) })],
                        (4,4--5,7)), (4,4--5,7)), [], None, (3,5--5,7),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--5,7));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M3],
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,13)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([f], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (7,14--7,16)), (7,14--7,16))],
                         None, (7,12--7,16)), None, Const (Unit, (7,19--7,21)),
                      (7,12--7,16), NoneAtLet,
                      { LeadingKeyword = Let (7,8--7,11)
                        InlineKeyword = None
                        EqualsRange = Some (7,17--7,18) })], (7,8--7,21))],
              false, (6,4--7,21), { ModuleKeyword = Some (6,4--6,10)
                                    EqualsRange = Some (6,14--6,15) })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,4)-(6,10) parse error Invalid declaration syntax

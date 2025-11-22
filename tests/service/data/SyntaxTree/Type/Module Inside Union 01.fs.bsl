ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Union 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [U],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (A, None), Fields [],
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,7), { BarRange = Some (5,4--5,5) });
                         SynUnionCase
                           ([], SynIdent (B, None), Fields [],
                            PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (6,6--6,7), { BarRange = Some (6,4--6,5) })],
                        (5,4--6,7)), (5,4--6,7)), [], None, (4,5--6,7),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--6,7));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M3],
                 PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (7,4--7,13)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((8,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([[]], SynArgInfo ([], false, None)),
                         None),
                      LongIdent
                        (SynLongIdent ([f], [], [None]), None, None,
                         Pats [Paren (Const (Unit, (8,14--8,16)), (8,14--8,16))],
                         None, (8,12--8,16)), None, Const (Unit, (8,19--8,21)),
                      (8,12--8,16), NoneAtLet,
                      { LeadingKeyword = Let (8,8--8,11)
                        InlineKeyword = None
                        EqualsRange = Some (8,17--8,18) })], (8,8--8,21),
                  { InKeyword = None })], false, (7,4--8,21),
              { ModuleKeyword = Some (7,4--7,10)
                EqualsRange = Some (7,14--7,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--8,21), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,44)] }, set []))

(7,4)-(7,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.

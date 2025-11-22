ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Record 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [R],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some A,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((5,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (5,6--5,13), { LeadingKeyword = None
                                                 MutableKeyword = None })],
                        (5,4--5,15)), (5,4--5,15)), [], None, (4,5--5,15),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--5,15));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M4],
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
                        EqualsRange = Some (7,17--7,18) })], (7,8--7,21),
                  { InKeyword = None })], false, (6,4--7,21),
              { ModuleKeyword = Some (6,4--6,10)
                EqualsRange = Some (6,14--6,15) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,21), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,45)] }, set []))

(6,4)-(6,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.

ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Interface 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [IFace],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,10)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (F, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (5,17--5,27), { ArrowRange = (5,21--5,23) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (5,4--5,27),
                            { LeadingKeyword = Abstract (5,4--5,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (5,4--5,27),
                         { GetSetKeywords = None })], (5,4--5,27)), [], None,
                  (4,5--5,27), { LeadingKeyword = Type (4,0--4,4)
                                 EqualsRange = Some (4,11--4,12)
                                 WithKeyword = None })], (4,0--5,27));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M],
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,12)), false,
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
                EqualsRange = Some (6,13--6,14) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,21), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,48)] }, set []))

(6,4)-(6,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.

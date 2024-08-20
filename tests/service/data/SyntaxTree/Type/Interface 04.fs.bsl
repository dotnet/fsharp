ImplFile
  (ParsedImplFileInput
     ("/root/Type/Interface 04.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Interface,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (P, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (4,4--4,19),
                            { LeadingKeyword = Abstract (4,4--4,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (4,4--4,19),
                         { GetSetKeywords = None })], (3,9--5,3)), [], None,
                  (3,5--5,3), { LeadingKeyword = Type (3,0--3,4)
                                EqualsRange = Some (3,7--3,8)
                                WithKeyword = None })], (3,0--5,3));
           Expr (Const (Unit, (7,0--7,2)), (7,0--7,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

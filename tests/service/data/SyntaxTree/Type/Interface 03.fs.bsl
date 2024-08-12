ImplFile
  (ParsedImplFileInput
     ("/root/Type/Interface 03.fs", false, QualifiedNameOfFile Module, [], [],
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
                            PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (5,8--5,23),
                            { LeadingKeyword = Abstract (5,8--5,16)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (5,8--5,23),
                         { GetSetKeywords = None })], (4,4--6,7)), [], None,
                  (3,5--6,7), { LeadingKeyword = Type (3,0--3,4)
                                EqualsRange = Some (3,7--3,8)
                                WithKeyword = None })], (3,0--6,7));
           Expr (Const (Unit, (8,0--8,2)), (8,0--8,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--8,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

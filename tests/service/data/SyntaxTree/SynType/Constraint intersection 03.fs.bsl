ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Constraint intersection 03.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [I],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (h, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (Intersection
                                 (None,
                                  [HashConstraint
                                     (LongIdent
                                        (SynLongIdent
                                           ([IDisposable], [], [None])),
                                      (4,16--4,28));
                                   HashConstraint
                                     (App
                                        (LongIdent
                                           (SynLongIdent ([seq], [], [None])),
                                         Some (4,35--4,36),
                                         [LongIdent
                                            (SynLongIdent ([int], [], [None]))],
                                         [], Some (4,39--4,40), false,
                                         (4,32--4,40)), (4,31--4,40))],
                                  (4,16--4,40)),
                               LongIdent (SynLongIdent ([unit], [], [None])),
                               (4,16--4,48), { ArrowRange = (4,41--4,43) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (4,4--4,48),
                            { LeadingKeyword = Abstract (4,4--4,12)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (4,4--4,48),
                         { GetSetKeywords = None })], (4,4--4,48)), [], None,
                  (3,5--4,48), { LeadingKeyword = Type (3,0--3,4)
                                 EqualsRange = Some (3,7--3,8)
                                 WithKeyword = None })], (3,0--4,48))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,48), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

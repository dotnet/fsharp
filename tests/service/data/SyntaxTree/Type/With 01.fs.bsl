ImplFile
  (ParsedImplFileInput
     ("/root/Type/With 01.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel (Augmentation (3,7--3,11), [], (3,5--4,21)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]; []],
                               SynArgInfo ([], false, None)), None, None),
                         LongIdent
                           (SynLongIdent
                              ([this; P], [(4,15--4,16)], [None; None]), None,
                            None, Pats [], None, (4,11--4,17)), None,
                         Const (Int32 1, (4,20--4,21)), (4,11--4,17),
                         NoneAtInvisible, { LeadingKeyword = Member (4,4--4,10)
                                            InlineKeyword = None
                                            EqualsRange = Some (4,18--4,19) }),
                      (4,4--4,21))], None, (3,5--4,21),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = None
                    WithKeyword = None })], (3,0--4,21));
           Expr (Const (Int32 2, (6,0--6,1)), (6,0--6,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

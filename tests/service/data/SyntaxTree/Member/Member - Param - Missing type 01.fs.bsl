ImplFile
  (ParsedImplFileInput
     ("/root/Member/Member - Param - Missing type 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
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
                                 ([[SynArgInfo ([], false, None)];
                                   [SynArgInfo ([], false, Some i)]],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([this; M], [(4,15--4,16)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Typed
                                       (Named
                                          (SynIdent (i, None), false, None,
                                           (4,18--4,19)),
                                        FromParseError (4,20--4,20),
                                        (4,18--4,20)), (4,17--4,21))], None,
                               (4,11--4,21)), None, Const (Unit, (4,24--4,26)),
                            (4,11--4,21), NoneAtInvisible,
                            { LeadingKeyword = Member (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange = Some (4,22--4,23) }), (4,4--4,26))],
                     (4,4--4,26)), [], None, (3,5--4,26),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--4,26))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,26), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,20)-(4,21) parse error Unexpected symbol ')' in pattern

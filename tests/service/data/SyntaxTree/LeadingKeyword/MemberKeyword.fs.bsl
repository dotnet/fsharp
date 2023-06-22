ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/MemberKeyword.fs", false,
      QualifiedNameOfFile MemberKeyword, [], [],
      [SynModuleOrNamespace
         ([MemberKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([this; Y], [(3,15--3,16)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Const (Unit, (3,18--3,20)), (3,18--3,20))],
                               None, (3,11--3,20)), None,
                            Const (Unit, (3,24--3,26)), (3,11--3,20),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (3,4--3,10)
                              InlineKeyword = None
                              EqualsRange = Some (3,22--3,23) }), (3,4--3,26))],
                     (3,4--3,26)), [], None, (2,5--3,26),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,7--2,8)
                    WithKeyword = None })], (2,0--3,26))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

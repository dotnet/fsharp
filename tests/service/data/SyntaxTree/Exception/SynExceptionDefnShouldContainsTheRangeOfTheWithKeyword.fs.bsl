ImplFile
  (ParsedImplFileInput
     ("/root/Exception/SynExceptionDefnShouldContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile SynExceptionDefnShouldContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([X], false, DeclaredNamespace,
          [Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (Foo, None), Fields [], PreXmlDocEmpty, None,
                       (4,10--4,13), { BarRange = None }), None,
                    PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (4,0--4,13)), Some (4,14--4,18),
                 [Member
                    (SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                          (SynLongIdent ([Meh], [], [None]), None, None,
                           Pats
                             [Paren (Const (Unit, (5,15--5,17)), (5,15--5,17))],
                           None, (5,11--5,17)), None, Const (Unit, (5,20--5,22)),
                        (5,11--5,17), NoneAtInvisible,
                        { LeadingKeyword = Member (5,4--5,10)
                          InlineKeyword = None
                          EqualsRange = Some (5,18--5,19) }), (5,4--5,22))],
                 (4,0--5,22)), (4,0--5,22))], PreXmlDocEmpty, [], None,
          (2,0--5,22), { LeadingKeyword = Namespace (2,0--2,9) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

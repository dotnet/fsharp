ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInBindingOfSynExprObjExpr.fs",
      false,
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInBindingOfSynExprObjExpr, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInBindingOfSynExprObjExpr], false,
          AnonModule,
          [Expr
             (ObjExpr
                (LongIdent
                   (SynLongIdent
                      ([System; Object], [(2,12--2,13)], [None; None])),
                 Some (Const (Unit, (2,19--2,21)), None), Some (2,22--2,26), [],
                 [Member
                    (SynBinding
                       (None, Normal, false, false,
                        [{ Attributes =
                            [{ TypeName = SynLongIdent ([Foo], [], [None])
                               ArgExpr = Const (Unit, (3,6--3,9))
                               Target = None
                               AppliesToGetterAndSetter = false
                               Range = (3,6--3,9) }]
                           Range = (3,4--3,11) }],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (Some { IsInstance = true
                                  IsDispatchSlot = false
                                  IsOverrideOrExplicitImpl = true
                                  IsFinal = false
                                  GetterOrSetterIsCompilerGenerated = false
                                  MemberKind = Member },
                           SynValInfo
                             ([[SynArgInfo ([], false, None)]; []],
                              SynArgInfo ([], false, None)), None),
                        LongIdent
                          (SynLongIdent
                             ([x; ToString], [(4,12--4,13)], [None; None]), None,
                           None,
                           Pats
                             [Paren (Const (Unit, (4,21--4,23)), (4,21--4,23))],
                           None, (4,11--4,23)), None,
                        Const
                          (String ("F#", Regular, (4,26--4,30)), (4,26--4,30)),
                        (3,4--4,23), NoneAtInvisible,
                        { LeadingKeyword = Member (4,4--4,10)
                          InlineKeyword = None
                          EqualsRange = Some (4,24--4,25) }), (3,4--4,30))], [],
                 (2,2--2,21), (2,0--4,32)), (2,0--4,32))], PreXmlDocEmpty, [],
          None, (2,0--4,32), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInSynMemberDefnLetBindings.fs",
      false,
      QualifiedNameOfFile
        RangeOfAttributeShouldBeIncludedInSynMemberDefnLetBindings, [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSynMemberDefnLetBindings], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Bar],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [LetBindings
                        ([SynBinding
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
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named (SynIdent (x, None), false, None, (4,8--4,9)),
                             None, Const (Int32 8, (4,12--4,13)), (3,4--4,9),
                             Yes (3,4--4,13),
                             { LeadingKeyword = Let (4,4--4,7)
                               InlineKeyword = None
                               EqualsRange = Some (4,10--4,11) })], false, false,
                         (3,4--4,13))], (3,4--4,13)), [], None, (2,5--4,13),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,9--2,10)
                    WithKeyword = None })], (2,0--4,13))], PreXmlDocEmpty, [],
          None, (2,0--5,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

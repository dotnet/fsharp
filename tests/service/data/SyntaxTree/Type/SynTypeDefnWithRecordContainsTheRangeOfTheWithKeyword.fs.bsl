ImplFile
  (ParsedImplFileInput
     ("/root/Type/SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithRecordContainsTheRangeOfTheWithKeyword], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  Simple
                    (Record
                       (None,
                        [SynField
                           ([], false, Some Bar,
                            LongIdent (SynLongIdent ([int], [], [None])), false,
                            PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                            None, (3,6--3,15), { LeadingKeyword = None })],
                        (3,4--3,17)), (3,4--3,17)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((5,8), FSharp.Compiler.Xml.XmlDocCollector),
                         SynValData
                           (Some { IsInstance = true
                                   IsDispatchSlot = false
                                   IsOverrideOrExplicitImpl = false
                                   IsFinal = false
                                   GetterOrSetterIsCompilerGenerated = false
                                   MemberKind = Member },
                            SynValInfo
                              ([[SynArgInfo ([], false, None)];
                                [SynArgInfo ([], false, Some v)]],
                               SynArgInfo ([], false, None)), None),
                         LongIdent
                           (SynLongIdent
                              ([this; Meh], [(5,19--5,20)], [None; None]), None,
                            None,
                            Pats
                              [Paren
                                 (Typed
                                    (Named
                                       (SynIdent (v, None), false, None,
                                        (5,25--5,26)),
                                     LongIdent
                                       (SynLongIdent ([int], [], [None])),
                                     (5,25--5,30)), (5,24--5,31))], None,
                            (5,15--5,31)), None,
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_Addition], [],
                                     [Some (OriginalNotation "+")]), None,
                                  (5,43--5,44)),
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([this; Bar], [(5,38--5,39)], [None; None]),
                                  None, (5,34--5,42)), (5,34--5,44)), Ident v,
                            (5,34--5,46)), (5,15--5,31), NoneAtInvisible,
                         { LeadingKeyword = Member (5,8--5,14)
                           InlineKeyword = None
                           EqualsRange = Some (5,32--5,33) }), (5,8--5,46))],
                  None, (2,5--5,46), { LeadingKeyword = Type (2,0--2,4)
                                       EqualsRange = Some (2,9--2,10)
                                       WithKeyword = Some (4,4--4,8) })],
              (2,0--5,46))], PreXmlDocEmpty, [], None, (2,0--6,0),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

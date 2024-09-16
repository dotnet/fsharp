SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/TriviaIsPresentInSynTypeDefnSig.fsi",
      QualifiedNameOfFile Meh, [], [],
      [SynModuleOrNamespaceSig
         ([Meh], false, NamedModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynValSig
                           ([], SynIdent (a, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, Some (Const (Int32 10, (5,21--5,23))),
                            (5,4--5,23), { LeadingKeyword = Member (5,4--5,10)
                                           InlineKeyword = None
                                           WithKeyword = None
                                           EqualsRange = Some (5,19--5,20) }),
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGet }, (5,4--5,23),
                         { GetSetKeywords = None })], (5,4--5,23)), [],
                  (4,5--5,23), { LeadingKeyword = Type (4,0--4,4)
                                 EqualsRange = Some (4,7--4,8)
                                 WithKeyword = None })], (4,0--5,23));
           Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Y],
                     PreXmlDoc ((11,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (11,5--11,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (11,9--11,12)), (11,9--11,12)), [], (7,0--11,12),
                  { LeadingKeyword = Type (11,0--11,4)
                    EqualsRange = Some (11,7--11,8)
                    WithKeyword = None })], (7,0--11,12));
           Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [Z],
                     PreXmlDoc ((14,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (14,5--14,6)),
                  Simple (None (14,5--15,32), (14,5--15,32)),
                  [Member
                     (SynValSig
                        ([], SynIdent (P, None), SynValTyparDecls (None, true),
                         Fun
                           (LongIdent (SynLongIdent ([int], [], [None])),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            (15,22--15,32), { ArrowRange = (15,26--15,28) }),
                         SynValInfo
                           ([[SynArgInfo ([], false, None)]],
                            SynArgInfo ([], false, None)), false, false,
                         PreXmlDoc ((15,4), FSharp.Compiler.Xml.XmlDocCollector),
                         Single None, None, (15,4--15,32),
                         { LeadingKeyword =
                            StaticMember ((15,4--15,10), (15,11--15,17))
                           InlineKeyword = None
                           WithKeyword = None
                           EqualsRange = None }),
                      { IsInstance = false
                        IsDispatchSlot = false
                        IsOverrideOrExplicitImpl = false
                        IsFinal = false
                        GetterOrSetterIsCompilerGenerated = false
                        MemberKind = Member }, (15,4--15,32),
                      { GetSetKeywords = None })], (14,5--15,32),
                  { LeadingKeyword = Type (14,0--14,4)
                    EqualsRange = None
                    WithKeyword = Some (14,7--14,11) })], (14,0--15,32))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--15,32), { LeadingKeyword = Module (2,0--2,6) })],
      { ConditionalDirectives =
         [If (Ident "CHECK_LINE0_TYPES", (8,0--8,21)); Else (10,0--10,5);
          EndIf (12,0--12,6)]
        CodeComments = [] }, set []))

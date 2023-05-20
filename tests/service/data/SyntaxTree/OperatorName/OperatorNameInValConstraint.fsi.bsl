SigFile
  (ParsedSigFileInput
     ("/root/OperatorName/OperatorNameInValConstraint.fsi",
      QualifiedNameOfFile Operators, [], [],
      [SynModuleOrNamespaceSig
         ([Operators], false, NamedModule,
          [Val
             (SynValSig
                ([],
                 SynIdent
                   (op_UnaryNegation,
                    Some
                      (OriginalNotationWithParen
                         ((12,15--12,16), "~-", (12,18--12,19)))),
                 SynValTyparDecls (None, true),
                 WithGlobalConstraints
                   (Fun
                      (SignatureParameter
                         ([], false, Some n,
                          Var (SynTypar (T, HeadType, false), (12,24--12,26)),
                          (12,21--12,26)),
                       Var (SynTypar (T, HeadType, false), (12,30--12,32)),
                       (12,21--12,32), { ArrowRange = (12,27--12,29) }),
                    [WhereTyparSupportsMember
                       (Var (SynTypar (T, HeadType, false), (12,38--12,40)),
                        Member
                          (SynValSig
                             ([],
                              SynIdent
                                (op_UnaryNegation,
                                 Some
                                   (OriginalNotationWithParen
                                      ((12,57--12,58), "~-", (12,62--12,63)))),
                              SynValTyparDecls (None, true),
                              Fun
                                (Var
                                   (SynTypar (T, HeadType, false),
                                    (12,65--12,67)),
                                 Var
                                   (SynTypar (T, HeadType, false),
                                    (12,71--12,73)), (12,65--12,73),
                                 { ArrowRange = (12,68--12,70) }),
                              SynValInfo
                                ([[SynArgInfo ([], false, None)]],
                                 SynArgInfo ([], false, None)), false, false,
                              PreXmlDoc ((12,43), FSharp.Compiler.Xml.XmlDocCollector),
                              None, None, (12,43--12,73),
                              { LeadingKeyword =
                                 StaticMember ((12,43--12,49), (12,50--12,56))
                                InlineKeyword = None
                                WithKeyword = None
                                EqualsRange = None }),
                           { IsInstance = false
                             IsDispatchSlot = false
                             IsOverrideOrExplicitImpl = false
                             IsFinal = false
                             GetterOrSetterIsCompilerGenerated = false
                             MemberKind = Member }, (12,43--12,73),
                           { GetSetKeywords = None }), (12,38--12,74));
                     WhereTyparDefaultsToType
                       (SynTypar (T, HeadType, false),
                        LongIdent (SynLongIdent ([int], [], [None])),
                        (12,79--12,94))], (12,21--12,94)),
                 SynValInfo
                   ([[SynArgInfo ([], false, Some n)]],
                    SynArgInfo ([], false, None)), true, false,
                 PreXmlDoc ((12,4), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, (4,4--12,94), { LeadingKeyword = Val (12,4--12,7)
                                       InlineKeyword = Some (12,8--12,14)
                                       WithKeyword = None
                                       EqualsRange = None }), (4,4--12,94))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes = [{ TypeName = SynLongIdent ([AutoOpen], [], [None])
                             ArgExpr = Const (Unit, (2,2--2,10))
                             Target = None
                             AppliesToGetterAndSetter = false
                             Range = (2,2--2,10) }]
             Range = (2,0--2,12) }], None, (2,0--12,94),
          { LeadingKeyword = Module (3,4--3,10) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(12,79)-(12,94) parse error This construct is deprecated: it is only for use in the F# library

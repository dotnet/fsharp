SigFile
  (ParsedSigFileInput
     ("/root/OperatorNameInValConstraint.fsi", QualifiedNameOfFile Operators, [],
      [],
      [SynModuleOrNamespaceSig
         ([Operators], false, NamedModule,
          [Val
             (SynValSig
                ([],
                 SynIdent
                   (op_UnaryNegation,
                    Some
                      (OriginalNotationWithParen
                         (/root/OperatorNameInValConstraint.fsi (11,15--11,16),
                          "~-",
                          /root/OperatorNameInValConstraint.fsi (11,18--11,19)))),
                 SynValTyparDecls (None, true),
                 WithGlobalConstraints
                   (Fun
                      (SignatureParameter
                         ([], false, Some n,
                          Var
                            (SynTypar (T, HeadType, false),
                             /root/OperatorNameInValConstraint.fsi (11,24--11,26)),
                          /root/OperatorNameInValConstraint.fsi (11,21--11,26)),
                       Var
                         (SynTypar (T, HeadType, false),
                          /root/OperatorNameInValConstraint.fsi (11,30--11,32)),
                       /root/OperatorNameInValConstraint.fsi (11,21--11,32),
                       { ArrowRange =
                          /root/OperatorNameInValConstraint.fsi (11,27--11,29) }),
                    [WhereTyparSupportsMember
                       (Var
                          (SynTypar (T, HeadType, false),
                           /root/OperatorNameInValConstraint.fsi (11,38--11,40)),
                        Member
                          (SynValSig
                             ([],
                              SynIdent
                                (op_UnaryNegation,
                                 Some
                                   (OriginalNotationWithParen
                                      (/root/OperatorNameInValConstraint.fsi (11,57--11,58),
                                       "~-",
                                       /root/OperatorNameInValConstraint.fsi (11,62--11,63)))),
                              SynValTyparDecls (None, true),
                              Fun
                                (Var
                                   (SynTypar (T, HeadType, false),
                                    /root/OperatorNameInValConstraint.fsi (11,65--11,67)),
                                 Var
                                   (SynTypar (T, HeadType, false),
                                    /root/OperatorNameInValConstraint.fsi (11,71--11,73)),
                                 /root/OperatorNameInValConstraint.fsi (11,65--11,73),
                                 { ArrowRange =
                                    /root/OperatorNameInValConstraint.fsi (11,68--11,70) }),
                              SynValInfo
                                ([[SynArgInfo ([], false, None)]],
                                 SynArgInfo ([], false, None)), false, false,
                              PreXmlDoc ((11,43), FSharp.Compiler.Xml.XmlDocCollector),
                              None, None,
                              /root/OperatorNameInValConstraint.fsi (11,43--11,73),
                              { LeadingKeyword =
                                 StaticMember
                                   (/root/OperatorNameInValConstraint.fsi (11,43--11,49),
                                    /root/OperatorNameInValConstraint.fsi (11,50--11,56))
                                InlineKeyword = None
                                WithKeyword = None
                                EqualsRange = None }),
                           { IsInstance = false
                             IsDispatchSlot = false
                             IsOverrideOrExplicitImpl = false
                             IsFinal = false
                             GetterOrSetterIsCompilerGenerated = false
                             MemberKind = Member },
                           /root/OperatorNameInValConstraint.fsi (11,43--11,73),
                           { GetSetKeywords = None }),
                        /root/OperatorNameInValConstraint.fsi (11,38--11,74));
                     WhereTyparDefaultsToType
                       (SynTypar (T, HeadType, false),
                        LongIdent (SynLongIdent ([int], [], [None])),
                        /root/OperatorNameInValConstraint.fsi (11,79--11,94))],
                    /root/OperatorNameInValConstraint.fsi (11,21--11,94)),
                 SynValInfo
                   ([[SynArgInfo ([], false, Some n)]],
                    SynArgInfo ([], false, None)), true, false,
                 PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, /root/OperatorNameInValConstraint.fsi (3,4--11,94),
                 { LeadingKeyword =
                    Val /root/OperatorNameInValConstraint.fsi (11,4--11,7)
                   InlineKeyword =
                    Some /root/OperatorNameInValConstraint.fsi (11,8--11,14)
                   WithKeyword = None
                   EqualsRange = None }),
              /root/OperatorNameInValConstraint.fsi (3,4--11,94))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes =
              [{ TypeName = SynLongIdent ([AutoOpen], [], [None])
                 ArgExpr =
                  Const
                    (Unit, /root/OperatorNameInValConstraint.fsi (1,2--1,10))
                 Target = None
                 AppliesToGetterAndSetter = false
                 Range = /root/OperatorNameInValConstraint.fsi (1,2--1,10) }]
             Range = /root/OperatorNameInValConstraint.fsi (1,0--1,12) }], None,
          /root/OperatorNameInValConstraint.fsi (1,0--11,94),
          { LeadingKeyword =
             Module /root/OperatorNameInValConstraint.fsi (2,4--2,10) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
ImplFile
  (ParsedImplFileInput
     ("/root/Type/NamedParametersInDelegateType.fs", false,
      QualifiedNameOfFile NamedParametersInDelegateType, [], [],
      [SynModuleOrNamespace
         ([NamedParametersInDelegateType], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Delegate
                       (Fun
                          (Tuple
                             (false,
                              [Type
                                 (SignatureParameter
                                    ([], false, Some a,
                                     LongIdent (SynLongIdent ([A], [], [None])),
                                     (2,23--2,27))); Star (2,28--2,29);
                               Type
                                 (SignatureParameter
                                    ([], false, Some b,
                                     LongIdent (SynLongIdent ([B], [], [None])),
                                     (2,30--2,34)))], (2,23--2,34)),
                           Fun
                             (SignatureParameter
                                ([], false, Some c,
                                 LongIdent (SynLongIdent ([C], [], [None])),
                                 (2,38--2,41)),
                              LongIdent (SynLongIdent ([D], [], [None])),
                              (2,38--2,46), { ArrowRange = (2,42--2,44) }),
                           (2,23--2,46), { ArrowRange = (2,35--2,37) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, Some a);
                             SynArgInfo ([], false, Some b)];
                            [SynArgInfo ([], false, Some c)]],
                           SynArgInfo ([], false, None))),
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (Invoke, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (Tuple
                                 (false,
                                  [Type
                                     (SignatureParameter
                                        ([], false, Some a,
                                         LongIdent
                                           (SynLongIdent ([A], [], [None])),
                                         (2,23--2,27))); Star (2,28--2,29);
                                   Type
                                     (SignatureParameter
                                        ([], false, Some b,
                                         LongIdent
                                           (SynLongIdent ([B], [], [None])),
                                         (2,30--2,34)))], (2,23--2,34)),
                               Fun
                                 (SignatureParameter
                                    ([], false, Some c,
                                     LongIdent (SynLongIdent ([C], [], [None])),
                                     (2,38--2,41)),
                                  LongIdent (SynLongIdent ([D], [], [None])),
                                  (2,38--2,46), { ArrowRange = (2,42--2,44) }),
                               (2,23--2,46), { ArrowRange = (2,35--2,37) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, Some a);
                                 SynArgInfo ([], false, Some b)];
                                [SynArgInfo ([], false, Some c)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDocEmpty, Single None, None, (2,11--2,46),
                            { LeadingKeyword = Synthetic
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (2,11--2,46),
                         { GetSetKeywords = None })], (2,11--2,46)), [], None,
                  (2,5--2,46), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,9--2,10)
                                 WithKeyword = None })], (2,0--2,46))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

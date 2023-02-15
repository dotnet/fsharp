SigFile
  (ParsedSigFileInput
     ("/root/SynType/SynTypeTupleDoesIncludeLeadingParameterName.fsi",
      QualifiedNameOfFile SynTypeTupleDoesIncludeLeadingParameterName, [], [],
      [SynModuleOrNamespaceSig
         ([SynTypeTupleDoesIncludeLeadingParameterName], false, AnonModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynValSig
                           ([], SynIdent (M, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (Tuple
                                 (false,
                                  [Type
                                     (SignatureParameter
                                        ([], false, Some p1,
                                         LongIdent
                                           (SynLongIdent ([a], [], [None])),
                                         (3,14--3,19))); Star (3,20--3,21);
                                   Type
                                     (SignatureParameter
                                        ([], false, Some p2,
                                         LongIdent
                                           (SynLongIdent ([b], [], [None])),
                                         (3,22--3,27)))], (3,14--3,27)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,14--3,34), { ArrowRange = (3,28--3,30) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, Some p1);
                                 SynArgInfo ([], false, Some p2)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (3,4--3,34),
                            { LeadingKeyword = Member (3,4--3,10)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (3,4--3,34),
                         { GetSetKeywords = None })], (3,4--3,34)), [],
                  (2,5--3,34), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,34))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

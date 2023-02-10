SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/SynValSigContainsParameterNames.fsi",
      QualifiedNameOfFile Meh, [], [],
      [SynModuleOrNamespaceSig
         ([Meh], false, NamedModule,
          [Val
             (SynValSig
                ([], SynIdent (InferSynValData, None),
                 SynValTyparDecls (None, true),
                 Fun
                   (Tuple
                      (false,
                       [Type
                          (SignatureParameter
                             ([], false, Some memberFlagsOpt,
                              App
                                (LongIdent (SynLongIdent ([option], [], [None])),
                                 None,
                                 [LongIdent
                                    (SynLongIdent ([SynMemberFlags], [], [None]))],
                                 [], None, true,
                                 /root/SignatureType/SynValSigContainsParameterNames.fsi (4,20--4,41)),
                              /root/SignatureType/SynValSigContainsParameterNames.fsi (4,4--4,41)));
                        Star
                          /root/SignatureType/SynValSigContainsParameterNames.fsi (4,42--4,43);
                        Type
                          (SignatureParameter
                             ([], false, Some pat,
                              App
                                (LongIdent (SynLongIdent ([option], [], [None])),
                                 None,
                                 [LongIdent
                                    (SynLongIdent ([SynPat], [], [None]))], [],
                                 None, true,
                                 /root/SignatureType/SynValSigContainsParameterNames.fsi (4,49--4,62)),
                              /root/SignatureType/SynValSigContainsParameterNames.fsi (4,44--4,62)));
                        Star
                          /root/SignatureType/SynValSigContainsParameterNames.fsi (4,63--4,64);
                        Type
                          (App
                             (LongIdent (SynLongIdent ([option], [], [None])),
                              None,
                              [LongIdent
                                 (SynLongIdent ([SynReturnInfo], [], [None]))],
                              [], None, true,
                              /root/SignatureType/SynValSigContainsParameterNames.fsi (4,65--4,85)));
                        Star
                          /root/SignatureType/SynValSigContainsParameterNames.fsi (4,86--4,87);
                        Type
                          (SignatureParameter
                             ([], false, Some origRhsExpr,
                              LongIdent (SynLongIdent ([SynExpr], [], [None])),
                              /root/SignatureType/SynValSigContainsParameterNames.fsi (4,88--4,108)))],
                       /root/SignatureType/SynValSigContainsParameterNames.fsi (4,4--4,108)),
                    Fun
                      (SignatureParameter
                         ([], false, Some x,
                          LongIdent (SynLongIdent ([string], [], [None])),
                          /root/SignatureType/SynValSigContainsParameterNames.fsi (5,8--5,17)),
                       LongIdent (SynLongIdent ([SynValData2], [], [None])),
                       /root/SignatureType/SynValSigContainsParameterNames.fsi (5,8--6,23),
                       { ArrowRange =
                          /root/SignatureType/SynValSigContainsParameterNames.fsi (5,18--5,20) }),
                    /root/SignatureType/SynValSigContainsParameterNames.fsi (4,4--6,23),
                    { ArrowRange =
                       /root/SignatureType/SynValSigContainsParameterNames.fsi (4,109--4,111) }),
                 SynValInfo
                   ([[SynArgInfo ([], false, Some memberFlagsOpt);
                      SynArgInfo ([], false, Some pat);
                      SynArgInfo ([], false, None);
                      SynArgInfo ([], false, Some origRhsExpr)];
                     [SynArgInfo ([], false, Some x)]],
                    SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None,
                 /root/SignatureType/SynValSigContainsParameterNames.fsi (3,0--6,23),
                 { LeadingKeyword =
                    Val
                      /root/SignatureType/SynValSigContainsParameterNames.fsi (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/SignatureType/SynValSigContainsParameterNames.fsi (3,0--6,23))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/SignatureType/SynValSigContainsParameterNames.fsi (2,0--6,23),
          { LeadingKeyword =
             Module
               /root/SignatureType/SynValSigContainsParameterNames.fsi (2,0--2,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

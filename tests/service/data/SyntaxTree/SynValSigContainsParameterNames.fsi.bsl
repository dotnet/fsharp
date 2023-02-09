SigFile
  (ParsedSigFileInput
     ("/root/SynValSigContainsParameterNames.fsi", QualifiedNameOfFile Meh, [],
      [],
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
                                 /root/SynValSigContainsParameterNames.fsi (3,20--3,41)),
                              /root/SynValSigContainsParameterNames.fsi (3,4--3,41)));
                        Star
                          /root/SynValSigContainsParameterNames.fsi (3,42--3,43);
                        Type
                          (SignatureParameter
                             ([], false, Some pat,
                              App
                                (LongIdent (SynLongIdent ([option], [], [None])),
                                 None,
                                 [LongIdent
                                    (SynLongIdent ([SynPat], [], [None]))], [],
                                 None, true,
                                 /root/SynValSigContainsParameterNames.fsi (3,49--3,62)),
                              /root/SynValSigContainsParameterNames.fsi (3,44--3,62)));
                        Star
                          /root/SynValSigContainsParameterNames.fsi (3,63--3,64);
                        Type
                          (App
                             (LongIdent (SynLongIdent ([option], [], [None])),
                              None,
                              [LongIdent
                                 (SynLongIdent ([SynReturnInfo], [], [None]))],
                              [], None, true,
                              /root/SynValSigContainsParameterNames.fsi (3,65--3,85)));
                        Star
                          /root/SynValSigContainsParameterNames.fsi (3,86--3,87);
                        Type
                          (SignatureParameter
                             ([], false, Some origRhsExpr,
                              LongIdent (SynLongIdent ([SynExpr], [], [None])),
                              /root/SynValSigContainsParameterNames.fsi (3,88--3,108)))],
                       /root/SynValSigContainsParameterNames.fsi (3,4--3,108)),
                    Fun
                      (SignatureParameter
                         ([], false, Some x,
                          LongIdent (SynLongIdent ([string], [], [None])),
                          /root/SynValSigContainsParameterNames.fsi (4,8--4,17)),
                       LongIdent (SynLongIdent ([SynValData2], [], [None])),
                       /root/SynValSigContainsParameterNames.fsi (4,8--5,23),
                       { ArrowRange =
                          /root/SynValSigContainsParameterNames.fsi (4,18--4,20) }),
                    /root/SynValSigContainsParameterNames.fsi (3,4--5,23),
                    { ArrowRange =
                       /root/SynValSigContainsParameterNames.fsi (3,109--3,111) }),
                 SynValInfo
                   ([[SynArgInfo ([], false, Some memberFlagsOpt);
                      SynArgInfo ([], false, Some pat);
                      SynArgInfo ([], false, None);
                      SynArgInfo ([], false, Some origRhsExpr)];
                     [SynArgInfo ([], false, Some x)]],
                    SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, /root/SynValSigContainsParameterNames.fsi (2,0--5,23),
                 { LeadingKeyword =
                    Val /root/SynValSigContainsParameterNames.fsi (2,0--2,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/SynValSigContainsParameterNames.fsi (2,0--5,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/SynValSigContainsParameterNames.fsi (1,0--5,23),
          { LeadingKeyword =
             Module /root/SynValSigContainsParameterNames.fsi (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
SigFile
  (ParsedSigFileInput
     ("/root/OperatorNameInSynValSig.fsi",
      QualifiedNameOfFile IntrinsicOperators, [], [],
      [SynModuleOrNamespaceSig
         ([IntrinsicOperators], false, NamedModule,
          [Val
             (SynValSig
                ([],
                 SynIdent
                   (op_Amp,
                    Some
                      (OriginalNotationWithParen
                         (/root/OperatorNameInSynValSig.fsi (2,4--2,5), "&",
                          /root/OperatorNameInSynValSig.fsi (2,6--2,7)))),
                 SynValTyparDecls (None, true),
                 Fun
                   (SignatureParameter
                      ([], false, Some e1,
                       LongIdent (SynLongIdent ([bool], [], [None])),
                       /root/OperatorNameInSynValSig.fsi (2,9--2,17)),
                    Fun
                      (SignatureParameter
                         ([], false, Some e2,
                          LongIdent (SynLongIdent ([bool], [], [None])),
                          /root/OperatorNameInSynValSig.fsi (2,21--2,29)),
                       LongIdent (SynLongIdent ([bool], [], [None])),
                       /root/OperatorNameInSynValSig.fsi (2,21--2,37),
                       { ArrowRange =
                          /root/OperatorNameInSynValSig.fsi (2,30--2,32) }),
                    /root/OperatorNameInSynValSig.fsi (2,9--2,37),
                    { ArrowRange =
                       /root/OperatorNameInSynValSig.fsi (2,18--2,20) }),
                 SynValInfo
                   ([[SynArgInfo ([], false, Some e1)];
                     [SynArgInfo ([], false, Some e2)]],
                    SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, /root/OperatorNameInSynValSig.fsi (2,0--2,37),
                 { LeadingKeyword =
                    Val /root/OperatorNameInSynValSig.fsi (2,0--2,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/OperatorNameInSynValSig.fsi (2,0--2,37))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/OperatorNameInSynValSig.fsi (1,0--2,37),
          { LeadingKeyword = Module /root/OperatorNameInSynValSig.fsi (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
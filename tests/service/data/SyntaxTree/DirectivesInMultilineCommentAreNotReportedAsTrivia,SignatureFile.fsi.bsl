SigFile
  (ParsedSigFileInput
     ("/root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi",
      QualifiedNameOfFile
        DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (v, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some
                   (Const
                      (Int32 42,
                       /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (9,0--9,2))),
                 /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (3,0--9,2),
                 { LeadingKeyword =
                    Val
                      /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (3,12--3,13) }),
              /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (3,0--9,2))],
          PreXmlDocEmpty, [], None,
          /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (1,0--9,2),
          { LeadingKeyword =
             Namespace
               /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (1,0--1,9) })],
      { ConditionalDirectives = []
        CodeComments =
         [BlockComment
            /root/DirectivesInMultilineCommentAreNotReportedAsTrivia,SignatureFile.fsi (4,0--8,2)] },
      set []))
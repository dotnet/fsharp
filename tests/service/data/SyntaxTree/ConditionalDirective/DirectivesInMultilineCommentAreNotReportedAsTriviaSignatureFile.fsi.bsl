SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi",
      QualifiedNameOfFile
        DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (v, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some
                   (Const
                      (Int32 42,
                       /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (10,0--10,2))),
                 /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (4,0--10,2),
                 { LeadingKeyword =
                    Val
                      /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (4,12--4,13) }),
              /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (4,0--10,2))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (2,0--10,2),
          { LeadingKeyword =
             Namespace
               /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives = []
        CodeComments =
         [BlockComment
            /root/ConditionalDirective/DirectivesInMultilineCommentAreNotReportedAsTriviaSignatureFile.fsi (5,0--9,2)] },
      set []))

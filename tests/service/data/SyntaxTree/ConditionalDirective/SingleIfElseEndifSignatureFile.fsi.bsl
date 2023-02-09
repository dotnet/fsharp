SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi",
      QualifiedNameOfFile SingleIfElseEndifSignatureFile, [], [],
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
                       /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (8,4--8,6))),
                 /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (4,0--8,6),
                 { LeadingKeyword =
                    Val
                      /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (4,12--4,13) }),
              /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (4,0--8,6))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (2,0--8,6),
          { LeadingKeyword =
             Namespace
               /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives =
         [If
            (Ident "DEBUG",
             /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (5,4--5,13));
          Else
            /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (7,4--7,9);
          EndIf
            /root/ConditionalDirective/SingleIfElseEndifSignatureFile.fsi (9,4--9,10)]
        CodeComments = [] }, set []))
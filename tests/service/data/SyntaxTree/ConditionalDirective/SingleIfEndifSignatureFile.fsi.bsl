SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/SingleIfEndifSignatureFile.fsi",
      QualifiedNameOfFile SingleIfEndifSignatureFile, [], [],
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
                       /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (8,4--8,6))),
                 /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (4,0--8,6),
                 { LeadingKeyword =
                    Val
                      /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (4,11--4,12) }),
              /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (4,0--8,6))],
          PreXmlDocEmpty, [], None,
          /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (2,0--8,6),
          { LeadingKeyword =
             Namespace
               /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives =
         [If
            (Ident "DEBUG",
             /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (5,4--5,13));
          EndIf
            /root/ConditionalDirective/SingleIfEndifSignatureFile.fsi (7,4--7,10)]
        CodeComments = [] }, set []))
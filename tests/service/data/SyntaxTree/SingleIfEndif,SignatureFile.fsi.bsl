SigFile
  (ParsedSigFileInput
     ("/root/SingleIfEndif,SignatureFile.fsi",
      QualifiedNameOfFile SingleIfEndif,SignatureFile, [], [],
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
                       /root/SingleIfEndif,SignatureFile.fsi (8,4--8,6))),
                 /root/SingleIfEndif,SignatureFile.fsi (4,0--8,6),
                 { LeadingKeyword =
                    Val /root/SingleIfEndif,SignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some /root/SingleIfEndif,SignatureFile.fsi (4,11--4,12) }),
              /root/SingleIfEndif,SignatureFile.fsi (4,0--8,6))], PreXmlDocEmpty,
          [], None, /root/SingleIfEndif,SignatureFile.fsi (2,0--8,6),
          { LeadingKeyword =
             Namespace /root/SingleIfEndif,SignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives =
         [If (Ident "DEBUG", /root/SingleIfEndif,SignatureFile.fsi (5,4--5,13));
          EndIf /root/SingleIfEndif,SignatureFile.fsi (7,4--7,10)]
        CodeComments = [] }, set []))
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
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some
                   (Const
                      (Int32 42,
                       /root/SingleIfEndif,SignatureFile.fsi (7,4--7,6))),
                 /root/SingleIfEndif,SignatureFile.fsi (3,0--7,6),
                 { LeadingKeyword =
                    Val /root/SingleIfEndif,SignatureFile.fsi (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some /root/SingleIfEndif,SignatureFile.fsi (3,11--3,12) }),
              /root/SingleIfEndif,SignatureFile.fsi (3,0--7,6))], PreXmlDocEmpty,
          [], None, /root/SingleIfEndif,SignatureFile.fsi (1,0--7,6),
          { LeadingKeyword =
             Namespace /root/SingleIfEndif,SignatureFile.fsi (1,0--1,9) })],
      { ConditionalDirectives =
         [If (Ident "DEBUG", /root/SingleIfEndif,SignatureFile.fsi (4,4--4,13));
          EndIf /root/SingleIfEndif,SignatureFile.fsi (6,4--6,10)]
        CodeComments = [] }, set []))
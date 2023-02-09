SigFile
  (ParsedSigFileInput
     ("/root/SingleIfElseEndif,SignatureFile.fsi",
      QualifiedNameOfFile SingleIfElseEndif,SignatureFile, [], [],
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
                       /root/SingleIfElseEndif,SignatureFile.fsi (7,4--7,6))),
                 /root/SingleIfElseEndif,SignatureFile.fsi (3,0--7,6),
                 { LeadingKeyword =
                    Val /root/SingleIfElseEndif,SignatureFile.fsi (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some /root/SingleIfElseEndif,SignatureFile.fsi (3,12--3,13) }),
              /root/SingleIfElseEndif,SignatureFile.fsi (3,0--7,6))],
          PreXmlDocEmpty, [], None,
          /root/SingleIfElseEndif,SignatureFile.fsi (1,0--7,6),
          { LeadingKeyword =
             Namespace /root/SingleIfElseEndif,SignatureFile.fsi (1,0--1,9) })],
      { ConditionalDirectives =
         [If
            (Ident "DEBUG",
             /root/SingleIfElseEndif,SignatureFile.fsi (4,4--4,13));
          Else /root/SingleIfElseEndif,SignatureFile.fsi (6,4--6,9);
          EndIf /root/SingleIfElseEndif,SignatureFile.fsi (8,4--8,10)]
        CodeComments = [] }, set []))
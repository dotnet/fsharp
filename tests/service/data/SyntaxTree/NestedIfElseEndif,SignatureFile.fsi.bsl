SigFile
  (ParsedSigFileInput
     ("/root/NestedIfElseEndif,SignatureFile.fsi",
      QualifiedNameOfFile NestedIfElseEndif,SignatureFile, [], [],
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
                      (Int32 3,
                       /root/NestedIfElseEndif,SignatureFile.fsi (11,8--11,9))),
                 /root/NestedIfElseEndif,SignatureFile.fsi (3,0--11,9),
                 { LeadingKeyword =
                    Val /root/NestedIfElseEndif,SignatureFile.fsi (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some /root/NestedIfElseEndif,SignatureFile.fsi (3,12--3,13) }),
              /root/NestedIfElseEndif,SignatureFile.fsi (3,0--11,9))],
          PreXmlDocEmpty, [], None,
          /root/NestedIfElseEndif,SignatureFile.fsi (1,0--11,9),
          { LeadingKeyword =
             Namespace /root/NestedIfElseEndif,SignatureFile.fsi (1,0--1,9) })],
      { ConditionalDirectives =
         [If
            (Ident "FOO", /root/NestedIfElseEndif,SignatureFile.fsi (4,4--4,11));
          If
            (Ident "MEH", /root/NestedIfElseEndif,SignatureFile.fsi (5,8--5,15));
          Else /root/NestedIfElseEndif,SignatureFile.fsi (7,8--7,13);
          EndIf /root/NestedIfElseEndif,SignatureFile.fsi (9,8--9,14);
          Else /root/NestedIfElseEndif,SignatureFile.fsi (10,4--10,9);
          EndIf /root/NestedIfElseEndif,SignatureFile.fsi (12,4--12,10)]
        CodeComments = [] }, set []))
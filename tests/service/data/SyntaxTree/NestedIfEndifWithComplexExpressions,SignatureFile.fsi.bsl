SigFile
  (ParsedSigFileInput
     ("/root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi",
      QualifiedNameOfFile NestedIfEndifWithComplexExpressions,SignatureFile, [],
      [],
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
                      (Int32 10,
                       /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (12,4--12,6))),
                 /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (4,0--12,6),
                 { LeadingKeyword =
                    Val
                      /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (4,12--4,13) }),
              /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (4,0--12,6))],
          PreXmlDocEmpty, [], None,
          /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (2,0--12,6),
          { LeadingKeyword =
             Namespace
               /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (2,0--2,9) })],
      { ConditionalDirectives =
         [If
            (Not (Ident "DEBUG"),
             /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (5,4--5,14));
          If
            (And (Ident "FOO", Ident "BAR"),
             /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (6,8--6,22));
          If
            (Or (Ident "MEH", Ident "HMM"),
             /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (7,12--7,26));
          EndIf
            /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (9,12--9,18);
          EndIf
            /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (10,8--10,14);
          EndIf
            /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (11,4--11,10)]
        CodeComments = [] }, set []))
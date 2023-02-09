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
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 Some
                   (Const
                      (Int32 10,
                       /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (11,4--11,6))),
                 /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (3,0--11,6),
                 { LeadingKeyword =
                    Val
                      /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange =
                    Some
                      /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (3,12--3,13) }),
              /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (3,0--11,6))],
          PreXmlDocEmpty, [], None,
          /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (1,0--11,6),
          { LeadingKeyword =
             Namespace
               /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (1,0--1,9) })],
      { ConditionalDirectives =
         [If
            (Not (Ident "DEBUG"),
             /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (4,4--4,14));
          If
            (And (Ident "FOO", Ident "BAR"),
             /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (5,8--5,22));
          If
            (Or (Ident "MEH", Ident "HMM"),
             /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (6,12--6,26));
          EndIf
            /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (8,12--8,18);
          EndIf
            /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (9,8--9,14);
          EndIf
            /root/NestedIfEndifWithComplexExpressions,SignatureFile.fsi (10,4--10,10)]
        CodeComments = [] }, set []))
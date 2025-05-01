SigFile
  (ParsedSigFileInput
     ("/root/ConditionalDirective/NestedIfElseEndifSignatureFile.fsi",
      QualifiedNameOfFile NestedIfElseEndifSignatureFile, [], [],
      [SynModuleOrNamespaceSig
         ([Foobar], false, DeclaredNamespace,
          [Val
             (SynValSig
                ([], SynIdent (v, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, Some (Const (Int32 3, (12,8--12,9))), (4,0--12,9),
                 { LeadingKeyword = Val (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = Some (4,12--4,13) }), (4,0--12,9))],
          PreXmlDocEmpty, [], None, (2,0--12,9),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives =
         [If (Ident "FOO", (5,4--5,11)); If (Ident "MEH", (6,8--6,15));
          Else (8,8--8,13); EndIf (10,8--10,14); Else (11,4--11,9);
          EndIf (13,4--13,10)]
        CodeComments = [] }, set []))

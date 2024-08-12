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
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, Some (Const (Int32 42, (8,4--8,6))), (4,0--8,6),
                 { LeadingKeyword = Val (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = Some (4,12--4,13) }), (4,0--8,6))],
          PreXmlDocEmpty, [], None, (2,0--8,6),
          { LeadingKeyword = Namespace (2,0--2,9) })],
      { ConditionalDirectives =
         [If (Ident "DEBUG", (5,4--5,13)); Else (7,4--7,9); EndIf (9,4--9,10)]
        CodeComments = [] }, set []))

SigFile
  (ParsedSigFileInput
     ("/root/ModuleMember/Val 01.fsi", QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespaceSig
         ([Module], false, NamedModule,
          [Val
             (SynValSig
                ([], SynIdent (f, None), SynValTyparDecls (None, true),
                 Fun
                   (Tuple
                      (false,
                       [Type (LongIdent (SynLongIdent ([int], [], [None])));
                        Star (3,11--3,12); Type (FromParseError (3,12--3,12))],
                       (3,7--3,12)),
                    LongIdent (SynLongIdent ([unit], [], [None])), (3,7--3,20),
                    { ArrowRange = (3,13--3,15) }),
                 SynValInfo
                   ([[SynArgInfo ([], false, None); SynArgInfo ([], false, None)]],
                    SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                 Single None, None, (3,0--3,20),
                 { LeadingKeyword = Val (3,0--3,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }), (3,0--3,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,20), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,13)-(3,15) parse error Unexpected symbol '->' in value signature

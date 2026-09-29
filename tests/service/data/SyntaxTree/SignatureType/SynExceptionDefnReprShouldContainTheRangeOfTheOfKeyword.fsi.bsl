SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/SynExceptionDefnReprShouldContainTheRangeOfTheOfKeyword.fsi",
      QualifiedNameOfFile X, [],
      [SynModuleOrNamespaceSig
         ([X], false, NamedModule,
          [Exception
             (SynExceptionSig
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (LoadedSourceNotFoundIgnoring, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([string], [], [None])),
                             false,
                             PreXmlDoc ((3,55), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (3,55--3,61), { LeadingKeyword = None
                                                   MutableKeyword = None });
                          SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([range], [], [None])),
                             false,
                             PreXmlDoc ((3,64), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (3,64--3,69), { LeadingKeyword = None
                                                   MutableKeyword = None })],
                       PreXmlDocEmpty, None, (3,10--3,69),
                       { BarRange = None
                         OfKeyword = Some (3,39--3,41) }), None,
                    PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (3,0--3,69), { ExceptionKeyword = (3,0--3,9) }), None, [],
                 (3,0--3,69)), (3,0--3,69))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,69), { LeadingKeyword = Module (1,0--1,6) })],
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [BlockComment (3,42--3,54)] }, set []))

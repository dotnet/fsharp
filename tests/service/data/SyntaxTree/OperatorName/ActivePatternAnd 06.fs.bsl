ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternAnd 06.fs", false,
      QualifiedNameOfFile ActivePatternAnd 06, [], [],
      [SynModuleOrNamespace
         ([ActivePatternAnd 06], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (Or
                       (Ands
                          ([Paren
                              (Typed
                                 (Wild (1,6--1,7),
                                  LongIdent (SynLongIdent ([exn], [], [None])),
                                  (1,6--1,13)), (1,5--1,14));
                            Paren
                              (LongIdent
                                 (SynLongIdent ([Failure], [], [None]), None,
                                  None, Pats [Wild (1,26--1,27)], None,
                                  (1,18--1,27)), (1,17--1,28))], (1,5--1,28)),
                        Paren (Wild (1,32--1,33), (1,31--1,34)), (1,5--1,34),
                        { BarRange = (1,29--1,30) }), (1,4--1,35)), None,
                  App
                    (NonAtomic, false, Ident exn, Const (Unit, (1,42--1,44)),
                     (1,38--1,44)), (1,4--1,35), Yes (1,0--1,44),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,36--1,37) })], (1,0--1,44))],
          PreXmlDocEmpty, [], None, (1,0--1,44), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,44) parse warning The declarations in this file will be placed in an implicit module 'ActivePatternAnd 06' based on the file name 'ActivePatternAnd 06.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

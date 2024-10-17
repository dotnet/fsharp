ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternAnd 08.fs", false,
      QualifiedNameOfFile ActivePatternAnd 08, [], [],
      [SynModuleOrNamespace
         ([ActivePatternAnd 08], false, AnonModule,
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
                            LongIdent
                              (SynLongIdent ([Failure], [], [None]), None, None,
                               Pats [Wild (1,25--1,26)], None, (1,17--1,26))],
                           (1,5--1,26)), Paren (Wild (1,30--1,31), (1,29--1,32)),
                        (1,5--1,32), { BarRange = (1,27--1,28) }), (1,4--1,33)),
                  None,
                  App
                    (NonAtomic, false, Ident exn, Const (Unit, (1,40--1,42)),
                     (1,36--1,42)), (1,4--1,33), Yes (1,0--1,42),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,34--1,35) })], (1,0--1,42))],
          PreXmlDocEmpty, [], None, (1,0--1,42), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,42) parse warning The declarations in this file will be placed in an implicit module 'ActivePatternAnd 08' based on the file name 'ActivePatternAnd 08.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

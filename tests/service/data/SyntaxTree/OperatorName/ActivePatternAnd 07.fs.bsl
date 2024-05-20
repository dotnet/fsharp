ImplFile
  (ParsedImplFileInput
     ("/root/OperatorName/ActivePatternAnd 07.fs", false,
      QualifiedNameOfFile ActivePatternAnd 07, [], [],
      [SynModuleOrNamespace
         ([ActivePatternAnd 07], false, AnonModule,
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
                          ([Typed
                              (Wild (1,5--1,6),
                               LongIdent (SynLongIdent ([exn], [], [None])),
                               (1,5--1,12));
                            Paren
                              (LongIdent
                                 (SynLongIdent ([Failure], [], [None]), None,
                                  None, Pats [Wild (1,24--1,25)], None,
                                  (1,16--1,25)), (1,15--1,26))], (1,5--1,26)),
                        Wild (1,29--1,30), (1,5--1,30),
                        { BarRange = (1,27--1,28) }), (1,4--1,31)), None,
                  App
                    (NonAtomic, false, Ident exn, Const (Unit, (1,38--1,40)),
                     (1,34--1,40)), (1,4--1,31), Yes (1,0--1,40),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,32--1,33) })], (1,0--1,40))],
          PreXmlDocEmpty, [], None, (1,0--1,40), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,40) parse warning The declarations in this file will be placed in an implicit module 'ActivePatternAnd 07' based on the file name 'ActivePatternAnd 07.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

ImplFile
  (ParsedImplFileInput
     ("/root/Measure/Constant - 04.fs", false, QualifiedNameOfFile Constant - 04,
      [], [],
      [SynModuleOrNamespace
         ([Constant - 04], false, AnonModule,
          [Expr
             (Const
                (Measure
                   (Int32 23, (1,0--1,2),
                    Divide
                      (None, Seq ([Named ([cm], (1,4--1,6))], (1,4--1,6)),
                       (1,3--1,6)), { LessRange = (1,2--1,3)
                                      GreaterRange = (1,6--1,7) }), (1,0--1,7)),
              (1,0--1,7))], PreXmlDocEmpty, [], None, (1,0--1,7),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(1,7) parse warning The declarations in this file will be placed in an implicit module 'Constant - 04' based on the file name 'Constant - 04.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

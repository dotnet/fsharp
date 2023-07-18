ImplFile
  (ParsedImplFileInput
     ("/root/Measure/Constant - 02.fs", false, QualifiedNameOfFile Constant - 02,
      [], [],
      [SynModuleOrNamespace
         ([Constant - 02], false, AnonModule,
          [Expr
             (Const
                (Measure
                   (Int32 23, (1,0--1,2),
                    Seq ([Named ([cm], (1,5--1,7))], (1,5--1,7)),
                    { LessRange = (1,2--1,3)
                      GreaterRange = (1,9--1,10) }), (1,0--1,10)), (1,0--1,10))],
          PreXmlDocEmpty, [], None, (1,0--1,10), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,10) parse warning The declarations in this file will be placed in an implicit module 'Constant - 02' based on the file name 'Constant - 02.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

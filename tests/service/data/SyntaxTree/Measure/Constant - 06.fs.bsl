ImplFile
  (ParsedImplFileInput
     ("/root/Measure/Constant - 06.fs", false, QualifiedNameOfFile Constant - 06,
      [], [],
      [SynModuleOrNamespace
         ([Constant - 06], false, AnonModule,
          [Expr
             (Const
                (Measure
                   (Int32 42, (1,0--1,2),
                    Seq
                      ([Power
                          (Named ([m], (1,3--1,4)), Integer (12345, (1,5--1,10)),
                           (1,3--1,10))], (1,3--1,10)),
                    { LessRange = (1,2--1,3)
                      GreaterRange = (1,10--1,11) }), (1,0--1,11)), (1,0--1,11))],
          PreXmlDocEmpty, [], None, (1,0--1,11), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(1,0)-(1,11) parse warning The declarations in this file will be placed in an implicit module 'Constant - 06' based on the file name 'Constant - 06.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

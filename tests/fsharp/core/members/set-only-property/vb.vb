namespace basic
    public class BasicClass
       dim v as integer
       public property Prop1 as integer
          private get
              return v
          end get
          set(value as integer)
               v = value
          end set
       end property
       public property Prop2 as integer
           get
               return v
           end get
           private set(value as integer)
               v = value
           end set
       end property
    end class
end namespace
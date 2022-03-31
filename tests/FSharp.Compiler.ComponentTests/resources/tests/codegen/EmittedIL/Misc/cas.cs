using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;

namespace CustomSecAttr
{
    [SerializableAttribute()]
    public sealed class CustomPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        private bool unrestricted;

        public CustomPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                unrestricted = true;
            }
            else
            {
                unrestricted = false;
            }
        }

        //Define the rest of your custom permission here. You must 
        //implement IsUnrestricted and override the Copy, Intersect, 
        //IsSubsetOf, ToXML, and FromXML methods.
        public bool IsUnrestricted()
        {
            return unrestricted;
        }

        public override IPermission Copy()
        {
            CustomPermission copy = new CustomPermission(PermissionState.None);

            if (this.IsUnrestricted())
            {
                copy.unrestricted = true;
            }
            else
            {
                copy.unrestricted = false;
            }
            return copy;
        }

        public override IPermission Intersect(IPermission target)
        {
            try
            {
                if (null == target)
                {
                    return null;
                }
                CustomPermission PassedPermission = (CustomPermission)target;

                if (!PassedPermission.IsUnrestricted())
                {
                    return PassedPermission;
                }
                return this.Copy();
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Argument_WrongType", this.GetType().FullName);
            }
        }

        public override bool IsSubsetOf(IPermission target)
        {
            if (null == target)
            {
                return !this.unrestricted;
            }
            try
            {
                CustomPermission passedpermission = (CustomPermission)target;
                if (this.unrestricted == passedpermission.unrestricted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Argument_WrongType", this.GetType().FullName);
            }
        }

        public override SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("IPermission");
            Type type = this.GetType();
            StringBuilder AssemblyName = new StringBuilder(type.Assembly.ToString());
            AssemblyName.Replace('\"', '\'');
            element.AddAttribute("class", type.FullName + ", " + AssemblyName);
            element.AddAttribute("version", "1");
            element.AddAttribute("Unrestricted", unrestricted.ToString());
            return element;
        }

        public override void FromXml(SecurityElement PassedElement)
        {
            string element = PassedElement.Attribute("Unrestricted");
            if (null != element)
            {
                this.unrestricted = Convert.ToBoolean(element);
            }
        }
    }

    // Mark - CustomePermissionAttribute
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
    public class CustomPermissionAttribute : CodeAccessSecurityAttribute
    {
        bool unrestricted = false;

        public new bool Unrestricted
        {
            get { return unrestricted; }
            set { unrestricted = value; }
        }

        public CustomPermissionAttribute(SecurityAction action)
            : base(action)
        {
        }
        public override IPermission CreatePermission()
        {
            if (Unrestricted)
            {
                return new CustomPermission(PermissionState.Unrestricted);
            }
            else
            {
                return new CustomPermission(PermissionState.None);
            }
        }
    }

    public enum SecurityArgType
    {
        A = 1,
        B = 2,
        C = 3,
    }

    // Mark - CustomePermissio2nAttribute
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
    //public class CustomPermission2Attribute : CodeAccessSecurityAttribute
    public class CustomPermission2Attribute : CustomPermissionAttribute
    {
        bool unrestricted = false;
        SecurityArgType x = SecurityArgType.A;

        public new bool Unrestricted
        {
            get { return unrestricted; }
            set { unrestricted = value; }
        }
        
        public SecurityArgType SecurityArg
        {
            get { return x; }
            set { x = value; }
        }

        public CustomPermission2Attribute(SecurityAction action)
            : base(action)
        {
        }
        public override IPermission CreatePermission()
        {
            if (Unrestricted)
            {
                return new CustomPermission(PermissionState.Unrestricted);
            }
            else
            {
                return new CustomPermission(PermissionState.None);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Security.Policy;
using System.Reflection;
using System.Collections;
using System.Security.Permissions;
using System.Drawing.Printing;
using System.Net;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.IO;

namespace PTRunner
{
    public enum PermSetNames { Everything, Internet, LocalIntranet, Execution, FullTrust };

    public class PTRunnerLib
    {

        #region NamedPermissionSets

        private static PermissionSet GetFullTrustPermissionSet()
        {
            PermissionSet ps = new PermissionSet(PermissionState.Unrestricted);
            return ps;
        }
        private static PermissionSet GetExecutionPermissionSet()
        {
            PermissionSet ps = new PermissionSet(PermissionState.None);
            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            return ps;
        }
        private static PermissionSet GetInternetPermissionSet()
        {
            Evidence ev = new Evidence();
            ev.AddHostEvidence(new Zone(SecurityZone.Internet));
            return SecurityManager.GetStandardSandbox(ev); 
        }
        private static PermissionSet GetIntranetPermissionSet()
        {
            Evidence ev = new Evidence();
            ev.AddHostEvidence(new Zone(SecurityZone.Intranet));
            return SecurityManager.GetStandardSandbox(ev);
        }
        //The mediapermission and webbrowserpermission are conditionally added to intranet and internet.
        private static void AddAdditionalPermissions(PermissionSet partialEverything)
        {
            foreach (IPermission perm in GetIntranetPermissionSet())
            {
                if(partialEverything.GetPermission(perm.GetType())==null)
                {
                    ConstructorInfo ci = perm.GetType().GetConstructor(new Type[] { typeof(PermissionState) });
                    partialEverything.AddPermission((IPermission)ci.Invoke(new object[]{PermissionState.Unrestricted}));
                }
            }
        }
        private static PermissionSet GetEverythingPermissionSet()
        {
            PermissionSet ps = new PermissionSet(PermissionState.None);
            ps.AddPermission(new IsolatedStorageFilePermission(PermissionState.Unrestricted));
            ps.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
            ps.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
            ps.AddPermission(new FileDialogPermission(PermissionState.Unrestricted));
            ps.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Assertion | SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread | SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlAppDomain | SecurityPermissionFlag.SerializationFormatter | SecurityPermissionFlag.ControlDomainPolicy | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.RemotingConfiguration | SecurityPermissionFlag.Infrastructure | SecurityPermissionFlag.BindingRedirects));
            ps.AddPermission(new UIPermission(PermissionState.Unrestricted));
            ps.AddPermission(new SocketPermission(PermissionState.Unrestricted));
            ps.AddPermission(new WebPermission(PermissionState.Unrestricted));
            ps.AddPermission(new DnsPermission(PermissionState.Unrestricted));
            ps.AddPermission(new KeyContainerPermission(PermissionState.Unrestricted));
            ps.AddPermission(new RegistryPermission(PermissionState.Unrestricted));
            ps.AddPermission(new PrintingPermission(PermissionState.Unrestricted));
            ps.AddPermission(new EventLogPermission(PermissionState.Unrestricted));
            ps.AddPermission(new StorePermission(PermissionState.Unrestricted));
            ps.AddPermission(new PerformanceCounterPermission(PermissionState.Unrestricted));
            ps.AddPermission(new OleDbPermission(PermissionState.Unrestricted));
            ps.AddPermission(new SqlClientPermission(PermissionState.Unrestricted));
            ps.AddPermission(new DataProtectionPermission(PermissionState.Unrestricted));
            AddAdditionalPermissions(ps);
            return ps;
        }
        #endregion

        //The available settings for the partial trust sandboxes

        /// <summary>
        /// Transform from a PermSetNames enum tu a PermissionSet
        /// </summary>
        /// <param name="permSetName">The sandbox that we want</param>
        /// <returns>The actual permission set</returns>
        public static PermissionSet GetStandardPermission(PermSetNames permSetName)
        {
            PermissionSet ps = null;
            switch (permSetName)
            {
                case PermSetNames.Execution:
                    ps = GetExecutionPermissionSet();
                    break;
                case PermSetNames.Internet:
                    ps = GetInternetPermissionSet();
                    break;
                case PermSetNames.LocalIntranet:
                    ps = GetIntranetPermissionSet();
                    break;
                case PermSetNames.Everything:
                    ps = GetEverythingPermissionSet();
                    break;
                case PermSetNames.FullTrust:
                    ps = GetFullTrustPermissionSet();
                    break;
            }
            return ps;
        }
        /// <summary>
        /// Return the strong name of an Assembly
        /// </summary>
        /// <param name="asm">The assembly that we want to get the strong name from</param>
        /// <returns>The strong name</returns>
        private static StrongName GetStrongName(Assembly assembly)
        {
            //This should actually use assembly.Evidence.GetHostEvidence<StrongName>(), but due to a bug that doesn't work
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            StrongName sn = assembly.Evidence.GetHostEvidence<StrongName>();
            if (sn == null)
                throw new InvalidOperationException(String.Format("Assembly \"{0}\" is not strongly named. Use /keyfile flag when compiling it", assembly.FullName));

            return sn;
        }
        /// <summary>
        /// Construct the Sandbox in which the program will run. This will have the reduced grant set
        /// </summary>
        /// <param name="ps">The permission set in which the program will be run</param>
        /// <param name="fullTrustList">The full trust list</param>
        /// <returns>The sandbox AppDomain</returns>
        public static AppDomain CreateSandboxedDomain(PermissionSet ps, params Assembly[] fullTrustList)
        {
            int ftLen = fullTrustList.Length;
            StrongName[] fullTrustStrongNames = new StrongName[ftLen + 1];
            //We have to add ourselves to the list of full trust assemblies
            fullTrustStrongNames[0] = GetStrongName(typeof(PTRunnerLib).Assembly);
            int i = 1;
            foreach (Assembly asm in fullTrustList)
            {
                StrongName asmStrongName = GetStrongName(asm);
                fullTrustStrongNames[i++] = asmStrongName;
            }

            return AppDomain.CreateDomain("Sandbox", null, AppDomain.CurrentDomain.SetupInformation, ps, fullTrustStrongNames);
        }
        /// <summary>
        /// Create a new sandbox AppDomain with the permission and full trust list passed and initialize 
        /// a new instance of the type T in it
        /// </summary>
        /// <typeparam name="T">The type that will be instantiated in the new domain. This has to be a MarshalByRefObject</typeparam>
        /// <param name="ps">Permission set for the new domain</param>
        /// <param name="fullTrustList">List of full trust assemblies. All assemblies in this list have to be fully trusted</param>
        /// <returns>An instance in the new sandboxed AppDomain</returns>
        [SecuritySafeCritical]
        public static T GetPartialTrustInstance<T>(PermissionSet ps, params Assembly[] fullTrustList) where T : MarshalByRefObject
        {
            AppDomain domain = CreateSandboxedDomain(ps, fullTrustList);
            return (T)domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }
        /// <summary>
        /// Create a new sandbox AppDomain with the permission and full trust list passed and initialize 
        /// a new instance of the type T in it
        /// </summary>
        /// <typeparam name="T">The type that will be instantiated in the new domain. This has to be a MarshalByRefObject</typeparam>
        /// <param name="psName">Name of standard permission set for the new domain</param>
        /// <param name="fullTrustList">List of full trust assemblies. All assemblies in this list have to be fully trusted</param>
        /// <returns>An instance in the new sandboxed AppDomain</returns>
        public static T GetPartialTrustInstance<T>(PermSetNames psName, params Assembly[] fullTrustList) where T : MarshalByRefObject
        {
            return GetPartialTrustInstance<T>(GetStandardPermission(psName), fullTrustList);
        }
        /// <summary>
        /// Create a new sandbox AppDomain with the permission and full trust list passed and initialize 
        /// a new instance of the type T in it. The permission in the new AppDomain  is Execution
        /// </summary>
        /// <typeparam name="T">The type that will be instantiated in the new domain. This has to be a MarshalByRefObject</typeparam>
        /// <param name="fullTrustList">List of full trust assemblies. All assemblies in this list have to be fully trusted</param>
        /// <returns>An instance in the new sandboxed AppDomain</returns>
        public static T GetPartialTrustInstance<T>(params Assembly[] fullTrustList) where T : MarshalByRefObject
        {
            return GetPartialTrustInstance<T>(GetStandardPermission(PermSetNames.Execution), fullTrustList);
        }

    }
}
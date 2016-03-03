// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem.Web
{
	internal class WAUtilities
	{

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Helper to create an instance from the local registry given a CLSID Guid
		/// </summary>
		///--------------------------------------------------------------------------------------------
		public static InterfaceType CreateInstance<InterfaceType>(IServiceProvider serviceProvider, Guid clsid) where InterfaceType : class
		{
			InterfaceType instance = null;

			if (clsid != Guid.Empty)
			{
				ILocalRegistry localRegistry = GetService<ILocalRegistry>(serviceProvider);
				if (localRegistry != null)
				{
					IntPtr pInstance = IntPtr.Zero;
					Guid iidUnknown = NativeMethods.IID_IUnknown;

					try
					{
						localRegistry.CreateInstance(clsid, null, ref iidUnknown, NativeMethods.CLSCTX_INPROC_SERVER, out pInstance);
					}
					catch { }

					if (pInstance != IntPtr.Zero)
					{
						try
						{
							instance = Marshal.GetObjectForIUnknown(pInstance) as InterfaceType;
						}
						catch { }

						try
						{
							Marshal.Release(pInstance);
						}
						catch { }
					}
				}
			}

			return instance;
		}

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Helper to create an instance from the local registry given a CLSID Guid
		/// </summary>
		///--------------------------------------------------------------------------------------------
		public static InterfaceType CreateSitedInstance<InterfaceType>(IServiceProvider serviceProvider, Guid clsid) where InterfaceType : class
		{
			InterfaceType instance = CreateInstance<InterfaceType>(serviceProvider, clsid);

			if (instance != null)
			{
				IObjectWithSite sitedObject = instance as IObjectWithSite;
				IOleServiceProvider site = GetService<IOleServiceProvider>(serviceProvider);
				if (sitedObject != null && site != null)
				{
					sitedObject.SetSite(site);
				}
				else
				{
					instance = null; // failed to site
				}
			}

			return instance;
		}
		public static InterfaceType CreateSitedInstance<InterfaceType>(IOleServiceProvider serviceProvider, Guid clsid) where InterfaceType : class
		{
			return CreateSitedInstance<InterfaceType>(new ServiceProvider(serviceProvider), clsid);
		}

		///--------------------------------------------------------------------------------------------
		/// <summary>
		/// Helper to get a shell service interface
		/// </summary>
		///--------------------------------------------------------------------------------------------
		public static InterfaceType GetService<InterfaceType>(IServiceProvider serviceProvider) where InterfaceType : class
		{
			InterfaceType service = null;

			try
			{
				service = serviceProvider.GetService(typeof(InterfaceType)) as InterfaceType;
			}
			catch
			{
			}

			return service;
		}

		///--------------------------------------------------------------------------------------------
		/// <summary>
		///    Helpers to ensure (or remove) trailing slashes (or back slashes)
		/// </summary>
		///--------------------------------------------------------------------------------------------
		public static string EnsureTrailingSlash(string str)
		{
			if (str != null && !str.EndsWith("/"))
			{
				str += "/";
			}
			return str;
		}
		public static string EnsureTrailingBackSlash(string str)
		{
			if (str != null && !str.EndsWith("\\"))
			{
				str += "\\";
			}
			return str;
		}

		///--------------------------------------------------------------------------------------------
		/// <summary>
		///    Helpers to make relative paths/urls from physical paths
		/// </summary>
		///--------------------------------------------------------------------------------------------
		public static string MakeRelativePath(string fullPath, string basePath)
		{
			string separator = Path.DirectorySeparatorChar.ToString();
			string tempBasePath = basePath;
			string tempFullPath = fullPath;
			string relativePath = null;

			if (!tempBasePath.EndsWith(separator))
				tempBasePath += separator;
			tempFullPath = tempFullPath.ToLowerInvariant();
			tempBasePath = tempBasePath.ToLowerInvariant();

			while (!string.IsNullOrEmpty(tempBasePath))
			{
				if (tempFullPath.StartsWith(tempBasePath))
				{
					relativePath += fullPath.Remove(0, tempBasePath.Length);
					if (relativePath == separator)
						relativePath = "";
					return relativePath;
				}
				else
				{
					tempBasePath = tempBasePath.Remove(tempBasePath.Length - 1);
					int nLastIndex = tempBasePath.LastIndexOf(separator);
					if (-1 != nLastIndex)
					{
						tempBasePath = tempBasePath.Remove(nLastIndex + 1);
						relativePath += "..";
						relativePath += separator;
					}
					else
						return fullPath;
				}
			}

			return fullPath;
		}
	}


}

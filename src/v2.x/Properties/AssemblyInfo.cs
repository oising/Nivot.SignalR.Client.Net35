// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System.Reflection;
using System;
using System.Resources;
using System.Runtime.InteropServices;


[assembly: AssemblyTitle("Nivot.SignalR.Client.Net35")]
[assembly: AssemblyDescription(".NET 3.5 client for SignalR v2.x")]
[assembly: AssemblyVersion("2.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("Microsoft Open Technologies, Inc.")]
[assembly: AssemblyCopyright("© Microsoft Open Technologies, Inc. All rights reserved.")]
[assembly: AssemblyProduct("Microsoft ASP.NET SignalR")]
[assembly: AssemblyMetadata("Serviceable", "True")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyConfiguration("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

[assembly: NeutralResourcesLanguage("en-US")]


#if !NET45 && !NETFX_CORE && !WINDOWS_PHONE
namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    internal sealed class AssemblyMetadataAttribute : Attribute
    {
        public AssemblyMetadataAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
#endif

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.


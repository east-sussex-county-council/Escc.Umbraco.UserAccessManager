using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Escc.Umbraco.UserAccessManager")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Escc.Umbraco.UserAccessManager")]
[assembly: AssemblyCopyright("Copyright ©  2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3777d7ee-1159-4912-b106-4b54b589ac67")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
// v1.0.1 First versioned DLL. Fixes #7903
// v1.0.2 added additional links to tools pages, to allow switching between page and user pages.
//        also enhanced and standardised the page layouts. Ref #7696, #8019
// v1.0.3 When looking for pages that expire within 3 days (or as configured), all pages for the related
//        user were being added rather than just the specific page. Fixes #8082
// v1.1.0 Added inbound links lookup
// v1.2.0 Moved Redirects DB lookup code here from WebService as no access from Azure
// v1.2.1 Fixed path to ajax-loader image for PageAuthor page
// v1.3.0 Added Page Permissions function
// v1.4.0 Enabled Inspyder data search.
//
[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyFileVersion("1.4.0.0")]
using System.Reflection;

[assembly: AssemblyDescription("Reference implementation for simple DI container")]
[assembly: AssemblyCompany("StugoLtd")]
[assembly: AssemblyProduct("Stugo.Glue")]
[assembly: AssemblyCopyright("Copyright StugoLtd ©  2016")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif

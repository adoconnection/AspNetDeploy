using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VsTestLibrary
{
    public class VsTestParser
    {
        public IList<VsTestClassInfo> Parse(string assemblyPath)
        {
            return this.Parse(Assembly.LoadFrom(assemblyPath));
        }

        public IList<VsTestClassInfo> Parse(Assembly assembly)
        {
            return assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes().Any(a => a.TypeId.ToString() == @"Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute"))
                    .Select(t => {

                        List<MemberInfo> memberInfos = t.GetMembers().ToList();

                        return new VsTestClassInfo
                        {
                            Type = t,
                            InitializeMethod = FindMethods(memberInfos, "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute").FirstOrDefault(),
                            CleanupMethod = FindMethods(memberInfos, "Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute").FirstOrDefault(),
                            TestMethods = FindMethods(memberInfos, "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute").ToList().AsReadOnly()
                        };
                    })
                    .ToList().AsReadOnly();
        }

        private static IList<string> FindMethods(List<MemberInfo> memberInfos, string attributeName)
        {
            return memberInfos
                    .Where(m => m.GetCustomAttributes().Any(a => a.TypeId.ToString() == attributeName))
                    .Select(m => m.Name)
                    .ToList();
        }
    }
}
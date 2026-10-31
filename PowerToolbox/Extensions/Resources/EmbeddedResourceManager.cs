using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace PowerToolbox.Extensions.Resources
{
    /// <summary>
    /// 支持主程序集多语言资源的 ResourceManager
    /// </summary>
    internal sealed class EmbeddedResourceManager(string baseName, Assembly assembly) : ResourceManager(baseName, assembly)
    {
        private readonly Assembly assembly = assembly;
        private readonly string baseName = baseName;
        private readonly Dictionary<string, ResourceSet> resourceSetDict = [];
        private readonly object syncRoot = new();

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            lock (syncRoot)
            {
                string cultureName = culture.Name.ToLowerInvariant();
                while (true)
                {
                    if (resourceSetDict.TryGetValue(cultureName, out ResourceSet resourceSet))
                    {
                        return resourceSet;
                    }

                    if (createIfNotExists)
                    {
                        string resourceName = string.IsNullOrEmpty(cultureName) ? baseName + ".resources" : baseName + "." + cultureName + ".resources";
                        if (assembly.GetManifestResourceStream(resourceName) is Stream stream)
                        {
                            resourceSet = new ResourceSet(stream);
                            resourceSetDict[cultureName] = resourceSet;
                            stream.Dispose();
                            return resourceSet;
                        }
                    }

                    if (!tryParents || Equals(culture, CultureInfo.InvariantCulture))
                    {
                        return null;
                    }

                    culture = culture.Parent;
                }
            }
        }
    }
}

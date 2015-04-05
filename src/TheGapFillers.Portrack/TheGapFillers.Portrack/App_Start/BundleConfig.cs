using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace TheGapFillers.Portrack
{
    public class BundleConfig
    {
        public static string AdminAppDir = "app";

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            CleanupUnusedFiles();
            BundleTable.VirtualPathProvider = new ScriptBundlePathProvider(HostingEnvironment.VirtualPathProvider);
            AddAppBundles(bundles);
            bundles.IgnoreList.Ignore("*Spec.js");
        }

        private static void AddAppBundles(BundleCollection bundles)
        {
            var scriptBundle = new ScriptBundle("~/adminscripts");
            var adminAppDirFullPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}", AdminAppDir));
            if (Directory.Exists(adminAppDirFullPath))
            {
                scriptBundle.Include(
                    // Order matters, so get the core app setup first
                    string.Format("~/{0}/app.module.js", AdminAppDir),
                    string.Format("~/{0}/app.core.module.js", AdminAppDir))
                    // then get any other top level js files
                    .IncludeDirectory(string.Format("~/{0}", AdminAppDir), "*.js", false)
                    // then get all nested module js files
                    .IncludeDirectory(string.Format("~/{0}", AdminAppDir), "*.module.js", true)
                    // finally, get all the other js files
                    .IncludeDirectory(string.Format("~/{0}", AdminAppDir), "*.js", true);
            }
            bundles.Add(scriptBundle);
            bundles.Add(new StyleBundle("~/adminstyles").Include(
                "~/content/css/admin.css"));
        }

        [Conditional("DEBUG")]
        private static void CleanupUnusedFiles()
        {
            var adminAppDirFullPath = HttpContext.Current.Server.MapPath(string.Format("~/{0}", AdminAppDir));
            if (Directory.Exists(adminAppDirFullPath))
            {
                var jsFiles = Directory.GetFiles(adminAppDirFullPath, "*.js", SearchOption.AllDirectories);
                foreach (var jsFile in jsFiles)
                {
                    var tsFile = jsFile.Remove(jsFile.Length - 3, 3) + ".ts";
                    if (!File.Exists(tsFile) && !jsFile.EndsWith("spec.js"))
                    {
                        File.Delete(jsFile);
                        var map = jsFile + ".map";
                        if (File.Exists(map)) File.Delete(map);
                    }
                }
            }
        }
    }

    class ScriptBundlePathProvider : VirtualPathProvider
    {
        private readonly VirtualPathProvider _virtualPathProvider;

        public ScriptBundlePathProvider(VirtualPathProvider virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
        }

        public override bool FileExists(string virtualPath)
        {
            return _virtualPathProvider.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return _virtualPathProvider.GetFile(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return _virtualPathProvider.GetDirectory(virtualDir);
        }

        public override bool DirectoryExists(string virtualDir)
        {
            return _virtualPathProvider.DirectoryExists(virtualDir);
        }
    }
}

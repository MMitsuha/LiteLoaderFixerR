using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteLoaderFixerR.Utils
{
    internal class QQ
    {
        public static string? GetQQLocation()
        {
            var software = Registry.LocalMachine.OpenSubKey("SOFTWARE");

            if (software == null)
            {
                return null;
            }

            var wow_registry = software.OpenSubKey("WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            var native_registry = software.OpenSubKey("Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            object? uninstall = null;

            if (wow_registry != null)
            {
                uninstall = wow_registry.GetValue("UninstallString");
            }
            else if (native_registry != null)
            {
                uninstall = native_registry.GetValue("UninstallString");
            }

            if (uninstall == null)
            {
                return null;
            }

            var location = uninstall.ToString();

            if (location == null)
            {
                return null;
            }

            return Path.GetDirectoryName(location);
        }

        public static string? GetQQVersion()
        {
            var software = Registry.LocalMachine.OpenSubKey("SOFTWARE");

            if (software == null)
            {
                return null;
            }

            var wow_registry = software.OpenSubKey("WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            var native_registry = software.OpenSubKey("Microsoft\\Windows\\CurrentVersion\\Uninstall\\QQ");
            object? version = null;

            if (wow_registry != null)
            {
                version = wow_registry.GetValue("DisplayVersion");
            }
            else if (native_registry != null)
            {
                version = native_registry.GetValue("DisplayVersion");
            }

            if (version == null)
            {
                return null;
            }

            return version.ToString();
        }
    }
}
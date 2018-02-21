﻿using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using NUnit.Framework;
using Radarr.Common.Test.DiskTests;
using Radarr.Windows.Disk;

namespace Radarr.Runtime.Windows.Test.DiskProviderTests
{
    [TestFixture]
    public class DiskProviderFixture : DiskProviderFixtureBase<DiskProvider>
    {
        public DiskProviderFixture()
        {
            WindowsOnly();
        }

        protected override void SetWritePermissions(string path, bool writable)
        {
            // Remove Write permissions, we're owner and have Delete permissions, so we can still clean it up.

            var owner = WindowsIdentity.GetCurrent().Owner;
            var accessControlType = writable ? AccessControlType.Allow : AccessControlType.Deny;

            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                var directorySecurity = new DirectorySecurity(path, AccessControlSections.None);
                directorySecurity.SetAccessRule(new FileSystemAccessRule(owner, FileSystemRights.Write, accessControlType));
                directoryInfo.SetAccessControl(directorySecurity);
            }
            else
            {
                var fileInfo = new FileInfo(path);
                var fileSecurity = new FileSecurity(path, AccessControlSections.None);
                fileSecurity.SetAccessRule(new FileSystemAccessRule(owner, FileSystemRights.Write, accessControlType));
                fileInfo.SetAccessControl(fileSecurity);
            }
        }
    }
}

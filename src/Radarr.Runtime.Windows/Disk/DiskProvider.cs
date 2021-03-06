﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using NLog;
using Radarr.Common.Disk;
using Radarr.Common.Disk.Interfaces;
using Radarr.Common.EnsureThat;
using Radarr.Common.Instrumentation;

namespace Radarr.Windows.Disk
{
    public class DiskProvider : DiskProviderBase
    {
        private static readonly Logger Logger = RadarrLogger.GetLogger(typeof(DiskProvider));

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        public override long? GetAvailableSpace(string path)
        {
            Ensure.That(path, () => path).IsValidPath();

            var root = GetPathRoot(path);

            if (!FolderExists(root))
                throw new DirectoryNotFoundException(root);

            return DriveFreeSpaceEx(root);
        }

        public override void InheritFolderPermissions(string filename)
        {
            Ensure.That(filename, () => filename).IsValidPath();

            var fileInfo = new FileInfo(filename);
            var fileSecurity = new FileSecurity(filename, AccessControlSections.None);
            fileSecurity.SetAccessRuleProtection(false, false);
            fileInfo.SetAccessControl(fileSecurity);
        }

        public override void SetPermissions(string path, string mask, string user, string group)
        {

        }

        public override long? GetTotalSize(string path)
        {
            Ensure.That(path, () => path).IsValidPath();

            var root = GetPathRoot(path);

            if (!FolderExists(root))
                throw new DirectoryNotFoundException(root);

            return DriveTotalSizeEx(root);
        }

        private static long DriveFreeSpaceEx(string folderName)
        {
            Ensure.That(folderName, () => folderName).IsValidPath();

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong free = 0;
            ulong dummy1 = 0;
            ulong dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
            {
                return (long)free;
            }

            return 0;
        }

        private static long DriveTotalSizeEx(string folderName)
        {
            Ensure.That(folderName, () => folderName).IsValidPath();

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong total = 0;
            ulong dummy1 = 0;
            ulong dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out dummy1, out total, out dummy2))
            {
                return (long)total;
            }

            return 0;
        }


        public override bool TryCreateHardLink(string source, string destination)
        {
            try
            {
                return CreateHardLink(destination, source, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, string.Format("Hardlink '{0}' to '{1}' failed.", source, destination));
                return false;
            }
        }
    }
}

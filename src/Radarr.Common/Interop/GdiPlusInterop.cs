using System;
using System.Drawing;
using Radarr.Common.EnvironmentInfo;

namespace Radarr.Common.Interop
{
    public static class GdiPlusInterop
    {
        private static Exception _gdiPlusException;

        static GdiPlusInterop()
        {
            TestLibrary();
        }

        private static void TestLibrary()
        {
            if (OsInfoCore.IsWindows)
            {
                return;
            }

            try
            {
                // We use StringFormat as test because it gets properly cleaned up by the finalizer even if gdiplus is absent and is relatively non-invasive.
                var strFormat = new StringFormat();

                strFormat.Dispose();
            }
            catch (Exception ex)
            {
                _gdiPlusException = ex;
            }
        }

        public static void CheckGdiPlus()
        {
            if (_gdiPlusException != null)
            {
                throw new DllNotFoundException("Couldn't load GDIPlus library", _gdiPlusException);
            }
        }
    }
}

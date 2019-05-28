using System;
using System.Runtime.InteropServices;

namespace DotnetCoreClipboard
{
    public class Clipboard
    {
        private Action<string> setAction => CreateSet();
        private Func<string> getFunc => CreateGet();

        public void SetText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            setAction(text);
        }

        public string GetText()
        {
            return getFunc();
        }

        private Action<string> CreateSet()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsClipboard.SetText;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OsxClipboard.SetText;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LinuxClipboard.SetText;
            }

            return s => throw new NotSupportedException();
        }

        private Func<string> CreateGet()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return WindowsClipboard.GetText;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OsxClipboard.GetText;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LinuxClipboard.GetText;
            }

            return () => throw new NotSupportedException();
        }
    }
}
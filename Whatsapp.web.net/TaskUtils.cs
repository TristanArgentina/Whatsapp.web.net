using System.Diagnostics;

namespace Whatsapp.web.net;

public static class TaskUtils
{
    public static void KillProcessesByName(string processName, string expectedPath)
    {
        var chrome = Process.GetProcessesByName(processName);

        
        foreach (var chromeProcess in chrome)
        {
            try
            {
                if (chromeProcess.MainModule == null) continue;
                var fullPath = chromeProcess.MainModule.FileName;

                if (fullPath.Equals(expectedPath))
                {
                    chromeProcess.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
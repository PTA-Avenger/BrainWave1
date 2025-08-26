namespace BrainWave.App;

using Microsoft.Maui.Devices;


public static class Constants
{
    // Returns the API base URL (no trailing slash) based on the current platform
    public static string ApiBaseUrl
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "http://10.0.2.2:5000/api";

            if (DeviceInfo.Platform == DevicePlatform.iOS)
                return "http://localhost:5000/api";

            // Windows, macOS, Linux (desktop)
            return "https://localhost:5001/api";
        }
    }
}
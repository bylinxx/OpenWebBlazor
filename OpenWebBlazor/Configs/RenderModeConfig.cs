using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace OpenWebBlazor.Configs;

public class RenderModeConfig
{
    private static readonly List<string> StaticRenderModePaths =
    [
        "/account/login",
        "/account/logout"
    ];

    public static IComponentRenderMode? GetRenderMode(string path)
    {
        return StaticRenderModePaths.Contains(path.ToLower()) ? null : RenderMode.InteractiveServer;
    }
}
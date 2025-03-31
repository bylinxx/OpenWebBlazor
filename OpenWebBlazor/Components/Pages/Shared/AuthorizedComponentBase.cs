using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace OpenWebBlazor
{
    [Authorize]
    public class AuthorizedComponentBase : ComponentBase
    {
    }
}

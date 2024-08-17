using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace MobileAPI
{
    public class HangFireAuthen : IDashboardAuthorizationFilter
    {
        bool IDashboardAuthorizationFilter.Authorize(DashboardContext context)
        {
            return true;
        }
    }
}

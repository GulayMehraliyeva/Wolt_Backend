using Service.ViewModels.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<HeaderVM> GetHeaderDataAsync(ClaimsPrincipal userPrincipal);
        Task<FooterVM> GetFooterDataAsync(ClaimsPrincipal userPrincipal);
    }
}

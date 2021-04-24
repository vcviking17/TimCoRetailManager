using System.Collections.Generic;
using System.Threading.Tasks;
using TRMWPFDesktopUI.Library.Models;

namespace TRMWPFDesktopUI.Library.Api
{
    public interface IProductEndpoint
    {
        Task<List<ProductModel>> GetAll();
    }
}
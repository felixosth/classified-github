using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InSupport.O3C.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InSupport.O3C.API.Pages
{
    public class IndexModel : PageModel
    {
        public O3CDeviceDbContext DeviceContext { get; private set; }

        public IndexModel(O3CDeviceDbContext deviceContext)
        {
            this.DeviceContext = deviceContext;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace asdfghjk
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MasterContext>(y => y.UseMySQL("server=127.0.0.1;port=3306;database=Test;user=root;password=root"));
        }

    }
}

namespace Database.Domain.Models.Library
{
    using System;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.DependencyInjection;

    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            try
            {
                services.AddHandlebarsScaffolding(
                    opts =>
                    {
                        opts.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;

                        opts.TemplateData = new Dictionary<string, object>
                                                       {
                                                           { "base-class", "BaseEntity" }
                                                       };
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphQLDemo00
{
    public class Startup
    {
       public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddGraphQLServer()//����GraphQL           
                .AddQueryType<Query>()//ע���ѯ����
                
                .AddSubscriptionType<SubQuery>()
                .AddProjections()//ӳ���ֶ�
                .AddFiltering()//ע���ѯ������
                .AddSorting()//ע������
                ;
        }       
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        }
    }
}
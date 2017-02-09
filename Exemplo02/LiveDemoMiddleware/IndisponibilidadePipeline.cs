using Microsoft.AspNetCore.Builder;

namespace LiveDemoMiddleware
{
    public class IndisponibilidadePipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseChecagemIndisponibilidade();
        }
    }
}
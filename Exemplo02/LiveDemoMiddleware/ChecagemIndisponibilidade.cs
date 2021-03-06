using System;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LiveDemoMiddleware
{
    public class ChecagemIndisponibilidade
    {
        private readonly RequestDelegate _next;

        public ChecagemIndisponibilidade(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            IConfiguration config = (IConfiguration)httpContext
                .RequestServices.GetService(typeof(IConfiguration));
            string mensagem = null;

            using (SqlConnection conexao = new SqlConnection(
                config.GetConnectionString("ExemploMiddleware")))
            {
                conexao.Open();

                SqlCommand cmd = conexao.CreateCommand();
                cmd.CommandText =
                    "SELECT TOP 1 Mensagem FROM dbo.Indisponibilidade " +
                    "WHERE @DataProcessamento BETWEEN InicioIndisponibilidade " +
                      "AND TerminoIndisponibilidade " +
                    "ORDER BY InicioIndisponibilidade";
                cmd.Parameters.Add("@DataProcessamento",
                    SqlDbType.DateTime).Value = DateTime.Now;

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                    mensagem = reader["Mensagem"].ToString();

                conexao.Close();
            }

            if (mensagem == null)
                await _next(httpContext);
            else
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync(
                    $"<h1>{mensagem}</h1>");
            }
        }
    }

    public static class ChecagemIndisponibilidadeExtensions
    {
        public static IApplicationBuilder UseChecagemIndisponibilidade(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ChecagemIndisponibilidade>();
        }
    }
}
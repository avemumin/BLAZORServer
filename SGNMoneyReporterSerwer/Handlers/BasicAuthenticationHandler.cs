using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SGNMoneyReporterSerwer.Data;
using SGNMoneyReporterSerwer.Data.Entities;

namespace SGNMoneyReporterSerwer.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly BankContext _repositoryContext;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            BankContext repositoryContext)
            : base(options, logger, encoder, clock)
        {
            _repositoryContext = repositoryContext;
        }

        private const string HeaderRequest = "Authorization";
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            if (!Request.Headers.ContainsKey(HeaderRequest))
                return AuthenticateResult.Fail("Brak nagłówka autoryzacji");

            try
            {
                var authenticactionHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers[HeaderRequest]);

                var bytes = Convert.FromBase64String(authenticactionHeaderValue.Parameter);
                string[] credential = Encoding.UTF8.GetString(bytes).Split(":");
                string email = credential[0];
                string password = credential[1];

                User user = _repositoryContext.User.FirstOrDefault(usr =>
                    usr.UserEmailAddress == email && usr.UserPassword == password);

                if (user == null)
                    AuthenticateResult.Fail("Niepoprawny login lub hasło");
                else
                {
                    var claims = new[] {new Claim(ClaimTypes.Name, user.UserEmailAddress)};
                    var identity = new ClaimsIdentity(claims,Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal,Scheme.Name);
                   return AuthenticateResult.Success(ticket);
                }

            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Wystąpił błąd");
            }

            return AuthenticateResult.Fail("");
        }
    }
}

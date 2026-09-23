using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator
{
    private readonly IConfiguration configuration;
	
	public JwtTokenGenerator(IConfiguration configuration)
	{
		this.configuration = configuration;
	}

    public string GenerateToken(long userId)
    {
        var secretKey = configuration["Jwt:Key"] ?? string.Empty;
        var issuer = configuration["Jwt:Issuer"] ?? string.Empty;
        var audience = configuration["Jwt:Audience"] ?? string.Empty;
        var expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
			/*
			sub			JwtRegisteredClaimNames.Sub			Identificador único del usuario
			email		JwtRegisteredClaimNames.Email		Email del usuario
			name		JwtRegisteredClaimNames.Name		Nombre del usuario
			given_name	JwtRegisteredClaimNames.GivenName	Nombre de pila
			family_name	JwtRegisteredClaimNames.FamilyName	Apellido
			jti			JwtRegisteredClaimNames.Jti			ID único del token
			iat			JwtRegisteredClaimNames.Iat			Fecha/hora de emisión
			exp			JwtRegisteredClaimNames.Exp			Fecha/hora de expiración
			iss			JwtRegisteredClaimNames.Iss			Emisor del token
			aud			JwtRegisteredClaimNames.Aud			Audiencia
			nbf			JwtRegisteredClaimNames.Nbf			No válido antes de esta fecha
			*/
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            SigningCredentials = credentials
        };

        var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        return token;
    }
}


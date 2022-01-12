using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace GenerateXCodeBuildExportOptions.Services;

// Port from https://github.com/dersia/AppStoreConnect.
public static class JwtTokenKeyUtils
{
    private static void GetPrivateKey(TextReader reader, ECDsa ecDsa)
    {
        var privateKeyParameters = (ECPrivateKeyParameters)new PemReader(reader).ReadObject();
        Org.BouncyCastle.Math.EC.ECPoint q = privateKeyParameters.Parameters.G.Multiply(privateKeyParameters.D);
        var publicKeyParameters = new ECPublicKeyParameters(privateKeyParameters.AlgorithmName, q,
            privateKeyParameters.PublicKeyParamSet);
        var encoded1 = publicKeyParameters.Q.AffineXCoord.GetEncoded();
        var encoded2 = publicKeyParameters.Q.AffineYCoord.GetEncoded();
        var byteArrayUnsigned = privateKeyParameters.D.ToByteArrayUnsigned();
        var parameters = new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q =
            {
                X = encoded1,
                Y = encoded2
            },
            D = byteArrayUnsigned
        };
        parameters.Validate();
        ecDsa.ImportParameters(parameters);
    }

    private static TextReader GetReaderFromFile(string p8FileContent) => File.OpenText(p8FileContent);

    public static string CreateTokenAndSign(
        string privateKeyFilePath,
        string kid,
        SecurityTokenDescriptor securityTokenDescriptor)
    {
        using var readerFromFile = GetReaderFromFile(privateKeyFilePath);
        return CreateTokenAndSignInternal(readerFromFile, kid, securityTokenDescriptor);
    }

    private static string CreateTokenAndSignInternal(
        TextReader reader,
        string kid,
        SecurityTokenDescriptor securityTokenDescriptor)
    {
        using ECDsa ecDsa = ECDsa.Create();
        GetPrivateKey(reader, ecDsa);
        ECDsaSecurityKey key = new ECDsaSecurityKey(ecDsa)
        {
            KeyId = kid
        };
        SigningCredentials signingCredentials = new SigningCredentials(key, "ES256");
        // Apple accepts JWT only.
        securityTokenDescriptor.TokenType = "JWT";
        securityTokenDescriptor.SigningCredentials = signingCredentials;

        return new JsonWebTokenHandler().CreateToken(securityTokenDescriptor);
    }
}

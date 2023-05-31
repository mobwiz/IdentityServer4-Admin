namespace Mobwiz.Common.HashAlg
{
    public interface IHashAlgorithm
    {
        string ComputeHash(string key, string input);
    }
}

namespace ChipherCrackingGA.Cipher.Types
{
    /// <summary>
    /// The interface for new cipher methods. e.g Vigenere, Casear, Autokey, etc.
    /// Contains all methods definitions for performing encryption and decryption of plain text.
    /// </summary>
    public interface ICipher
    {
        string PlainText { get; set; }
        string CipherText { get; set; }
        string AttemptedDecipherText { get; set; }
        string Encipher(string key);
        string Decipher(string key);
    }
}
namespace VueCore.Models.Domain
{
    public record FileModel(string FileName, string MimeType, byte[] Content);
}

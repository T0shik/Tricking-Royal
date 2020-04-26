using System.Threading.Tasks;

namespace Battles.Application.SubServices.FileStorage
{
    public interface IFileStorage
    {
        Task<string> Save(byte[] file, params string[] filePath);
        Task<byte[]> Pop(string fileName);
        Task Sweep();
    }
}
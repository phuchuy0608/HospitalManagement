using System.Threading.Tasks;

namespace HospitalManagement.Interfaces
{
    public interface IAutoBackground
    {
        Task<bool> AutoDelete();
    }
}

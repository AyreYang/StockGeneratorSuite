using SGNativeEntities.General;
using System.Collections.Generic;

namespace SGDataService.Interfaces
{
    public interface IDataService
    {
        T FetchData<T>(string code) where T : BasicInfoEntity;
        List<T> FetchData<T>(List<string> codes) where T : BasicInfoEntity;
    }
}



using System.ServiceModel;
using MyServiceContract.Models;

namespace MyServiceContract
{

    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]

        //[WebInvoke(UriTemplate = "GetPerson/{personId}", Method = "GET")]

        //The url will be /root/prefix/GetPerson/1
        Person GetPerson(int personId);
    }
}

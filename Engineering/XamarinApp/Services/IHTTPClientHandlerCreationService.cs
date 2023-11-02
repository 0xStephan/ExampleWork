using System;
using System.Net.Http;

namespace VSpaceParkers
{
    public interface IHTTPClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}

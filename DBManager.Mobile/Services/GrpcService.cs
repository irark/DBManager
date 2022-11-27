using Grpc.Net.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Mobile.Services
{
    public class GrpcService
    {
        protected readonly GrpcChannel _chanel;
        private string backendUrl = "http://192.168.0.107:5110";
        /// <summary>
        /// Constructor.
        /// </summary>
        protected GrpcService()
        {
            _chanel = GrpcChannel.ForAddress(backendUrl);
        }

    }
}

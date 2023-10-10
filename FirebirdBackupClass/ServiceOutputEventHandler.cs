using FirebirdSql.Data.Services;
using System;

namespace FirebirdBackup
{
    internal class ServiceOutputEventHandler
    {
        private Action<object, ServiceOutputEventArgs> serviceOutput;

        public ServiceOutputEventHandler(Action<object, ServiceOutputEventArgs> serviceOutput)
        {
            this.serviceOutput = serviceOutput;
        }
    }
}
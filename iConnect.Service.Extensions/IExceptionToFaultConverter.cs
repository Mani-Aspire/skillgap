using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iConnect.Service.Extensions
{
    public interface IExceptionToFaultConverter
    {
        object ConvertExceptionToFaultDetail(Exception error);
    }
}

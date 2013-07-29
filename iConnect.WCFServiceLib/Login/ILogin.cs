using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using iConnect.Entities;


namespace iConnect.WCFServiceLib
{
	[ServiceContract]
	public interface ILogin
	{
		[OperationContract]
		bool Login(  );
	}
}

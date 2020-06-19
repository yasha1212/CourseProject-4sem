using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface ISerializer
    {
        byte[] Serialize(object obj);

        object Deserialize(byte[] data);
    }
}

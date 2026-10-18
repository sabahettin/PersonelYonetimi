using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Abstract
{
    public interface IBaseEntity
    {
        Guid Id { get; }
    }
}

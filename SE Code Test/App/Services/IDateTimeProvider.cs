using System;
using System.Collections.Generic;
using System.Text;

namespace App
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}

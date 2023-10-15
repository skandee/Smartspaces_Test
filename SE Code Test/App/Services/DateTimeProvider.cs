using System;
using System.Collections.Generic;
using System.Text;

namespace App
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}

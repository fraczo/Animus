using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Models
{
    public enum Marker
    {
        Ignore, //pomiń załączniki
        WithAttachements, //dołącz załączniki
        ReminderZUS, //tylko załączniki dotyczące płatności składek ZUS
        ReminderZUS_PIT //tylko załączniki dotyczące płatności składek ZUS_PIT
    }
}

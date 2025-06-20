using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Helpers.Enums
{
    public enum OrderStatus
    {
        Gözləyir = 1,
        Hazırlanır,
        Yolda,
        Çatdırıldı,
        Cancelled,
        Təsdiqlənmədi
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Enums
{
    public enum TicketStatus
    {
        Available, // Disponible à la vente
        Bought, // Achété mais pas encore utilisé
        Used, // Utilisé
        Expired // Expiré mais non utilisé
    }
}

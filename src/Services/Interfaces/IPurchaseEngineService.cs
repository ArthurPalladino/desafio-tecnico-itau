using System;
using System.Threading.Tasks;

namespace ItauCompraProgramada.Application.Interfaces;

public interface IPurchaseEngineService
{
    Task ExecuteAsync(DateTime referenceDate);
}
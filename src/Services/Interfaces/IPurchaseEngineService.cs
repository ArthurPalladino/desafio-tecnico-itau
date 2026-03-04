using System;
using System.Threading.Tasks;

public interface IPurchaseEngineService
{
    Task<MotorCompraResponseDto> ExecuteAsync(DateTime referenceDate);
}
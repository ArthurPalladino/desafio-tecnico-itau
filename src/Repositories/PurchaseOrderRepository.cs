using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class PurchaseOrderRepository(ItauTopFiveDbContext dbContext) : Repository<PurchaseOrder>(dbContext), IPurchaseOrderRepository
{
}
using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class RebalancingRepository(ItauTopFiveDbContext dbContext) : Repository<Rebalancing>(dbContext), IRebalancingRepository
{
}
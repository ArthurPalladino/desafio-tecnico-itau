using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class RelabancingRepository(ItauTopFiveDbContext dbContext) : Repository<Rebalancing>(dbContext), IRebalancingRepository
{
}
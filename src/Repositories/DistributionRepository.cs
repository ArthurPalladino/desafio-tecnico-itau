using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class DistributionRepository(ItauTopFiveDbContext dbContext) : Repository<Distribution>(dbContext), IDistributionRepository
{
}
using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;


public class CustodyRepository(ItauTopFiveDbContext dbContext) : Repository<Custody>(dbContext), ICustodyRepository
{
    
}
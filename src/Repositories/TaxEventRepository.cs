using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Generic;
using Repositories.Interfaces;

public class TaxEventRepository(ItauTopFiveDbContext dbContext) : Repository<TaxEvent>(dbContext), ITaxEventRepository
{
}
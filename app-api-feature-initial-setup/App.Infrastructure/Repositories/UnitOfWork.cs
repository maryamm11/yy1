using App.Core.Entities;
using App.Core.Interfaces;
using App.Infrastructure.DbContext;

namespace App.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<Charity>? _charities;
        private IGenericRepository<DonorOrganization>? _donorOrganizations;
        private IGenericRepository<CharityNeed>? _charityNeeds;
        private IGenericRepository<Offer>? _offers;
        private IGenericRepository<NeedApplication>? _needApplications;
        private IGenericRepository<OfferApplication>? _offerApplications;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Charity> Charities =>
            _charities ??= new GenericRepository<Charity>(_context);

        public IGenericRepository<DonorOrganization> DonorOrganizations =>
            _donorOrganizations ??= new GenericRepository<DonorOrganization>(_context);

        public IGenericRepository<CharityNeed> CharityNeeds =>
            _charityNeeds ??= new GenericRepository<CharityNeed>(_context);

        public IGenericRepository<Offer> Offers =>
            _offers ??= new GenericRepository<Offer>(_context);

        public IGenericRepository<NeedApplication> NeedApplications =>
            _needApplications ??= new GenericRepository<NeedApplication>(_context);

        public IGenericRepository<OfferApplication> OfferApplications =>
            _offerApplications ??= new GenericRepository<OfferApplication>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

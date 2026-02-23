using App.Core.Entities;

namespace App.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Charity> Charities { get; }
        IGenericRepository<DonorOrganization> DonorOrganizations { get; }
        IGenericRepository<CharityNeed> CharityNeeds { get; }
        IGenericRepository<Offer> Offers { get; }
        IGenericRepository<NeedApplication> NeedApplications { get; }
        IGenericRepository<OfferApplication> OfferApplications { get; }
        Task<int> SaveChangesAsync();
    }
}

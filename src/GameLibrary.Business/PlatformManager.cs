using GameLibrary.Data.Repositories;

namespace GameLibrary.Business;

public class PlatformManager
{
    private readonly IPlatformRepository repository;

    public PlatformManager(IPlatformRepository repository)
    {
        this.repository = repository;
    }

    // TODO: Write GetAll, GetById, Save, and Delete business methods yourself.
    // Save should decide between repository.InsertAsync and UpdateAsync.
    // See LAYER_PLAN.txt for the endpoint and return-value plan.
}

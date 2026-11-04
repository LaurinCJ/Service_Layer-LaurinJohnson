using GameLibrary.Data.Repositories;

namespace GameLibrary.Business;

public class GameManager
{
    private readonly IGameRepository repository;

    public GameManager(IGameRepository repository)
    {
        this.repository = repository;
    }

    // TODO: Write GetAll, GetById, Save, and Delete business methods yourself.
    // Save should decide between repository.InsertAsync and UpdateAsync.
    // See LAYER_PLAN.txt for the endpoint and return-value plan.
}

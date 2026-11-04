using GameLibrary.Business;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Api.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly GameManager manager;

    public GamesController(GameManager manager)
    {
        this.manager = manager;
    }

    // TODO: Write GET all, GET by ID, POST Save, and DELETE actions yourself.
    // The constructor is wired up; there are no working CRUD endpoints yet.
}

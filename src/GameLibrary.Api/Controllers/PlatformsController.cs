using GameLibrary.Business;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Api.Controllers;

[ApiController]
[Route("api/platforms")]
public class PlatformsController : ControllerBase
{
    private readonly PlatformManager manager;

    public PlatformsController(PlatformManager manager)
    {
        this.manager = manager;
    }

    // TODO: Write GET all, GET by ID, POST Save, and DELETE actions yourself.
    // The constructor is wired up; there are no working CRUD endpoints yet.
}

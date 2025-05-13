using Kol1_APBD.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kol1_APBD.Controllers;

[Route("api/")]
[ApiController]
public class CustomController : ControllerBase
{
    private readonly IDBservice _service;

    public CustomController(IDBservice service)
    {
        _service = service;
    }
    
    
}
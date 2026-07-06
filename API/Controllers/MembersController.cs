using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers;

[Authorize]
  public class MembersController(AppDbContext context):BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await context.User.ToListAsync();
            
            return members;
            
        }

        [HttpGet("{id}")] //localhost:5001/api/members/bob-id
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member=await context.User.FindAsync(id);

            if (member == null) return NotFound();

            return member;
        }
    }

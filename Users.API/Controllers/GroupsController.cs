using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.APP.Features.Groups;

namespace Users.API.Controllers
{
    /// <summary>
    /// API controller for managing group-related operations.
    /// Handles HTTP requests for inserting, updating, deleting or retrieving group data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
   
    [Authorize] 
               
    public class GroupsController : ControllerBase
    {
        private readonly IMediator _mediator;

      
        public GroupsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpGet] 
       
        public async Task<IActionResult> Get()
        {
            // Send a GroupQueryRequest to MediatR, which dispatches it to the appropriate handler (GroupQueryHandler).
            var query = await _mediator.Send(new GroupQueryRequest());

            // Execute the query and retrieve the results as a list asynchronously.
            var list = await query.ToListAsync();

            // Return the list of groups with HTTP 200 OK.
            return Ok(list);

           
        }

        
        [HttpGet("{id}")] 
        public async Task<IActionResult> Get(int id)
        {
            // Send a GroupQueryRequest to MediatR, which dispatches it to the appropriate handler (GroupQueryHandler).
            // The handler returns an IQueryable<GroupQueryResponse> representing all groups.
            var query = await _mediator.Send(new GroupQueryRequest());

            // Asynchronously find the group with the specified ID.
            // If no group matches, item will be null.
            var item = await query.SingleOrDefaultAsync(groupResponse => groupResponse.Id == id);

            // If the group is not found, return HTTP 404 Not Found.
            if (item is null)
                return NotFound();

            // If the group is found, return it with HTTP 200 OK.
            return Ok(item);
        }

        
        [HttpPost] // post route: /Groups
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Post(GroupCreateRequest request)
        {
            // Check if the incoming request model passes validations through data annotations.
            if (ModelState.IsValid)
            {
                // Send the creation request to MediatR, which will route it to the appropriate handler (GroupCreateHandler).
                var response = await _mediator.Send(request);

                // If the group was created successfully
                if (response.IsSuccessful)
                {
                    // Way 1: return HTTP 200 OK with the success command response.
                    //return Ok(response);
                    // Way 2: return HTTP 201 Created with the location of the new group and the success command response.
                    return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
                }

                // If the creation failed due to business logic (e.g., duplicate title), return HTTP 400 Bad Request with the command response.
                return BadRequest(response);
            }

            // If the model state is invalid, return HTTP 400 Bad Request with validation error details.
            return BadRequest(ModelState);
        }

      
        [HttpPut] // put route: /Groups
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Put(GroupUpdateRequest request)
        {
            // Check if the incoming request model passes validations through data annotations.
            if (ModelState.IsValid)
            {
                // Send the update request to MediatR, which will route it to the appropriate handler (GroupUpdateHandler).
                var response = await _mediator.Send(request);

                // If the group was updated successfully
                if (response.IsSuccessful)
                {
                    // Way 1: return HTTP 200 OK with the success command response.
                    //return Ok(response);
                    // Way 2: return HTTP 204 No Content (no response body).
                    return NoContent();
                }

                // If the update failed due to business logic (e.g., duplicate title or group not found), return HTTP 400 Bad Request with the command response.
                return BadRequest(response);
            }

            // If the model state is invalid, return HTTP 400 Bad Request with validation error details.
            return BadRequest(ModelState);
        }

        
        [HttpDelete("{id}")] 
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Delete(int id)
        {
            // Create a GroupDeleteRequest with the specified group ID and send it to MediatR.
            // MediatR dispatches the request to the appropriate handler (GroupDeleteHandler).
            var response = await _mediator.Send(new GroupDeleteRequest() { Id = id });

            // If group was deleted successfully
            if (response.IsSuccessful)
            {
                // Way 1: return HTTP 200 OK with the success command response.
                // return Ok(response); 
                // Way 2: return HTTP 204 No Content (no response body).
                return NoContent(); 
            }

            // If the deletion failed (e.g., group not found, relational records found), return HTTP 400 Bad Request with error details.
            return BadRequest(response);
        }
    }
}

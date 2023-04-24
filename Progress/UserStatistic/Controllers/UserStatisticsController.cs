using Entity;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Cryptography.Pkcs;
using UserStatistic.API;
using View.Models.In;
using View.Models.Out;

// For more information on enabling Web APIGateway for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserStatistic.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserStatisticsController : ControllerBase
    {
        private UserStatisticAPI API;

        public UserStatisticsController(DatabaseContext context)
        {
            API = new UserStatisticAPI(context);
        }


        // GET: api/<GetTop10>
        [HttpGet]
        public /*async Task<ActionResult<List<OutUserStatistics>>>*/IActionResult GetTop10()
        {
            var userTop = API.GetTop(10);

            if (userTop == null)
                return NotFound();

            return Ok(userTop);
        }

        // GET api/<Get>/5
        [HttpGet("{id}")]
        public /*async Task<ActionResult<OutUserStatistics>>*/IActionResult Get(long id)
        {
            var result = API.Get(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST api/<UserStatisticsController>
        [HttpPost]
        public IActionResult Create([FromBody] InUserStatistics userStatistics)
        {
            var result = API.CreateUserStatistic(userStatistics);

            if (!result)
                return NotFound();

            return Ok(result);

        }

        // PUT api/<UpdatePrivileged>/5
        [HttpPut("{id}")]
        public /*async Task<IActionResult>*/IActionResult UpdatePrivileged(long id, bool privileg)
        {
            if (!API.UpdatePrivileged(id, privileg))
                return NotFound();

            Task<bool> task = API.UpdateRatingGameAsync(id);
            if (!task.Result)//не забываем изменить статистику по игроку
                return NoContent();

            return Ok();
        }

        // PUT api/UpdateUserStatistic
        [HttpPut]
        public IActionResult UpdateUserStatistic([FromBody] InUserStatistics userStatistics)
        {
            if (!API.UpdateUserStatistic(userStatistics))
                return NoContent();

            return Ok();
        }
    }
}

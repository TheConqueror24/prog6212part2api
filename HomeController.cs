using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using prog_part_2.Models;

namespace prog_part_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Event> events = new List<Event>();
            string connectionString = _configuration.GetConnectionString("RaceDayConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT EventID, OrganizerID, EventName, EventDate, Location, Description FROM EVENT";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new Event
                            {
                                EventID = reader.GetInt32(0),
                                OrganizerID = reader.GetInt32(1),
                                EventName = reader.GetString(2),
                                EventDate = reader.GetDateTime(3),
                                Location = reader.GetString(4),
                                Description = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                            });
                        }
                    }
                }
            }

            return View(events);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
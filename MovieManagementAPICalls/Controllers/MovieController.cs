using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieManagementAPICalls.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMovies()
        {
            var data = new List<Movie>();
            string jsonContent = System.IO.File.ReadAllText("C:\\Users\\lbvik\\source\\repos\\MovieManagementAPICalls\\MovieManagementAPICalls\\Storage\\movies.json");
            return new ContentResult
            {
                Content = jsonContent,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        [HttpPost]
        public IActionResult AddMovie(Movie AddNewMovie)
        {
            if (AddNewMovie == null || IsDefaultMovie(AddNewMovie))
            {
                return BadRequest("Invalid movie data");
            }

            List<Movie> movies = LoadMovies();
            movies.Add(AddNewMovie);
            SaveMovies(movies);

            return Ok("Movie Added Successfully");
        }

        private bool IsDefaultMovie(Movie movie)
        {
            return movie.MovieTitle == "string" &&
                   movie.MovieDirector == "string" &&
                   movie.MovieCategory == "string" &&
                   movie.MovieReleaseYear == 0;
        }


        private List<Movie> LoadMovies()
        {
            List<Movie> Loadedmovies;
            try
            {
                string jsonData = System.IO.File.ReadAllText("C:\\Users\\lbvik\\source\\repos\\MovieManagementAPICalls\\MovieManagementAPICalls\\Storage\\movies.json");

                if (!string.IsNullOrEmpty(jsonData))
                {
                    JArray jsonArray = JArray.Parse(jsonData);

                    Loadedmovies = new List<Movie>();

                    foreach (JToken token in jsonArray)
                    {
                        Movie movie = new Movie
                        {
                            MovieTitle = token["MovieTitle"]?.ToString() ?? string.Empty,
                            MovieDirector = token["MovieDirector"]?.ToString() ?? string.Empty,
                            MovieCategory = token["MovieCategory"]?.ToString() ?? string.Empty,
                            MovieReleaseYear = token["MovieReleaseYear"]?.ToObject<int>() ?? 0
                        };
                        Loadedmovies.Add(movie);
                    }
                }
                else
                {
                    Loadedmovies = new List<Movie>();
                }
            }
            catch (Exception)
            {
                Loadedmovies = new List<Movie>();
            }
            return Loadedmovies;
        }

        private void SaveMovies(List<Movie> Savedmovies)
        {
            JArray jsonArray = new JArray();
            foreach (Movie movie in Savedmovies)
            {
                JObject jsonMovie = new JObject
                {
                    ["MovieTitle"] = movie.MovieTitle,
                    ["MovieDirector"] = movie.MovieDirector,
                    ["MovieCategory"] = movie.MovieCategory,
                    ["MovieReleaseYear"] = movie.MovieReleaseYear
                };

                jsonArray.Add(jsonMovie);
            }
            string jsonData = JsonConvert.SerializeObject(Savedmovies, Formatting.Indented);
            System.IO.File.WriteAllText("C:\\Users\\lbvik\\source\\repos\\MovieManagementAPICalls\\MovieManagementAPICalls\\Storage\\movies.json", jsonData);
        }

        [HttpPut]
        public IActionResult UpdateMovie(Movie Update)
        {
            if (Update == null)
            {
                return BadRequest("INVALID");
            }

            string title = Update.MovieTitle;
            List<Movie> movies = LoadMovies();
            Movie existingMovie = movies.FirstOrDefault(movie => movie.MovieTitle == title);

            if (existingMovie != null)
            {
                existingMovie.MovieTitle = title;
                existingMovie.MovieDirector = Update.MovieDirector;
                existingMovie.MovieCategory = Update.MovieCategory;
                existingMovie.MovieReleaseYear = Update.MovieReleaseYear;

                SaveMovies(movies);

                return Ok("Movie Details Updated Successfully");
            }
            else
            {
                return NotFound("Movie not found");
            }
        }

        [HttpDelete]
        public IActionResult DeleteMovies(string titleMovie)
        {
            if (titleMovie == null)
            {
                return BadRequest("Invalid movie data.");
            }
            List<Movie> movies = LoadMovies();
            Movie movieToDelete = movies.FirstOrDefault(movie => movie.MovieTitle == titleMovie);

            if (movieToDelete != null)
            {
                movies.Remove(movieToDelete);
                SaveMovies(movies);
                return Ok("Movie Deleted Successfully");
            }
            else
            {
                return Ok("Invalid");
            }
        }
    }
}

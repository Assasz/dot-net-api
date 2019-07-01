using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using todoapi.Models;

namespace todoapi.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TodoController : ApiController
    {
        TodoEntities db = new TodoEntities();

        // GET api/todo
        public IEnumerable<todo> Get()
        {
            return db.todo.ToList().OrderBy(t => t.dueDate);
        }

        // GET api/todo/5
        public HttpResponseMessage Get(int id)
        {
            todo todo = db.todo.SingleOrDefault(t => t.id == id);

            if(todo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse<todo>(HttpStatusCode.OK, todo);
        }

        // POST api/todo
        public HttpResponseMessage Post([FromBody]todo todo)
        {
            if (ModelState.IsValid)
            {
                db.todo.Add(todo);
                db.SaveChanges();

                return Request.CreateResponse<todo>(HttpStatusCode.OK, todo);
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // PUT api/todo/5
        public HttpResponseMessage Put(int id, [FromBody]todo todo)
        {
            if (ModelState.IsValid)
            {
                todo exTodo = db.todo.SingleOrDefault(t => t.id == id);

                if(exTodo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                exTodo.title = todo.title;
                exTodo.content = todo.content;
                exTodo.dueDate = todo.dueDate;

                db.Entry(exTodo).State = EntityState.Modified;
                db.SaveChanges();

                return Request.CreateResponse<todo>(HttpStatusCode.OK, todo);
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/todo/5
        public HttpResponseMessage Delete(int id)
        {
            if (ModelState.IsValid)
            {
                todo exTodo = db.todo.SingleOrDefault(t => t.id == id);

                if (exTodo == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                db.Entry(exTodo).State = EntityState.Deleted;
                db.SaveChanges();

                return Request.CreateResponse<bool>(HttpStatusCode.OK, true);
            }

            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            //return BadRequest(ModelState);
        }

        // POST api/todo/search
        [HttpPost]
        [Route("api/todo/search")]
        public IEnumerable<todo> Search([FromBody] Dictionary<String, String> body)
        {
            string x = body["searchTerm"];

            return db.todo.Where(t => t.content.Contains(x) || t.title.Contains(x)).ToList().OrderBy(t => t.dueDate);
        }
    }
}

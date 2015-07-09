using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPITutoria.Models.Domain;
using WebAPITutoria.Repository;

namespace WebAPITutoria.Controllers
{
    [Authorize]
    public class RockBandsController : ApiController
    {
        private IObjectContextFactory _objectContextFactory;

        public RockBandsController()
        {
             _objectContextFactory = new LazySingletonObjectContextFactory();
        }


       // [EnableCors("http://localhost:51809", "*", "GET")]
        public IEnumerable<RockBand> Get()
        {
            return _objectContextFactory.Create().GetAll();
        }
        /**
        public HttpResponseMessage Get(int id)
        {
            RockBand rockband = _objectContextFactory.Create().GetById(id);
            if (rockband == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such rockband");
            }
            return Request.CreateResponse<RockBand>(HttpStatusCode.OK, rockband);


        }
        
           [Route("api/rockbands/{id:int:min(1)}/albums")]
        public HttpResponseMessage GetAlbums(int id)
        {
            RockBand rockband = _objectContextFactory.Create().GetById(id);
            if (rockband == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such rockband");
            }
            return Request.CreateResponse<IEnumerable<Album>>(HttpStatusCode.OK, rockband.Albums);
        }

         * 
        
        **/

        public IHttpActionResult Get(int id)
        {
            RockBand rockband = _objectContextFactory.Create().GetById(id);
            if (rockband == null)
            {
                return NotFound();
            }
            return Ok<RockBand>(rockband);
        }


        public IHttpActionResult GetAlbums(int id)
        {
            RockBand rockband = _objectContextFactory.Create().GetById(id);
            if (rockband == null)
            {
                return NotFound();
            }
            return Ok<IEnumerable<Album>>(rockband.Albums);
        }




    }
}

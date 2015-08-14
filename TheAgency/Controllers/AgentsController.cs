using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using TheAgency.Models;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TheAgency.Controllers
{
    public class AgentsController : ApiController
    {
        [Filters.CachingFilter(30)]
        public IEnumerable<Agent> GetAll()
        {
            return Database.Agents;
        }
        //public Agent Get(int id)
        //{
        //    Agent agent = Database.Agents.SingleOrDefault(a => a.AgentID == id);
        //    return agent;
        //}
        //Using the ETag Header to Manage Versions of Resources
        public HttpResponseMessage Get(int id)
        {
            Agent agent = Database.Agents.SingleOrDefault(a => a.AgentID == id);
            HttpResponseMessage response;
            EntityTagHeaderValue etag = Request.Headers.IfNoneMatch.FirstOrDefault();
            bool shouldReturnAgent = true;
            if (etag != null)
            {
                string etagValue = etag.Tag.Replace("\"", "");
                string currentVersion = agent.GetHashCode().ToString();
                shouldReturnAgent = (etagValue != currentVersion);
            }
            if (shouldReturnAgent)
            {
                response = Request.CreateResponse(HttpStatusCode.OK, agent);
                response.Headers.ETag = new EntityTagHeaderValue(
                string.Format("\"{0}\"", agent.GetHashCode().ToString()));
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotModified);
            }
            return response;
        }
        [HttpDelete]
        public void RemoveAgent(int id)
        {
            Agent agent = Database.Agents.SingleOrDefault(a => a.AgentID == id);
            if (agent == null)
            {
                throw new HttpResponseException(
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Content = new StringContent("Agent not found")
                    }
                );
            }
            Database.Agents.Remove(agent);
        }
        public HttpResponseMessage CreateAgent(Agent newAgent)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            // Extract the image from the image string
            string regEx = "data:(.+);base64,(.+)";
            Match match = Regex.Match(newAgent.ImagePath, regEx);
            if (match.Success)
            {
                // Get the content-type of the file and the content
                string imageType = match.Groups[1].Value;
                string base64image = match.Groups[2].Value;
                if (imageType != null && base64image != null)
                {
                    // Verify the content-type is an image
                    string imageRegEx = "image/(.+)";
                    match = Regex.Match(imageType, imageRegEx);
                    if (match.Success)
                    {
                        // Get the file extension from the content-type
                        string fileExtension = match.Groups[1].Value;
                        // Get the byte-array of the file from the base64 string
                        byte[] image = Convert.FromBase64String(base64image);
                        string path = HttpContext.Current.Server.MapPath("~/images");
                        string fileName = newAgent.FirstName + newAgent.LastName;
                        // Generate a unique name for the file (add an index to it if it already exists)
                        string targetFile = fileName + "." + fileExtension;
                        int index = 0;
                        while (File.Exists(Path.Combine(path, targetFile)))
                        {
                            index++;
                            targetFile = fileName + index + "." + fileExtension;
                        }
                        // Write the image to the target file, and update the agent with the new image path
                        File.WriteAllBytes(Path.Combine(path, targetFile), image);
                        newAgent.ImagePath = "images/" + targetFile;
                        newAgent = Database.AddAgent(newAgent);
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, newAgent);
                        string url = Url.Link("DefaultApi", new
                        {
                            controller = this.ControllerContext.ControllerDescriptor.ControllerName,
                            id = newAgent.AgentID
                        });
                        response.Headers.Location = new System.Uri(url);
                        return response;
                    }
                }
            }
            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Could not deserialize agent"));
        }
        public HttpResponseMessage Put(int id, Agent updatedAgent)
        {
            Agent agent = Database.Agents.SingleOrDefault(a => a.AgentID == id);
            // Update the task from the database
            agent.CodeName = updatedAgent.CodeName;
            agent.Description = updatedAgent.Description;
            agent.FirstName = updatedAgent.FirstName;
            agent.LastName = updatedAgent.LastName;
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
        /// <summary>
        /// The preceding example matches a scenario in which the client application uses an HTML form to submit a file 
        /// to the ASP.NET Web API service.When you use form submit to upload files, the browser uploads the content using a 
        /// special multipart/form-data MIME type
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Post()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                HttpStatusCode.UnsupportedMediaType, "Request does not contain a photo"));
            }
            string root = HttpContext.Current.Server.MapPath("~/images");
            var provider = new MultipartFormDataStreamProvider(root);
            // Read the file - this will also save the file
            await Request.Content.ReadAsMultipartAsync(provider);
            // Change the file name from a temp name to the original name
            string fileName = provider.FileData[0].Headers.ContentDisposition.FileName.Replace("\"", "");
            string tempPath = provider.FileData[0].LocalFileName;
            string newPath = Path.Combine(Path.GetDirectoryName(tempPath), fileName);
            File.Move(tempPath, newPath);
            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            return response;
        }

        /// <summary>
        /// Submitting files using HTML forms is one of the common ways to upload content to a service. Another way of
        /// uploading files from browsers is to use the new File API of HTML5.The File API uploads files as binary streams, 
        /// which makes the process of reading them on the service side a lot easier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("api/agents/{id}/photo")]
        //public async Task<HttpResponseMessage> UploadPhoto(int id)
        //{
        //    Agent agent = Database.Agents.SingleOrDefault(a => a.AgentID == id);
        //    Stream imageStream = await Request.Content.ReadAsStreamAsync();
        //    string path = GetImagePathForAgent(agent);
        //    using (FileStream fs = new FileStream(path, FileMode.Create))
        //    {
        //        await imageStream.CopyToAsync(fs);
        //        fs.Close();
        //    }
        //    imageStream.Close();
        //    return Request.CreateResponse(HttpStatusCode.Accepted);
        //}
    }
}

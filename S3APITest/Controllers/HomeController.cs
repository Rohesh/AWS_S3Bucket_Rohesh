using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetObjectRequest = Amazon.S3.Model.GetObjectRequest;
using PutObjectRequest = Amazon.S3.Model.PutObjectRequest;

namespace AwsS3Bucket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        IAmazonS3 S3Client { get; set; }

        public HomeController(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }

        //Creating a Folder

        [HttpPost("CreateFolder")]
        public async Task<int> CreateFolder(string bucketname, string newFolderName, string prefix = "")
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = bucketname;
            request.Key = (prefix.TrimEnd('/') + "/" + newFolderName.TrimEnd('/') + "/").TrimStart('/');
            var response = await S3Client.PutObjectAsync(request);
            return (int)response.HttpStatusCode;

        }

        //Sample File
        public string filePath = "D:\\Test.json";
        //To Upload a FIle
        [HttpPost("UploadFile")]
        public async Task<int> UploadFile(string bucketName, string keyName)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);

            try
            {
                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,

                    Key = keyName,
                    FilePath = filePath,
                    ContentType = "text/plain"
                };

                var response = await S3Client.PutObjectAsync(putRequest);
                return (int)response.HttpStatusCode;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    return 400;
                }
                else
                {
                    return 400;
                }

            }
        }

        //To Get all the objects

        [HttpGet("GetS3Object")]
        public async Task<string> GetS3Object(string BUCKET_NAME, string S3_KEY)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            GetObjectRequest request = new GetObjectRequest();

            request.BucketName = BUCKET_NAME;
            request.Key = S3_KEY;

            GetObjectResponse response = await client.GetObjectAsync(request);

            StreamReader reader = new StreamReader(response.ResponseStream);

            string content = reader.ReadToEnd();
           
            Console.Out.WriteLine("Read S3 object with key " + S3_KEY + " in bucket " + BUCKET_NAME + ". Content is: " + content);
            return content;
        }

        //To Get a specific ID
        [HttpGet("GetObjectbyID")]
        public async Task<masterjson> GetObjectbyID(int id)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            GetObjectRequest request = new GetObjectRequest();

            request.BucketName = "elasticbeanstalk-ap-south-1-958370057608";
            request.Key = "masterjson";

            GetObjectResponse response = await client.GetObjectAsync(request);

            StreamReader reader = new StreamReader(response.ResponseStream);

            string content = reader.ReadToEnd();
            List<masterjson> dese = JsonConvert.DeserializeObject<List<masterjson>>(content);
            masterjson result = dese.Single(a => a.id == id);

            return result;
        }

        //To Create A record
        [HttpPost("CreateRecord")]
        public async Task<string> CreateRecord(string firstname, string lastname, int id)
        {
            masterjson inst = new masterjson();
            inst.firstName = firstname;
            inst.lastName = lastname;
            inst.id = id;
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            GetObjectRequest request = new GetObjectRequest();

            request.BucketName = "elasticbeanstalk-ap-south-1-958370057608";
            request.Key = "masterjson";


            GetObjectResponse response = await client.GetObjectAsync(request);

            StreamReader reader = new StreamReader(response.ResponseStream);

            string content = reader.ReadToEnd();
            List<masterjson> dese = JsonConvert.DeserializeObject<List<masterjson>>(content);
            dese.Add(inst);
            //string serializedval =JsonSerializer. .ser(dese);
            string serializedval = JsonConvert.SerializeObject(dese);

            System.IO.File.WriteAllText(filePath, string.Empty);
            System.IO.File.WriteAllText(filePath, serializedval);

            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = "elasticbeanstalk-ap-south-1-958370057608",

                Key = "masterjson",
                FilePath = filePath,
                ContentType = "text/plain"
            };

            var response2 = S3Client.PutObjectAsync(putRequest);
            return content;

        }

        //To Update a Record
        [HttpPost("UpdateRecord")]
        public async Task<string> UpdateRecord(string firstname, string lastname, int id)
        {
            masterjson inst = new masterjson();
            inst.firstName = firstname;
            inst.lastName = lastname;
            inst.id = id;
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            GetObjectRequest request = new GetObjectRequest();

            request.BucketName = "elasticbeanstalk-ap-south-1-958370057608";
            request.Key = "masterjson";


            GetObjectResponse response = await client.GetObjectAsync(request);

            StreamReader reader = new StreamReader(response.ResponseStream);

            string content = reader.ReadToEnd();
            List<masterjson> dese = JsonConvert.DeserializeObject<List<masterjson>>(content);
            dese.RemoveAll(x => x.id == id);
            dese.Add(inst);
            
            string serializedval = JsonConvert.SerializeObject(dese);

            System.IO.File.WriteAllText(filePath, string.Empty);
            System.IO.File.WriteAllText(filePath, serializedval);

            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = "elasticbeanstalk-ap-south-1-958370057608",

                Key = "masterjson",
                FilePath = filePath,
                ContentType = "text/plain"
            };

            var response2 = S3Client.PutObjectAsync(putRequest);
            return content;

        }

        //To delete a Record
        [HttpDelete("DeleteRecord")]
        public  async Task<string> DeleteRecord(int id)
        {
            masterjson inst = new masterjson();

            inst.id = id;
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            GetObjectRequest request = new GetObjectRequest();

            request.BucketName = "elasticbeanstalk-ap-south-1-958370057608";
            request.Key = "masterjson";


            GetObjectResponse response = await client.GetObjectAsync(request);

            StreamReader reader = new StreamReader(response.ResponseStream);

            string content = reader.ReadToEnd();
            List<masterjson> dese = JsonConvert.DeserializeObject<List<masterjson>>(content);
            dese.RemoveAll(x => x.id == id);
            
            string serializedval = JsonConvert.SerializeObject(dese);

            System.IO.File.WriteAllText(filePath, string.Empty);
            System.IO.File.WriteAllText(filePath, serializedval);

            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = "elasticbeanstalk-ap-south-1-958370057608",

                Key = "masterjson",
                FilePath = filePath,
                ContentType = "text/plain"
            };

            var response2 = await S3Client.PutObjectAsync(putRequest);
            return "successfully deleted";

        }
       
        public class masterjson
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public int id { get; set; }

        }

    }
}


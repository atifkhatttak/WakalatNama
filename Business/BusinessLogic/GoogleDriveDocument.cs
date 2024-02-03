﻿using Business.Services;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Drive.v3.DriveService;
using Google.Apis.Download;
using Data.DomainModels;
using Data.Context;
using Microsoft.Extensions.Configuration;

namespace Business.BusinessLogic
{
    public class GoogleDriveDocument : BaseRepository<UserDocument>,IDocumentService
    {
        private readonly IConfiguration config;

        private DriveService driveService { get; set; }

        public GoogleDriveDocument(WKNNAMADBCtx ctx,IConfiguration config): base(ctx)
        {
           
            this.config = config;
            Init();
        }
        public void Init()
        {
           var driveConfig= config.GetSection("GoogleDrive");

            var tokenResponse = new TokenResponse
            {
                AccessToken = driveConfig.GetSection("AccessToken").Value,//  "ya29.a0AfB_byC02UjGVY-mdBPVz1jqCvp419Qk4QpUGqpWAkesIW38k89InpMn6ldDrivCd12q-f5aEKtJSBIfTux-kB_6v6yOP6ojBLq6shJ4SKQ6lOcMhlCa5hLoZVJrKC3iKfYU6VC4Sx7gKXg0f-90x2ESLYJUBY5_nBE7aCgYKAdQSARISFQHGX2Mifk420ftkCVv7ekPCiyZTcQ0171",
                RefreshToken = driveConfig.GetSection("RefreshToken").Value //"1//04HhMAm7YMVOcCgYIARAAGAQSNwF-L9IrXYSwbH3pOo3cSoGQTNgIDlT9EtNrQhIuaxUAqUFb9WKgxWSfyt-ZgUn8KhfdjBI8qu8",
            };


            var applicationName = driveConfig.GetSection("App").Value;// "WakalatNameApp"; // Use the name of the project in Google Cloud
            var username = driveConfig.GetSection("User").Value; // "atifkhattak804@gmail.com"; // Use your email


            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = driveConfig.GetSection("ClientId").Value,// "431477244184-7rhgorhk7hucu4jbhqel55b458jr4tgs.apps.googleusercontent.com",
                    ClientSecret = driveConfig.GetSection("ClientSecret").Value// "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"
                },
                Scopes = new[] { Scope.Drive },
                DataStore = new FileDataStore(applicationName)
            });


            var credential = new UserCredential(apiCodeFlow, username, tokenResponse);


    var service = new DriveService(new BaseClientService.Initializer
    {
        HttpClientInitializer = credential,
        ApplicationName = applicationName
    });

      driveService=service;

        }

        public Google.Apis.Drive.v3.Data.File CreateFolder(string folderName)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };

            var command = driveService.Files.Create(fileMetadata);
            var file = command.Execute();
            return file;
        }

        public Google.Apis.Drive.v3.Data.File UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription)
        {


    var driveFile = new Google.Apis.Drive.v3.Data.File();
            driveFile.Name = fileName;
            driveFile.Description = fileDescription;
            driveFile.MimeType = fileMime;
            driveFile.Parents = new string[] { folder };


    var request = driveService.Files.Create(driveFile, file, fileMime);
            request.Fields = "id";

            var response = request.Upload();

            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw response.Exception;



            return request.ResponseBody;
        }
        public void DeleteFile(string fileId)
        {
            var command = driveService.Files.Delete(fileId);
            var result = command.Execute();
        }
        public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string folder)
        {

            var fileList = driveService.Files.List();
            fileList.Q = $"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
            fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";

            var result = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {
                fileList.PageToken = pageToken;
                var filesResult = fileList.Execute();
                var files = filesResult.Files;
                pageToken = filesResult.NextPageToken;
                result.AddRange(files);
            } while (pageToken != null);


    return result;
        }


        public   MemoryStream DownloadFile(string fileId)
        {
            try
            { 

                var request = driveService.Files.Get(fileId);
                var stream = new MemoryStream();

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                    progress =>
                    {
                        switch (progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                {
                                    Console.WriteLine(progress.BytesDownloaded);
                                    break;
                                }
                            case DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    break;
                                }
                            case DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    break;
                                }
                        }
                    };
                request.Download(stream);

                return stream;
            }
            catch (Exception e)
            {
                // TODO(developer) - handle error appropriately
                if (e is AggregateException)
                {
                    Console.WriteLine("Credential Not found");
                }
                else
                {
                    throw;
                }
            }
            return null;
        }

        //public MemoryStream CreateFolder(string fileId)
        //{
        //    try
        //    {
        //        var service = Init();

        //        var request = service.Files.Get(fileId);
        //        var stream = new MemoryStream();

        //        // Add a handler which will be notified on progress changes.
        //        // It will notify on each chunk download and when the
        //        // download is completed or failed.
        //        request.MediaDownloader.ProgressChanged +=
        //            progress =>
        //            {
        //                switch (progress.Status)
        //                {
        //                    case DownloadStatus.Downloading:
        //                        {
        //                            Console.WriteLine(progress.BytesDownloaded);
        //                            break;
        //                        }
        //                    case DownloadStatus.Completed:
        //                        {
        //                            Console.WriteLine("Download complete.");
        //                            break;
        //                        }
        //                    case DownloadStatus.Failed:
        //                        {
        //                            Console.WriteLine("Download failed.");
        //                            break;
        //                        }
        //                }
        //            };
        //        request.Download(stream);

        //        return stream;
        //    }
        //    catch (Exception e)
        //    {
        //        // TODO(developer) - handle error appropriately
        //        if (e is AggregateException)
        //        {
        //            Console.WriteLine("Credential Not found");
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    return null;
        //}
    }
}
    


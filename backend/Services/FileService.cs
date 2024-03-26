﻿using backend.Entities;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }

    public class FileService : IFileService
    {
        private FileShareDbContext _dbContext;
        private IUserContextService _userContextService;
        public FileService(FileShareDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public async Task<string> UploadFileAsync(IFormFile File)
        {
            // 1. Check if file is not empty
            if (File == null || File.Length == 0)
            {
                throw new Exception("File is empty");
            }

            // 2. Check if user is authenticated
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            // 3. Create a new file entity
            var fileEntity = new backend.Entities.File
            {
                Name = File.FileName,
                OwnerId = userId.Value,
                UploadedAt = DateTime.UtcNow
            };

            // 4. Generate unique file name
            var filePath = Guid.NewGuid().ToString() + "_" + File.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", filePath);

            // 5. Save the file to disk
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            // 6. Save the file path to the database
            fileEntity.Path = path;
            _dbContext.Files.Add(fileEntity);
            await _dbContext.SaveChangesAsync();
            return filePath;
        }

        // Create an method which will return all files of logged user
        public async Task<IEnumerable<FileModel>> GetAllFilesAsync()
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var files = await _dbContext.Files
                .Where(file => file.OwnerId == userId)
                .Select(file => new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    UploadedAt = file.UploadedAt
                })
                .ToListAsync();

            return files;
        }



    }
}
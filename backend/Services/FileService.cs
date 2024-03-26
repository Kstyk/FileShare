using backend.Entities;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string[]> UploadFilesAsync(IFormFile[] files);
        Task<IEnumerable<FileModelDto>> GetAllFilesAsync();
        Task DeleteFileAsync(int fileId);
        Task<FileModelDto> GetFileByIdAsync(int fileId);
        Task<FileStream> DownloadFileAsync(int fileId);
        Task DeleteFilesAsync(int[] fileIds);
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

        public async Task<string[]> UploadFilesAsync(IFormFile[] files)
        {
            if (files == null || files.Length == 0)
            {
                throw new Exception("Files are empty");
            }

            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var filePaths = new List<string>();

            foreach (var file in files)
            {
                var fileEntity = new backend.Entities.File
                {
                    Name = file.FileName,
                    OwnerId = userId.Value,
                    UploadedAt = DateTime.UtcNow
                };

                var filePath = Guid.NewGuid().ToString() + "_" + file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", filePath);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                fileEntity.Path = path;
                _dbContext.Files.Add(fileEntity);
                await _dbContext.SaveChangesAsync();
                filePaths.Add(filePath);
            }

            return filePaths.ToArray();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty");
            }

            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var fileEntity = new backend.Entities.File
            {
                Name = file.FileName,
                OwnerId = userId.Value,
                UploadedAt = DateTime.UtcNow
            };

            var filePath = Guid.NewGuid().ToString() + "_" + file.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", filePath);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            fileEntity.Path = path;
            _dbContext.Files.Add(fileEntity);
            await _dbContext.SaveChangesAsync();
            return filePath;
        }

        // Create an method which will return all files of logged user
        public async Task<IEnumerable<FileModelDto>> GetAllFilesAsync()
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var files = await _dbContext.Files
                .Where(f => f.OwnerId == userId)
                .Select(f => new FileModelDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Path = f.Path,
                    UploadedAt = f.UploadedAt
                })
                .ToListAsync();

            return files;
        }
        
        // Delete file by id with using transaction
        public async Task DeleteFileAsync(int fileId)
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var file = await _dbContext.Files
                .Where(f => f.OwnerId == userId && f.Id == fileId)
                .FirstOrDefaultAsync();

            if (file == null)
            {
                throw new Exception("File not found");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.Files.Remove(file);
                    await _dbContext.SaveChangesAsync();
                    System.IO.File.Delete(file.Path);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        // Delete few files by ids with using transaction
        public async Task DeleteFilesAsync(int[] fileIds)
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var files = await _dbContext.Files
                .Where(f => f.OwnerId == userId && fileIds.Contains(f.Id))
                .ToListAsync();

            if (files == null || files.Count == 0)
            {
                throw new Exception("Files not found");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.Files.RemoveRange(files);
                    await _dbContext.SaveChangesAsync();

                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file.Path);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        // Create an method which will return file by id
        public async Task<FileModelDto> GetFileByIdAsync(int fileId)
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var file = await _dbContext.Files
                .Where(f => f.OwnerId == userId && f.Id == fileId)
                .Select(f => new FileModelDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Path = f.Path,
                    UploadedAt = f.UploadedAt
                })
                .FirstOrDefaultAsync();

            if (file == null)
            {
                throw new Exception("File not found");
            }

            return file;
        }

        // Create an method which will return file stream by id and set the content type that is what the file is
        public async Task<FileStream> DownloadFileAsync(int fileId)
        {
            var userId = _userContextService.GetUserId;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var file = await _dbContext.Files
                .Where(f => f.OwnerId == userId && f.Id == fileId)
                .FirstOrDefaultAsync();

            if (file == null)
            {
                throw new Exception("File not found");
            }

            var stream = new FileStream(file.Path, FileMode.Open);
            return stream;
        }
        
        
    }
}
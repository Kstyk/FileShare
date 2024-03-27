using AutoMapper;
using backend.Entities;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

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
        Task<FileStream> DownloadFilesAsync(int[] fileIds);
        Task<FileModelDto> ChangeFilePublicAsync(int fileId, bool isPublic);
    }

    public class FileService : IFileService
    {
        private FileShareDbContext _dbContext;
        private IUserContextService _userContextService;
        private readonly IMapper _mapper;
        public FileService(FileShareDbContext dbContext, IUserContextService userContextService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
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

            // Get files from database by userId and map them to FileModelDto
            var files = await _dbContext.Files
                .Where(f => f.OwnerId == userId)
                .Select(f => _mapper.Map<FileModelDto>(f))
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

            // Get file from database by userId and fileId and map it to FileModelDto
            var file = await _dbContext.Files
                .Where(f => f.OwnerId == userId && f.Id == fileId)
                .Select(f => _mapper.Map<FileModelDto>(f))
                .FirstOrDefaultAsync();

            if (file == null)
            {
                throw new Exception("File not found");
            }

            return file;
        }

        // Create an method which will return file stream by id and set the content type that is what the file is
        // Also increment the downloads count - use transaction for that
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

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    file.Downloads++;
                    _dbContext.Files.Update(file);
                    await _dbContext.SaveChangesAsync();

                    var stream = new FileStream(file.Path, FileMode.Open);
                    transaction.Commit();
                    return stream;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        // Download a few files by ids, increment the downloads count - use transaction for that, pack downloaded files into zip
        public async Task<FileStream> DownloadFilesAsync(int[] fileIds)
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
                    foreach (var file in files)
                    {
                        file.Downloads++;
                        _dbContext.Files.Update(file);
                    }

                    await _dbContext.SaveChangesAsync();

                    var zipPath = Path.Combine(Directory.GetCurrentDirectory(), "Files", Guid.NewGuid().ToString() + ".zip");

                    using (var zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                    {
                        foreach (var file in files)
                        {
                            zipArchive.CreateEntryFromFile(file.Path, file.Name);
                        }
                    }

                    var stream = new FileStream(zipPath, FileMode.Open);
                    transaction.Commit();
                    return stream;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        // Change isPublic property of file by id and return the updated file
        public async Task<FileModelDto> ChangeFilePublicAsync(int fileId, bool isPublic)
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

            file.IsPublic = isPublic;
            _dbContext.Files.Update(file);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<FileModelDto>(file);
        }
        
        
    }
}
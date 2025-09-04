using AutoMapper;
using DoctorAppointmentAPI.DTOs;
using DoctorAppointmentAPI.Models;
using MongoDB.Driver;

namespace DoctorAppointmentAPI.Services
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync();
        Task<SpecialtyDto?> GetSpecialtyByIdAsync(string id);
        Task<SpecialtyDto> CreateSpecialtyAsync(CreateSpecialtyDto createSpecialtyDto);
        Task<SpecialtyDto?> UpdateSpecialtyAsync(string id, UpdateSpecialtyDto updateSpecialtyDto);
        Task<bool> DeleteSpecialtyAsync(string id);
    }

    public class SpecialtyService : ISpecialtyService
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IMapper _mapper;
        private readonly ILogger<SpecialtyService> _logger;

        public SpecialtyService(
            IMongoDbService mongoDbService,
            IMapper mapper,
            ILogger<SpecialtyService> logger)
        {
            _mongoDbService = mongoDbService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SpecialtyDto>> GetAllSpecialtiesAsync()
        {
            var specialties = await _mongoDbService.Specialties
                .Find(s => s.IsActive)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SpecialtyDto>>(specialties);
        }

        public async Task<SpecialtyDto?> GetSpecialtyByIdAsync(string id)
        {
            var specialty = await _mongoDbService.Specialties
                .Find(s => s.Id == id && s.IsActive)
                .FirstOrDefaultAsync();

            return specialty != null ? _mapper.Map<SpecialtyDto>(specialty) : null;
        }

        public async Task<SpecialtyDto> CreateSpecialtyAsync(CreateSpecialtyDto createSpecialtyDto)
        {
            // Check if specialty with same name already exists
            var existingSpecialty = await _mongoDbService.Specialties
                .Find(s => s.Name.ToLower() == createSpecialtyDto.Name.ToLower() && s.IsActive)
                .FirstOrDefaultAsync();

            if (existingSpecialty != null)
                throw new ArgumentException("Specialty with this name already exists");

            var specialty = _mapper.Map<Specialty>(createSpecialtyDto);
            specialty.CreatedAt = DateTime.UtcNow;
            specialty.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Specialties.InsertOneAsync(specialty);

            return _mapper.Map<SpecialtyDto>(specialty);
        }

        public async Task<SpecialtyDto?> UpdateSpecialtyAsync(string id, UpdateSpecialtyDto updateSpecialtyDto)
        {
            var specialty = await _mongoDbService.Specialties
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync();

            if (specialty == null)
                return null;

            if (updateSpecialtyDto.Name != null)
            {
                // Check if another specialty with same name exists
                var existingSpecialty = await _mongoDbService.Specialties
                    .Find(s => s.Name.ToLower() == updateSpecialtyDto.Name.ToLower() && 
                              s.IsActive && s.Id != id)
                    .FirstOrDefaultAsync();

                if (existingSpecialty != null)
                    throw new ArgumentException("Specialty with this name already exists");

                specialty.Name = updateSpecialtyDto.Name;
            }

            if (updateSpecialtyDto.Description != null)
                specialty.Description = updateSpecialtyDto.Description;

            if (updateSpecialtyDto.IsActive.HasValue)
                specialty.IsActive = updateSpecialtyDto.IsActive.Value;

            specialty.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Specialties.ReplaceOneAsync(s => s.Id == id, specialty);

            return _mapper.Map<SpecialtyDto>(specialty);
        }

        public async Task<bool> DeleteSpecialtyAsync(string id)
        {
            var specialty = await _mongoDbService.Specialties
                .Find(s => s.Id == id)
                .FirstOrDefaultAsync();

            if (specialty == null)
                return false;

            // Check if any doctors are associated with this specialty
            var doctorsWithSpecialty = await _mongoDbService.Doctors
                .Find(d => d.SpecialtyId == id && d.IsActive)
                .CountDocumentsAsync();

            if (doctorsWithSpecialty > 0)
                throw new InvalidOperationException("Cannot delete specialty that has associated doctors");

            specialty.IsActive = false;
            specialty.UpdatedAt = DateTime.UtcNow;

            await _mongoDbService.Specialties.ReplaceOneAsync(s => s.Id == id, specialty);

            return true;
        }
    }
}

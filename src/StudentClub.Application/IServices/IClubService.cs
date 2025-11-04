using StudentClub.Application.DTOs.request;
using StudentClub.Application.DTOs.response;
using StudentClub.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IClubService
    {
        Task<CreateClubResponseDto> CreateClubAsync(CreateClubRequestDto createClubRequestDto);

        Task<UpdateClubResponseDto> UpdateClubAsync(UpdateClubRequestDto updateClubRequestDto);

        Task<List<GetAllClubsResponseDto>> GetAllClubAsync();
        Task<GetClubResponseDto> GetClubAsync(int clubId);
        Task DeleteClubAsync(int clubId);
    }
}

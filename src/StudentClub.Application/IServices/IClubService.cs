using StudentClub.Application.DTOs;
using StudentClub.Application.DTOs.Clubs;
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
        Task DeleteClubAsync(int clubId);
    }
}

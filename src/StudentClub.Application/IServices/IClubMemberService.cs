using StudentClub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IClubMemberService
    {
        Task<CreateClubMemberResponseDto> CreateClubMemberAsync(CreateClubMemberRequestDto createClubMemberRequestDto);
    }
}

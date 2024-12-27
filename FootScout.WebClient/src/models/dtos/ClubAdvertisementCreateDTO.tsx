import SalaryRangeCreateDTO from "./SalaryRangeCreateDTO";

interface ClubAdvertisementCreateDTO {
    playerPositionId: number;
    clubName: string;
    league: string;
    region: string;
    salaryRangeDTO: SalaryRangeCreateDTO;
    clubMemberId: string;
}

export default ClubAdvertisementCreateDTO;
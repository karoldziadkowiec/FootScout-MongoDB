import SalaryRangeCreateDTO from "./SalaryRangeCreateDTO";

interface PlayerAdvertisementCreateDTO {
    playerPositionId: number;
    league: string;
    region: string;
    age: number;
    height: number;
    playerFootId: number;
    salaryRangeDTO: SalaryRangeCreateDTO;
    playerId: string;
}

export default PlayerAdvertisementCreateDTO;
import PlayerPosition from './PlayerPosition';
import SalaryRange from "./SalaryRange";
import UserDTO from '../dtos/UserDTO';

interface ClubAdvertisement {
    id: number;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    clubName: string;
    league: string;
    region: string;
    salaryRangeId: number;
    salaryRange: SalaryRange;
    creationDate: string;
    endDate: string;
    clubMemberId: string;
    clubMember: UserDTO;
}

export default ClubAdvertisement;
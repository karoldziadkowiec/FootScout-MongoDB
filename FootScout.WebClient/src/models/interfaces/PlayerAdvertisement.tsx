import PlayerPosition from './PlayerPosition';
import PlayerFoot from './PlayerFoot';
import SalaryRange from "./SalaryRange";
import UserDTO from '../dtos/UserDTO';

interface PlayerAdvertisement {
    id: number;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    league: string;
    region: string;
    age: number;
    height: number;
    playerFootId: number;
    playerFoot: PlayerFoot;
    salaryRangeId: number;
    salaryRange: SalaryRange;
    creationDate: string;
    endDate: string;
    playerId: string;
    player: UserDTO;
}

export default PlayerAdvertisement;
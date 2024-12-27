import Achievements from '../dtos/AchievementsDTO';
import UserDTO from '../dtos/UserDTO';
import PlayerPosition from './PlayerPosition';

interface ClubHistoryModel {
    id: number;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    clubName: string;
    league: string;
    region: string;
    startDate: string;
    endDate: string;
    achievementsId: number;
    achievements: Achievements;
    playerId: string;
    player: UserDTO;
}

export default ClubHistoryModel;
import UserDTO from '../dtos/UserDTO';
import PlayerAdvertisement from './PlayerAdvertisement';

interface FavoritePlayerAdvertisement {
    id: number;
    playerAdvertisementId: number;
    playerAdvertisement: PlayerAdvertisement;
    userId: string;
    user: UserDTO;
}

export default FavoritePlayerAdvertisement;
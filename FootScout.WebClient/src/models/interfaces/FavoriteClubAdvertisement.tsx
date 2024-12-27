import UserDTO from '../dtos/UserDTO';
import ClubAdvertisement from './ClubAdvertisement';

interface FavoriteClubAdvertisement {
    id: number;
    clubAdvertisementId: number;
    clubAdvertisement: ClubAdvertisement;
    userId: string;
    user: UserDTO;
}

export default FavoriteClubAdvertisement;
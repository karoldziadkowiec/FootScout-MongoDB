import ClubAdvertisement from './ClubAdvertisement';
import OfferStatus from './OfferStatus';
import PlayerPosition from './PlayerPosition';
import PlayerFoot from './PlayerFoot';
import UserDTO from '../dtos/UserDTO';

interface PlayerOffer {
    id: number;
    clubAdvertisementId: number;
    clubAdvertisement: ClubAdvertisement;
    offerStatusId: number;
    offerStatus: OfferStatus;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    age: number;
    height: number;
    playerFootId: number;
    playerFoot: PlayerFoot;
    salary: number;
    additionalInformation: string;
    creationDate: string;
    playerId: string;
    player: UserDTO;
}

export default PlayerOffer;
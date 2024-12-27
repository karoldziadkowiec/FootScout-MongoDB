import PlayerAdvertisement from './PlayerAdvertisement';
import OfferStatus from './OfferStatus';
import PlayerPosition from './PlayerPosition';
import UserDTO from '../dtos/UserDTO';

interface ClubOffer {
    id: number;
    playerAdvertisementId: number;
    playerAdvertisement: PlayerAdvertisement;
    offerStatusId: number;
    offerStatus: OfferStatus;
    playerPositionId: number;
    playerPosition: PlayerPosition;
    clubName: string;
    league: string;
    region: string;
    salary: number;
    additionalInformation: string;
    creationDate: string;
    clubMemberId: string;
    clubMember: UserDTO;
}

export default ClubOffer;
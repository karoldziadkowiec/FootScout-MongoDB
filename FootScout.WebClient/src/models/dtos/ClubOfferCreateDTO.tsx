interface ClubOfferCreateDTO {
    playerAdvertisementId: number;
    playerPositionId: number;
    clubName: string;
    league: string;
    region: string;
    salary: number;
    additionalInformation: string;
    clubMemberId: string;
}

export default ClubOfferCreateDTO;
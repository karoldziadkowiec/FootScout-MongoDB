import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Form, Row, Col, Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import ClubOfferService from '../../services/api/ClubOfferService';
import PlayerOfferService from '../../services/api/PlayerOfferService';
import ChatService from '../../services/api/ChatService';
import OfferStatusName from '../../models/enums/OfferStatusName';
import ClubOffer from '../../models/interfaces/ClubOffer';
import PlayerOffer from '../../models/interfaces/PlayerOffer';
import ClubHistoryModel from '../../models/interfaces/ClubHistory';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import '../../App.css';
import '../../styles/user/MyOffersAsPlayer.css';

const MyOffersAsPlayer = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [receivedClubOffers, setReceivedClubOffers] = useState<ClubOffer[]>([]);
    const [sentPlayerOffers, setSentPlayerOffers] = useState<PlayerOffer[]>([])
    const [showReceivedClubOfferDetailsModal, setShowReceivedClubOfferDetailsModal] = useState<boolean>(false);
    const [showSentPlayerOfferDetailsModal, setShowSentPlayerOfferDetailsModal] = useState<boolean>(false);
    const [playerClubHistories, setPlayerClubHistories] = useState<ClubHistoryModel[]>([]);
    const [showAcceptReceivedClubOfferModal, setShowAcceptReceivedClubOfferModal] = useState<boolean>(false);
    const [showRejectReceivedClubOfferModal, setShowRejectReceivedClubOfferModal] = useState<boolean>(false);
    const [showDeleteSentPlayerOfferModal, setShowDeleteSentPlayerOfferModal] = useState<boolean>(false);
    const [showClubHistoryDetailsModal, setShowClubHistoryDetailsModal] = useState<boolean>(false);
    const [selectedReceivedClubOffer, setSelectedReceivedClubOffer] = useState<ClubOffer | null>(null);
    const [selectedSentPlayerOffer, setSelectedSentPlayerOffer] = useState<PlayerOffer | null>(null);
    const [selectedClubHistory, setSelectedClubHistory] = useState<ClubHistoryModel | null>(null);
    const [receivedClubOfferToAccept, setReceivedClubOfferToAccept] = useState<ClubOffer | null>(null);
    const [receivedClubOfferToReject, setReceivedClubOfferToReject] = useState<ClubOffer | null>(null);
    const [deleteSentPlayerOfferId, setDeleteSentPlayerOfferId] = useState<number | null>(null);

    useEffect(() => {
        if (location.state && location.state.toastMessage)
            toast.success(location.state.toastMessage);

        const fetchUserData = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId) {
                    setUserId(userId);

                    const _receivedClubOffers = await UserService.getReceivedClubOffers(userId);
                    setReceivedClubOffers(_receivedClubOffers);

                    const _sentPlayerOffers = await UserService.getSentPlayerOffers(userId);
                    setSentPlayerOffers(_sentPlayerOffers);
                }
            }
            catch (error) {
                console.error('Failed to fetch user\'s data:', error);
                toast.error('Failed to load user\'s data.');
            }
        };

        fetchUserData();
    }, [location]);

    const handleShowReceivedClubOfferDetails = (clubOffer: ClubOffer) => {
        setSelectedReceivedClubOffer(clubOffer);
        setShowReceivedClubOfferDetailsModal(true);
    };

    const handleShowAcceptReceivedClubOfferModal = (clubOffer: ClubOffer) => {
        setReceivedClubOfferToAccept(clubOffer);
        setShowAcceptReceivedClubOfferModal(true);
    };

    const handleAcceptReceivedClubOffer = async () => {
        if (!receivedClubOfferToAccept || !userId)
            return;

        try {
            const updatedFormData = {
                ...receivedClubOfferToAccept
            };

            await ClubOfferService.acceptClubOffer(receivedClubOfferToAccept.id, updatedFormData);
            setShowAcceptReceivedClubOfferModal(false);
            toast.success('Received club offer has been accepted successfully.');
            // Refresh data
            const _receivedClubOffers = await UserService.getReceivedClubOffers(userId);
            setReceivedClubOffers(_receivedClubOffers);
        }
        catch (error) {
            console.error('Failed to accept received club offer:', error);
            toast.error('Failed to accept received club offer.');
        }
    };

    const handleShowRejectReceivedClubOfferModal = (clubOffer: ClubOffer) => {
        setReceivedClubOfferToReject(clubOffer);
        setShowRejectReceivedClubOfferModal(true);
    };

    const handleRejectReceivedClubOffer = async () => {
        if (!receivedClubOfferToReject || !userId)
            return;

        try {
            const updatedFormData = {
                ...receivedClubOfferToReject
            };

            await ClubOfferService.rejectClubOffer(receivedClubOfferToReject.id, updatedFormData);
            setShowRejectReceivedClubOfferModal(false);
            toast.success('Received club offer has been rejected successfully.');
            // Refresh data
            const _receivedClubOffers = await UserService.getReceivedClubOffers(userId);
            setReceivedClubOffers(_receivedClubOffers);
        }
        catch (error) {
            console.error('Failed to reject received club offer:', error);
            toast.error('Failed to reject received club offer.');
        }
    };

    const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
        navigate(`/player-advertisement/${playerAdvertisementId}`, { state: { playerAdvertisementId } });
    };

    const handleShowSentPlayerOfferDetails = async (playerOffer: PlayerOffer) => {
        const playerClubHistories = await UserService.getUserClubHistory(playerOffer.playerId);
        setPlayerClubHistories(playerClubHistories);

        setSelectedSentPlayerOffer(playerOffer);
        setShowSentPlayerOfferDetailsModal(true);
    };

    const moveToClubAdvertisementPage = (clubAdvertisementId: number) => {
        navigate(`/club-advertisement/${clubAdvertisementId}`, { state: { clubAdvertisementId } });
    };

    const handleShowDeleteSentPlayerOfferModal = (playerOfferId: number) => {
        setDeleteSentPlayerOfferId(playerOfferId);
        setShowDeleteSentPlayerOfferModal(true);
    };

    const handleDeleteSentPlayerOffer = async () => {
        if (!deleteSentPlayerOfferId)
            return;

        try {
            await PlayerOfferService.deletePlayerOffer(deleteSentPlayerOfferId);
            toast.success('Your sent player offer has been deleted successfully.');
            setShowDeleteSentPlayerOfferModal(false);
            setShowSentPlayerOfferDetailsModal(false);
            setDeleteSentPlayerOfferId(null);

            // Refresh the user data
            if (userId) {
                const _receivedClubOffers = await UserService.getReceivedClubOffers(userId);
                setReceivedClubOffers(_receivedClubOffers);

                const _sentPlayerOffers = await UserService.getSentPlayerOffers(userId);
                setSentPlayerOffers(_sentPlayerOffers);
            }
        }
        catch (error) {
            console.error('Failed to delete sent club offer:', error);
            toast.error('Failed to delete sent club offer.');
        }
    };

    const handleShowClubHistoryDetails = (clubHistory: ClubHistoryModel) => {
        setSelectedClubHistory(clubHistory);
        setShowClubHistoryDetailsModal(true);
    };

    const handleOpenChat = async (receiverId: string) => {
        if (!receiverId || !userId)
            return;

        try {
            let chatId = await ChatService.getChatIdBetweenUsers(userId, receiverId);

            if (chatId === 0) {
                const chatCreateDTO: ChatCreateDTO = {
                    user1Id: userId,
                    user2Id: receiverId
                };

                await ChatService.createChat(chatCreateDTO);
                chatId = await ChatService.getChatIdBetweenUsers(userId, receiverId);
            }
            navigate(`/chat/${chatId}`, { state: { chatId } });
        }
        catch (error) {
            console.error('Failed to open chat:', error);
            toast.error('Failed to open chat.');
        }
    };

    return (
        <div className="MyOffersAsPlayer">
            <h1><i className="bi bi-briefcase"></i> My Offers as a Player</h1>
            <p></p>
            {/* Received offers from clubs*/}
            <Tabs defaultActiveKey="received" id="problem-tabs" className="mb-3 custom-tabs">
                <Tab eventKey="received" title="Received offers from clubs">
                    <h3><i className="bi bi-arrow-down-left-square"></i> Received offers from clubs</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-dark">
                                <tr>
                                    <th>Received Date</th>
                                    <th>Offer Status</th>
                                    <th>Club Name</th>
                                    <th>League (Region)</th>
                                    <th>Position</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {receivedClubOffers.length > 0 ? (
                                    receivedClubOffers.map((clubOffer, index) => (
                                        <tr key={index}>
                                            <td className="offer-row">{TimeService.formatDateToEUR(clubOffer.creationDate)}</td>
                                            <td className="offer-row">
                                                {clubOffer.offerStatus.statusName === "Offered" && (
                                                    <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                                )}
                                                {clubOffer.offerStatus.statusName === "Accepted" && (
                                                    <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                                )}
                                                {clubOffer.offerStatus.statusName === "Rejected" && (
                                                    <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                                )}
                                                {clubOffer.offerStatus.statusName}
                                            </td>
                                            <td className="offer-row">{clubOffer.clubName}</td>
                                            <td className="offer-row">{clubOffer.league} ({clubOffer.region})</td>
                                            <td className="offer-row">{clubOffer.playerPosition.positionName}</td>
                                            <td className="offer-row">
                                                <Button variant="primary" className="button-spacing" onClick={() => handleShowReceivedClubOfferDetails(clubOffer)}>
                                                    <i className="bi bi-info-circle"></i> Offer
                                                </Button>
                                                {clubOffer.offerStatus.statusName === OfferStatusName.Offered && (new Date(clubOffer.playerAdvertisement.endDate) > new Date()) && (
                                                    <>
                                                        <Button variant="success" className="button-spacing" onClick={() => handleShowAcceptReceivedClubOfferModal(clubOffer)}>
                                                            <i className="bi bi-check-lg"></i>
                                                        </Button>
                                                        <Button variant="danger" className="button-spacing" onClick={() => handleShowRejectReceivedClubOfferModal(clubOffer)}>
                                                            <i className="bi bi-x"></i>
                                                        </Button>
                                                    </>
                                                )}
                                                <span className="button-spacing">|</span>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(clubOffer.playerAdvertisement.id)}>
                                                    <i className="bi bi-info-square"></i> Ad
                                                </Button>
                                                <Button variant="info" onClick={() => handleOpenChat(clubOffer.clubMemberId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No received club offer available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>
                </Tab>

                {/* My sent offers to clubs*/}
                <Tab eventKey="sent" title="My sent offers to clubs">
                    <h3><i className="bi bi-send"></i> My sent requests to clubs</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-success">
                                <tr>
                                    <th>Sent Date</th>
                                    <th>Offer Status</th>
                                    <th>Club Name</th>
                                    <th>League (Region)</th>
                                    <th>Position</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {sentPlayerOffers.length > 0 ? (
                                    sentPlayerOffers.map((playerOffer, index) => (
                                        <tr key={index}>
                                            <td className="offer-row">{TimeService.formatDateToEUR(playerOffer.creationDate)}</td>
                                            <td className="offer-row">
                                                {playerOffer.offerStatus.statusName === "Offered" && (
                                                    <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                                )}
                                                {playerOffer.offerStatus.statusName === "Accepted" && (
                                                    <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                                )}
                                                {playerOffer.offerStatus.statusName === "Rejected" && (
                                                    <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                                )}
                                                {playerOffer.offerStatus.statusName}
                                            </td>
                                            <td className="offer-row">{playerOffer.clubAdvertisement.clubName}</td>
                                            <td className="offer-row">{playerOffer.clubAdvertisement.league} ({playerOffer.clubAdvertisement.region})</td>
                                            <td className="offer-row">{playerOffer.clubAdvertisement.playerPosition.positionName}</td>
                                            <td className="offer-row">
                                                <Button variant="primary" className="button-spacing" onClick={() => handleShowSentPlayerOfferDetails(playerOffer)}>
                                                    <i className="bi bi-info-circle"></i> Offer
                                                </Button>
                                                <span className="button-spacing">|</span>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToClubAdvertisementPage(playerOffer.clubAdvertisement.id)}>
                                                    <i className="bi bi-info-square"></i> Ad
                                                </Button>
                                                <Button variant="info" onClick={() => handleOpenChat(playerOffer.clubAdvertisement.clubMemberId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No sent player offer available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>

                    {/* Details of Received Club Offer */}
                    <Modal size="lg" show={showReceivedClubOfferDetailsModal} onHide={() => setShowReceivedClubOfferDetailsModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Offer Details</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {selectedReceivedClubOffer && (
                                <div className="modal-content-centered">
                                    <p><Form.Label className="offer-name-label">{(selectedReceivedClubOffer.clubName).toUpperCase()}</Form.Label></p>
                                    <p><Form.Label className="offer-position-label">{selectedReceivedClubOffer.playerPosition.positionName}</Form.Label></p>
                                    <Form.Label className="offer-section">OFFER INFO</Form.Label>
                                    <p><strong>Received Date</strong> {TimeService.formatDateToEUR(selectedReceivedClubOffer.creationDate)}</p>
                                    <p><strong>End Date (days left/passed)</strong> {TimeService.formatDateToEUR(selectedReceivedClubOffer.playerAdvertisement.endDate)} ({TimeService.calculateDaysLeftPassed(selectedReceivedClubOffer.playerAdvertisement.endDate)})</p>
                                    <p>
                                        <strong>Offer status: </strong>
                                        {selectedReceivedClubOffer.offerStatus.statusName === "Offered" && (
                                            <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                        )}
                                        {selectedReceivedClubOffer.offerStatus.statusName === "Accepted" && (
                                            <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                        )}
                                        {selectedReceivedClubOffer.offerStatus.statusName === "Rejected" && (
                                            <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                        )}
                                        {selectedReceivedClubOffer.offerStatus.statusName}
                                    </p>
                                    <Form.Label className="offer-section">DETAILS</Form.Label>
                                    <p><strong>Club Name:</strong> {selectedReceivedClubOffer.clubName}</p>
                                    <p><strong>League:</strong> {selectedReceivedClubOffer.league}</p>
                                    <p><strong>Region:</strong> {selectedReceivedClubOffer.region}</p>
                                    <p><strong>Position:</strong> {selectedReceivedClubOffer.playerPosition.positionName}</p>
                                    <p><strong>Salary (zł.) / month:</strong> {selectedReceivedClubOffer.salary}</p>
                                    <p><strong>Additional Information:</strong> {selectedReceivedClubOffer.additionalInformation}</p>
                                    <Form.Label className="offer-section">RECEIVED FROM</Form.Label>
                                    <p><strong>Name:</strong> {selectedReceivedClubOffer.clubMember.firstName} {selectedReceivedClubOffer.clubMember.lastName}</p>
                                    <p><strong>E-mail:</strong> {selectedReceivedClubOffer.clubMember.email}</p>
                                    <p><strong>Phone number:</strong> {selectedReceivedClubOffer.clubMember.phoneNumber}</p>
                                </div>
                            )}
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowReceivedClubOfferDetailsModal(false)}>Close</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Details of Sent Player Offer */}
                    <Modal size="lg" show={showSentPlayerOfferDetailsModal} onHide={() => setShowSentPlayerOfferDetailsModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Offer Details</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {selectedSentPlayerOffer && (
                                <div className="modal-content-centered">
                                    <Button variant="danger" onClick={() => handleShowDeleteSentPlayerOfferModal(selectedSentPlayerOffer.id)}>
                                        <i className="bi bi-trash"></i> Delete
                                    </Button>
                                    <p><Form.Label className="offer-name-label">{(selectedSentPlayerOffer.player.firstName).toUpperCase()} {(selectedSentPlayerOffer.player.lastName).toUpperCase()}</Form.Label></p>
                                    <p><Form.Label className="offer-position-label">{selectedSentPlayerOffer.clubAdvertisement.playerPosition.positionName}</Form.Label></p>
                                    <Row>
                                        <Col>
                                            <Form.Label className="offer-section">OFFER INFO</Form.Label>
                                            <p><strong>Sent Date:</strong> {TimeService.formatDateToEUR(selectedSentPlayerOffer.creationDate)}</p>
                                            <p><strong>End Date (days left/passed):</strong> {TimeService.formatDateToEUR(selectedSentPlayerOffer.clubAdvertisement.endDate)} ({TimeService.calculateDaysLeftPassed(selectedSentPlayerOffer.clubAdvertisement.endDate)})</p>
                                            <p>
                                                <strong>Offer status: </strong>
                                                {selectedSentPlayerOffer.offerStatus.statusName === "Offered" && (
                                                    <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                                )}
                                                {selectedSentPlayerOffer.offerStatus.statusName === "Accepted" && (
                                                    <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                                )}
                                                {selectedSentPlayerOffer.offerStatus.statusName === "Rejected" && (
                                                    <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                                )}
                                                {selectedSentPlayerOffer.offerStatus.statusName}
                                            </p>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <Form.Label className="offer-section">CONTACT INFO</Form.Label>
                                            <p><strong>E-mail:</strong> {selectedSentPlayerOffer.player.email}</p>
                                            <p><strong>Phone number:</strong> {selectedSentPlayerOffer.player.phoneNumber}</p>
                                            <p><strong>Location:</strong> {selectedSentPlayerOffer.player.location}</p>
                                        </Col>
                                        <Col>
                                            <Form.Label className="offer-section">PLAYER PROFILE</Form.Label>
                                            <p><strong>Age:</strong> {selectedSentPlayerOffer.age}</p>
                                            <p><strong>Height:</strong> {selectedSentPlayerOffer.height}</p>
                                            <p><strong>Foot:</strong> {selectedSentPlayerOffer.playerFoot.footName}</p>
                                        </Col>
                                        <Col>
                                            <Form.Label className="offer-section">PREFERENCES</Form.Label>
                                            <p><strong>Position:</strong> {selectedSentPlayerOffer.playerPosition.positionName}</p>
                                            <p><strong>Salary (zł.) / month:</strong> {selectedSentPlayerOffer.salary}</p>
                                            <p><strong>Additional Information:</strong> {selectedSentPlayerOffer.additionalInformation}</p>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Form.Label className="offer-section">CLUB HISTORY</Form.Label>
                                        <div className="ad-table-responsive">
                                            <Table striped bordered hover variant="light">
                                                <thead className="table-dark">
                                                    <tr>
                                                        <th>Date</th>
                                                        <th>Club</th>
                                                        <th>League (Region)</th>
                                                        <th>Position</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    {playerClubHistories.length > 0 ? (
                                                        playerClubHistories.map((history, index) => (
                                                            <tr key={index}>
                                                                <td>{TimeService.formatDateToEUR(history.startDate)} - {TimeService.formatDateToEUR(history.endDate)}</td>
                                                                <td>{history.clubName}</td>
                                                                <td>{history.league} ({history.region})</td>
                                                                <td>{history.playerPosition.positionName}</td>
                                                                <td>
                                                                    <Button variant="dark" className="button-spacing" onClick={() => handleShowClubHistoryDetails(history)}>
                                                                        <i className="bi bi-info-square"></i>
                                                                    </Button>
                                                                </td>
                                                            </tr>
                                                        ))
                                                    ) : (
                                                        <tr>
                                                            <td colSpan={5} className="text-center">No club history available</td>
                                                        </tr>
                                                    )}
                                                </tbody>
                                            </Table>
                                        </div>
                                    </Row>
                                </div>
                            )}
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowSentPlayerOfferDetailsModal(false)}>Close</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Accept Received Club Offer */}
                    <Modal show={showAcceptReceivedClubOfferModal} onHide={() => setShowAcceptReceivedClubOfferModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm action</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>Are you sure you want to accept this club offer?</Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowAcceptReceivedClubOfferModal(false)}>Cancel</Button>
                            <Button variant="success" onClick={handleAcceptReceivedClubOffer}>Accept</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Reject Received Club Offer */}
                    <Modal show={showRejectReceivedClubOfferModal} onHide={() => setShowRejectReceivedClubOfferModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm action</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>Are you sure you want to reject this club offer?</Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowRejectReceivedClubOfferModal(false)}>Cancel</Button>
                            <Button variant="danger" onClick={handleRejectReceivedClubOffer}>Reject</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Delete Sent Player Offer */}
                    <Modal show={showDeleteSentPlayerOfferModal} onHide={() => setShowDeleteSentPlayerOfferModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm action</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>Are you sure you want to delete this offer?</Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowDeleteSentPlayerOfferModal(false)}>Cancel</Button>
                            <Button variant="danger" onClick={handleDeleteSentPlayerOffer}>Delete</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Details of Club History Modal */}
                    <Modal show={showClubHistoryDetailsModal} onHide={() => setShowClubHistoryDetailsModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Club History Details</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {selectedClubHistory && (
                                <div className="modal-content-centered">
                                    <p><Form.Label className="clubHistory-name-label">{(selectedClubHistory.clubName).toUpperCase()}</Form.Label></p>
                                    <p><Form.Label className="clubHistory-position-label">{selectedClubHistory.playerPosition.positionName}</Form.Label></p>
                                    <Row>
                                        <Col>
                                            <Form.Label className="clubHistory-section">CLUB INFO</Form.Label>
                                            <p><strong>League:</strong> {selectedClubHistory.league}</p>
                                            <p><strong>Region:</strong> {selectedClubHistory.region}</p>
                                            <p><strong>Start Date:</strong> {TimeService.formatDateToEUR(selectedClubHistory.startDate)}</p>
                                            <p><strong>End Date:</strong> {TimeService.formatDateToEUR(selectedClubHistory.endDate)}</p>
                                        </Col>
                                        <Col>
                                            <Form.Label className="clubHistory-section">ACHIEVEMENTS</Form.Label>
                                            <p><strong>Matches:</strong> {selectedClubHistory.achievements.numberOfMatches}</p>
                                            <p><strong>Goals:</strong> {selectedClubHistory.achievements.goals}</p>
                                            <p><strong>Assists:</strong> {selectedClubHistory.achievements.assists}</p>
                                            <p><strong>Additional Achievements:</strong> {selectedClubHistory.achievements.additionalAchievements}</p>
                                        </Col>
                                    </Row>
                                </div>
                            )}
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowClubHistoryDetailsModal(false)}>Close</Button>
                        </Modal.Footer>
                    </Modal>
                </Tab>
            </Tabs>
        </div>
    );
}

export default MyOffersAsPlayer;
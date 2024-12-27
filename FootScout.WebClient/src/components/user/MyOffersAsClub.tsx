import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Form, Col, Row, Tabs, Tab } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import PlayerOfferService from '../../services/api/PlayerOfferService';
import ClubOfferService from '../../services/api/ClubOfferService';
import ChatService from '../../services/api/ChatService';
import OfferStatusName from '../../models/enums/OfferStatusName';
import ClubOffer from '../../models/interfaces/ClubOffer';
import PlayerOffer from '../../models/interfaces/PlayerOffer';
import ClubHistoryModel from '../../models/interfaces/ClubHistory';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import '../../App.css';
import '../../styles/user/MyOffersAsClub.css';

const MyOffersAsClub = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [sentClubOffers, setSentClubOffers] = useState<ClubOffer[]>([]);
    const [receivedPlayerOffers, setReceivedPlayerOffers] = useState<PlayerOffer[]>([]);
    const [playerClubHistories, setPlayerClubHistories] = useState<ClubHistoryModel[]>([]);
    const [showSentClubOfferDetailsModal, setShowSentClubOfferDetailsModal] = useState<boolean>(false);
    const [showReceivedPlayerOfferDetailsModal, setShowReceivedPlayerOfferDetailsModal] = useState<boolean>(false);
    const [showClubHistoryDetailsModal, setShowClubHistoryDetailsModal] = useState<boolean>(false);
    const [showAcceptReceivedPlayerOfferModal, setShowAcceptReceivedPlayerOfferModal] = useState<boolean>(false);
    const [showRejectReceivedPlayerOfferModal, setShowRejectReceivedPlayerOfferModal] = useState<boolean>(false);
    const [showDeleteSentClubOfferModal, setShowDeleteSentClubOfferModal] = useState<boolean>(false);
    const [selectedSentClubOffer, setSelectedSentClubOffer] = useState<ClubOffer | null>(null);
    const [selectedReceivedPlayerOffer, setSelectedReceivedPlayerOffer] = useState<PlayerOffer | null>(null);
    const [selectedClubHistory, setSelectedClubHistory] = useState<ClubHistoryModel | null>(null);
    const [receivedPlayerOfferToAccept, setReceivedPlayerOfferToAccept] = useState<PlayerOffer | null>(null);
    const [receivedPlayerOfferToReject, setReceivedPlayerOfferToReject] = useState<PlayerOffer | null>(null);
    const [deleteSentClubOfferId, setDeleteSentClubOfferId] = useState<number | null>(null);

    useEffect(() => {
        if (location.state && location.state.toastMessage)
            toast.success(location.state.toastMessage);

        const fetchUserData = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId) {
                    setUserId(userId);

                    const _sentClubOffers = await UserService.getSentClubOffers(userId);
                    setSentClubOffers(_sentClubOffers);

                    const _receivedPlayerOffers = await UserService.getReceivedPlayerOffers(userId);
                    setReceivedPlayerOffers(_receivedPlayerOffers);
                }
            }
            catch (error) {
                console.error('Failed to fetch user\'s data:', error);
                toast.error('Failed to load user\'s data.');
            }
        };

        fetchUserData();
    }, [location]);

    const handleShowReceivedPlayerOfferDetails = async (playerOffer: PlayerOffer) => {
        const playerClubHistories = await UserService.getUserClubHistory(playerOffer.playerId);
        setPlayerClubHistories(playerClubHistories);

        setSelectedReceivedPlayerOffer(playerOffer);
        setShowReceivedPlayerOfferDetailsModal(true);
    };

    const handleShowAcceptReceivedPlayerOfferModal = (playerOffer: PlayerOffer) => {
        setReceivedPlayerOfferToAccept(playerOffer);
        setShowAcceptReceivedPlayerOfferModal(true);
    };

    const handleAcceptReceivedPlayerOffer = async () => {
        if (!receivedPlayerOfferToAccept || !userId)
            return;

        try {
            const updatedFormData = {
                ...receivedPlayerOfferToAccept
            };

            await PlayerOfferService.acceptPlayerOffer(receivedPlayerOfferToAccept.id, updatedFormData);
            setShowAcceptReceivedPlayerOfferModal(false);
            toast.success('Received player offer has been accepted successfully.');
            // Refresh data
            const _receivedPlayerOffers = await UserService.getReceivedPlayerOffers(userId);
            setReceivedPlayerOffers(_receivedPlayerOffers);
        }
        catch (error) {
            console.error('Failed to accept received player offer:', error);
            toast.error('Failed to accept received player offer.');
        }
    };

    const handleShowRejectReceivedPlayerOfferModal = (playerOffer: PlayerOffer) => {
        setReceivedPlayerOfferToReject(playerOffer);
        setShowRejectReceivedPlayerOfferModal(true);
    };

    const handleRejectReceivedPlayerOffer = async () => {
        if (!receivedPlayerOfferToReject || !userId)
            return;

        try {
            const updatedFormData = {
                ...receivedPlayerOfferToReject
            };

            await PlayerOfferService.rejectPlayerOffer(receivedPlayerOfferToReject.id, updatedFormData);
            setShowRejectReceivedPlayerOfferModal(false);
            toast.success('Received player offer has been rejected successfully.');
            // Refresh data
            const _receivedPlayerOffers = await UserService.getReceivedPlayerOffers(userId);
            setReceivedPlayerOffers(_receivedPlayerOffers);
        }
        catch (error) {
            console.error('Failed to reject received player offer:', error);
            toast.error('Failed to reject received player offer.');
        }
    };

    const moveToClubAdvertisementPage = (clubAdvertisementId: number) => {
        navigate(`/club-advertisement/${clubAdvertisementId}`, { state: { clubAdvertisementId } });
    };

    const handleShowSentClubOfferDetails = (clubOffer: ClubOffer) => {
        setSelectedSentClubOffer(clubOffer);
        setShowSentClubOfferDetailsModal(true);
    };

    const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
        navigate(`/player-advertisement/${playerAdvertisementId}`, { state: { playerAdvertisementId } });
    };

    const handleShowClubHistoryDetails = (clubHistory: ClubHistoryModel) => {
        setSelectedClubHistory(clubHistory);
        setShowClubHistoryDetailsModal(true);
    };

    const handleShowDeleteSentClubOfferModal = (clubOfferId: number) => {
        setDeleteSentClubOfferId(clubOfferId);
        setShowDeleteSentClubOfferModal(true);
    };

    const handleDeleteSentClubOffer = async () => {
        if (!deleteSentClubOfferId)
            return;

        try {
            await ClubOfferService.deleteClubOffer(deleteSentClubOfferId);
            toast.success('Your sent club offer has been deleted successfully.');
            setShowDeleteSentClubOfferModal(false);
            setShowSentClubOfferDetailsModal(false);
            setDeleteSentClubOfferId(null);

            // Refresh the user data
            if (userId) {
                const _sentClubOffers = await UserService.getSentClubOffers(userId);
                setSentClubOffers(_sentClubOffers);

                const _receivedPlayerOffers = await UserService.getReceivedPlayerOffers(userId);
                setReceivedPlayerOffers(_receivedPlayerOffers);
            }
        }
        catch (error) {
            console.error('Failed to delete sent club offer:', error);
            toast.error('Failed to delete sent club offer.');
        }
    };

    const formatDate = (dateString: string): string => {
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}-${month}-${year}`;
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
        <div className="MyOffersAsClub">
            <h1><i className="bi bi-briefcase-fill"></i> My Offers as a Club member</h1>
            <p></p>
            {/* Received offers from players*/}
            <Tabs defaultActiveKey="received" id="problem-tabs" className="mb-3 custom-tabs">
                <Tab eventKey="received" title="Received requests from players">
                    <h3><i className="bi bi-arrow-down-left-square-fill"></i> Received requests from players</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-dark">
                                <tr>
                                    <th>Received Date</th>
                                    <th>Offer Status</th>
                                    <th>Player</th>
                                    <th>Position</th>
                                    <th>Club Name</th>
                                    <th>League (Region)</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {receivedPlayerOffers.length > 0 ? (
                                    receivedPlayerOffers.map((playerOffer, index) => (
                                        <tr key={index}>
                                            <td className="offer-row">{formatDate(playerOffer.creationDate)}</td>
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
                                            <td className="offer-row">{playerOffer.player.firstName} {playerOffer.player.lastName}</td>
                                            <td className="offer-row">{playerOffer.clubAdvertisement.playerPosition.positionName}</td>
                                            <td className="offer-row">{playerOffer.clubAdvertisement.clubName}</td>
                                            <td className="offer-row">{playerOffer.clubAdvertisement.league} ({playerOffer.clubAdvertisement.region})</td>
                                            <td className="offer-row">
                                                <Button variant="primary" className="button-spacing" onClick={() => handleShowReceivedPlayerOfferDetails(playerOffer)}>
                                                    <i className="bi bi-info-circle"></i> Offer
                                                </Button>
                                                {playerOffer.offerStatus.statusName === OfferStatusName.Offered && (new Date(playerOffer.clubAdvertisement.endDate) > new Date()) && (
                                                    <>
                                                        <Button variant="success" className="button-spacing" onClick={() => handleShowAcceptReceivedPlayerOfferModal(playerOffer)}>
                                                            <i className="bi bi-check-lg"></i>
                                                        </Button>
                                                        <Button variant="danger" className="button-spacing" onClick={() => handleShowRejectReceivedPlayerOfferModal(playerOffer)}>
                                                            <i className="bi bi-x"></i>
                                                        </Button>
                                                    </>
                                                )}
                                                <span className="button-spacing">|</span>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToClubAdvertisementPage(playerOffer.clubAdvertisement.id)}>
                                                    <i className="bi bi-info-square"></i> Ad
                                                </Button>
                                                <Button variant="info" onClick={() => handleOpenChat(playerOffer.playerId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No received player request available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>
                </Tab>

                {/* My sent offers to players*/}
                <Tab eventKey="sent" title="My sent offers to players">
                    <h3><i className="bi bi-send-fill"></i> My sent offers to players</h3>
                    <div className="table-responsive">
                        <Table striped bordered hover variant="light">
                            <thead className="table-success">
                                <tr>
                                    <th>Sent Date</th>
                                    <th>Offer Status</th>
                                    <th>Player</th>
                                    <th>Position</th>
                                    <th>Club Name</th>
                                    <th>League (Region)</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {sentClubOffers.length > 0 ? (
                                    sentClubOffers.map((clubOffer, index) => (
                                        <tr key={index}>
                                            <td className="offer-row">{formatDate(clubOffer.creationDate)}</td>
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
                                            <td className="offer-row">{clubOffer.playerAdvertisement.player.firstName} {clubOffer.playerAdvertisement.player.lastName}</td>
                                            <td className="offer-row">{clubOffer.playerPosition.positionName}</td>
                                            <td className="offer-row">{clubOffer.clubName}</td>
                                            <td className="offer-row">{clubOffer.league} ({clubOffer.region})</td>
                                            <td className="offer-row">
                                                <Button variant="primary" className="button-spacing" onClick={() => handleShowSentClubOfferDetails(clubOffer)}>
                                                    <i className="bi bi-info-circle"></i> Offer
                                                </Button>
                                                <span className="button-spacing">|</span>
                                                <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(clubOffer.playerAdvertisement.id)}>
                                                    <i className="bi bi-info-square"></i> Ad
                                                </Button>
                                                <Button variant="info" onClick={() => handleOpenChat(clubOffer.playerAdvertisement.playerId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={9} className="text-center">No sent club offer available</td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>
                    </div>

                    {/* Details of Received Player Offer */}
                    <Modal size="lg" show={showReceivedPlayerOfferDetailsModal} onHide={() => setShowReceivedPlayerOfferDetailsModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Offer Details</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {selectedReceivedPlayerOffer && (
                                <div className="modal-content-centered">
                                    <p><Form.Label className="offer-name-label">{(selectedReceivedPlayerOffer.player.firstName).toUpperCase()} {(selectedReceivedPlayerOffer.player.lastName).toUpperCase()}</Form.Label></p>
                                    <p><Form.Label className="offer-position-label">{selectedReceivedPlayerOffer.clubAdvertisement.playerPosition.positionName}</Form.Label></p>
                                    <Row>
                                        <Col>
                                            <Form.Label className="offer-section">OFFER INFO</Form.Label>
                                            <p><strong>Sent Date:</strong> {formatDate(selectedReceivedPlayerOffer.creationDate)}</p>
                                            <p><strong>End Date (days left/passed):</strong> {formatDate(selectedReceivedPlayerOffer.clubAdvertisement.endDate)} ({TimeService.calculateDaysLeftPassed(selectedReceivedPlayerOffer.clubAdvertisement.endDate)})</p>
                                            <p>
                                                <strong>Offer status: </strong>
                                                {selectedReceivedPlayerOffer.offerStatus.statusName === "Offered" && (
                                                    <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                                )}
                                                {selectedReceivedPlayerOffer.offerStatus.statusName === "Accepted" && (
                                                    <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                                )}
                                                {selectedReceivedPlayerOffer.offerStatus.statusName === "Rejected" && (
                                                    <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                                )}
                                                {selectedReceivedPlayerOffer.offerStatus.statusName}
                                            </p>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <Form.Label className="offer-section">CONTACT INFO</Form.Label>
                                            <p><strong>E-mail:</strong> {selectedReceivedPlayerOffer.player.email}</p>
                                            <p><strong>Phone number:</strong> {selectedReceivedPlayerOffer.player.phoneNumber}</p>
                                            <p><strong>Location:</strong> {selectedReceivedPlayerOffer.player.location}</p>
                                        </Col>
                                        <Col>
                                            <Form.Label className="offer-section">PLAYER PROFILE</Form.Label>
                                            <p><strong>Age:</strong> {selectedReceivedPlayerOffer.age}</p>
                                            <p><strong>Height:</strong> {selectedReceivedPlayerOffer.height}</p>
                                            <p><strong>Foot:</strong> {selectedReceivedPlayerOffer.playerFoot.footName}</p>
                                        </Col>
                                        <Col>
                                            <Form.Label className="offer-section">PREFERENCES</Form.Label>
                                            <p><strong>Position:</strong> {selectedReceivedPlayerOffer.playerPosition.positionName}</p>
                                            <p><strong>Salary (zł.) / month:</strong> {selectedReceivedPlayerOffer.salary}</p>
                                            <p><strong>Additional Information:</strong> {selectedReceivedPlayerOffer.additionalInformation}</p>
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
                                                                <td>{formatDate(history.startDate)} - {formatDate(history.endDate)}</td>
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
                            <Button variant="secondary" onClick={() => setShowReceivedPlayerOfferDetailsModal(false)}>Close</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Details of Sent Club Offer */}
                    <Modal size="lg" show={showSentClubOfferDetailsModal} onHide={() => setShowSentClubOfferDetailsModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Offer Details</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {selectedSentClubOffer && (
                                <div className="modal-content-centered">
                                    <Button variant="danger" onClick={() => handleShowDeleteSentClubOfferModal(selectedSentClubOffer.id)}>
                                        <i className="bi bi-trash"></i> Delete
                                    </Button>
                                    <p><Form.Label className="offer-name-label">{(selectedSentClubOffer.clubName).toUpperCase()}</Form.Label></p>
                                    <p><Form.Label className="offer-position-label">{selectedSentClubOffer.playerPosition.positionName}</Form.Label></p>
                                    <Form.Label className="offer-section">OFFER INFO</Form.Label>
                                    <p><strong>Sent Date</strong> {formatDate(selectedSentClubOffer.creationDate)}</p>
                                    <p><strong>End Date (days left/passed)</strong> {formatDate(selectedSentClubOffer.playerAdvertisement.endDate)} ({TimeService.calculateDaysLeftPassed(selectedSentClubOffer.playerAdvertisement.endDate)})</p>
                                    <p>
                                        <strong>Offer status: </strong>
                                        {selectedSentClubOffer.offerStatus.statusName === "Offered" && (
                                            <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                        )}
                                        {selectedSentClubOffer.offerStatus.statusName === "Accepted" && (
                                            <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                        )}
                                        {selectedSentClubOffer.offerStatus.statusName === "Rejected" && (
                                            <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                        )}
                                        {selectedSentClubOffer.offerStatus.statusName}
                                    </p>
                                    <Form.Label className="offer-section">SENT TO</Form.Label>
                                    <p><strong>Name:</strong> {selectedSentClubOffer.playerAdvertisement.player.firstName} {selectedSentClubOffer.playerAdvertisement.player.lastName}</p>
                                    <p><strong>E-mail:</strong> {selectedSentClubOffer.playerAdvertisement.player.email}</p>
                                    <p><strong>Phone number:</strong> {selectedSentClubOffer.playerAdvertisement.player.phoneNumber}</p>
                                    <Form.Label className="offer-section">DETAILS</Form.Label>
                                    <p><strong>Club Name:</strong> {selectedSentClubOffer.clubName}</p>
                                    <p><strong>League:</strong> {selectedSentClubOffer.league}</p>
                                    <p><strong>Region:</strong> {selectedSentClubOffer.region}</p>
                                    <p><strong>Position:</strong> {selectedSentClubOffer.playerPosition.positionName}</p>
                                    <p><strong>Salary (zł.) / month:</strong> {selectedSentClubOffer.salary}</p>
                                    <p><strong>Additional Information:</strong> {selectedSentClubOffer.additionalInformation}</p>
                                </div>
                            )}
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowSentClubOfferDetailsModal(false)}>Close</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Delete Sent Club Offer */}
                    <Modal show={showDeleteSentClubOfferModal} onHide={() => setShowDeleteSentClubOfferModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm action</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>Are you sure you want to delete this offer?</Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowDeleteSentClubOfferModal(false)}>Cancel</Button>
                            <Button variant="danger" onClick={handleDeleteSentClubOffer}>Delete</Button>
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
                                            <p><strong>Start Date:</strong> {formatDate(selectedClubHistory.startDate)}</p>
                                            <p><strong>End Date:</strong> {formatDate(selectedClubHistory.endDate)}</p>
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

                    {/* Accept Received Player Offer */}
                    <Modal show={showAcceptReceivedPlayerOfferModal} onHide={() => setShowAcceptReceivedPlayerOfferModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm action</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>Are you sure you want to accept this player offer?</Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowAcceptReceivedPlayerOfferModal(false)}>Cancel</Button>
                            <Button variant="success" onClick={handleAcceptReceivedPlayerOffer}>Accept</Button>
                        </Modal.Footer>
                    </Modal>

                    {/* Reject Received Player Offer */}
                    <Modal show={showRejectReceivedPlayerOfferModal} onHide={() => setShowRejectReceivedPlayerOfferModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm action</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>Are you sure you want to reject this player offer?</Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowRejectReceivedPlayerOfferModal(false)}>Cancel</Button>
                            <Button variant="danger" onClick={handleRejectReceivedPlayerOffer}>Reject</Button>
                        </Modal.Footer>
                    </Modal>
                </Tab>
            </Tabs>
        </div>
    );
}

export default MyOffersAsClub;
import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Pagination, Form, Row, Col } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerOfferService from '../../services/api/PlayerOfferService';
import ChatService from '../../services/api/ChatService';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import PlayerOfferModel from '../../models/interfaces/PlayerOffer';
import ClubHistoryModel from '../../models/interfaces/ClubHistory';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import '../../App.css';
import '../../styles/admin/AdminPlayerOffers.css';

const AdminPlayerOffers = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [playerOffers, setPlayerOffers] = useState<PlayerOfferModel[]>([]);
    const [playerClubHistories, setPlayerClubHistories] = useState<ClubHistoryModel[]>([]);
    const [showOfferDetailsModal, setShowOfferDetailsModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [showClubHistoryDetailsModal, setShowClubHistoryDetailsModal] = useState<boolean>(false);
    const [selectedOffer, setSelectedbOffer] = useState<PlayerOfferModel | null>(null);
    const [deleteOfferId, setDeleteOfferId] = useState<number | null>(null);
    const [selectedClubHistory, setSelectedClubHistory] = useState<ClubHistoryModel | null>(null);
    // Searching term
    const [searchTerm, setSearchTerm] = useState('');
    // Filtering position
    const [selectedStatus, setSelectedStatus] = useState<string | 'all'>('all');
    const [selectedPosition, setSelectedPosition] = useState<string | ''>('');
    // Sorting
    const [sortCriteria, setSortCriteria] = useState('creationDateDesc');
    // Pagination
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 20;
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const userId = await AccountService.getId();
                if (userId) {
                    setUserId(userId);
                }
            }
            catch (error) {
                console.error('Failed to fetch user\'s data:', error);
                toast.error('Failed to load user\'s data.');
            }
        };

        const fetchPositions = async () => {
            try {
                const positionsData = await PlayerPositionService.getPlayerPositions();
                setPositions(positionsData);
            }
            catch (error) {
                console.error('Failed to fetch positions:', error);
                toast.error('Failed to load positions.');
            }
        };

        const fetchClubOffers = async () => {
            try {
                const _playerOffers = await PlayerOfferService.getPlayerOffers();
                setPlayerOffers(_playerOffers);
            }
            catch (error) {
                console.error('Failed to fetch player offers:', error);
                toast.error('Failed to load player offers.');
            }
        };

        fetchUserData();
        fetchPositions();
        fetchClubOffers();
    }, [location, userId]);

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
    };

    const moveToClubAdvertisementPage = (clubAdvertisementId: number) => {
        navigate(`/club-advertisement/${clubAdvertisementId}`, { state: { clubAdvertisementId } });
    };

    const handleShowOfferDetails = async (playerOffer: PlayerOfferModel) => {
        const playerClubHistories = await UserService.getUserClubHistory(playerOffer.playerId);
        setPlayerClubHistories(playerClubHistories);
        
        setSelectedbOffer(playerOffer);
        setShowOfferDetailsModal(true);
    };

    const handleShowDeleteModal = (offerId: number) => {
        setDeleteOfferId(offerId);
        setShowDeleteModal(true);
    };

    const handleDeleteOffer = async () => {
        if (!deleteOfferId)
            return;

        try {
            await PlayerOfferService.deletePlayerOffer(deleteOfferId);
            setShowDeleteModal(false);
            toast.success("Offer has been deleted successfully.");
            //Refresh data
            const _playerOffers = await PlayerOfferService.getPlayerOffers();
            setPlayerOffers(_playerOffers);
        }
        catch (error) {
            console.error('Failed to delete offer:', error);
            toast.error('Failed to delete offer.');
        }
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

    const handleShowClubHistoryDetails = (clubHistory: ClubHistoryModel) => {
        setSelectedClubHistory(clubHistory);
        setShowClubHistoryDetailsModal(true);
    };

    const searchOffers = (offers: PlayerOfferModel[]) => {
        if (!searchTerm) {
            return offers;
        }
        const lowerCaseSearchTerm = searchTerm.toLowerCase();
        return offers.filter(offer =>
            (offer.player.firstName + ' ' + offer.player.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
            (offer.clubAdvertisement.clubMember.firstName + ' ' + offer.clubAdvertisement.clubMember.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
            offer.clubAdvertisement.clubName.toLowerCase().includes(lowerCaseSearchTerm) ||
            offer.clubAdvertisement.league.toLowerCase().includes(lowerCaseSearchTerm) ||
            offer.clubAdvertisement.region.toLowerCase().includes(lowerCaseSearchTerm)
        );
    };

    const filterOffersByStatus = (offers: PlayerOfferModel[]) => {
        if (selectedStatus === 'all') {
            return offers;
        }

        if (selectedStatus === 'active') {
            return offers.filter(o => new Date(o.clubAdvertisement.endDate).getTime() >= Date.now());
        }
        else if (selectedStatus === 'inactive') {
            return offers.filter(o => new Date(o.clubAdvertisement.endDate).getTime() < Date.now());
        }
        else {
            return offers;
        }
    };

    const filterOffersByPosition = (offers: PlayerOfferModel[]) => {
        if (!selectedPosition) {
            return offers;
        }
        return offers.filter(o => o.playerPosition.id === parseInt(selectedPosition, 10));
    };

    const sortOffers = (offers: PlayerOfferModel[]) => {
        switch (sortCriteria) {
            case 'creationDateAsc':
                return [...offers].sort((a, b) => new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime());
            case 'creationDateDesc':
                return [...offers].sort((a, b) => new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime());
            case 'positionAsc':
                return [...offers].sort((a, b) => a.playerPosition.positionName.localeCompare(b.playerPosition.positionName));
            case 'positionDesc':
                return [...offers].sort((a, b) => b.playerPosition.positionName.localeCompare(a.playerPosition.positionName));
            case 'clubNameAsc':
                return [...offers].sort((a, b) => a.clubAdvertisement.clubName.localeCompare(b.clubAdvertisement.clubName));
            case 'clubNameDesc':
                return [...offers].sort((a, b) => b.clubAdvertisement.clubName.localeCompare(a.clubAdvertisement.clubName));
            case 'leagueAsc':
                return [...offers].sort((a, b) => a.clubAdvertisement.league.localeCompare(b.clubAdvertisement.league));
            case 'leagueDesc':
                return [...offers].sort((a, b) => b.clubAdvertisement.league.localeCompare(a.clubAdvertisement.league));
            case 'regionAsc':
                return [...offers].sort((a, b) => a.clubAdvertisement.region.localeCompare(b.clubAdvertisement.region));
            case 'regionDesc':
                return [...offers].sort((a, b) => b.clubAdvertisement.region.localeCompare(a.clubAdvertisement.region));
            case 'salaryAsc':
                return [...offers].sort((a, b) => a.salary - b.salary);
            case 'salaryDesc':
                return [...offers].sort((a, b) => b.salary - a.salary);
            default:
                return offers;
        }
    };

    const searchedOffers = searchOffers(playerOffers);
    const filteredOffersByStatus = filterOffersByStatus(searchedOffers);
    const filteredOffersByPosition = filterOffersByPosition(filteredOffersByStatus);
    const sortedOffers = sortOffers(filteredOffersByPosition);
    const currentClubOfferItems = sortedOffers.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(sortedOffers.length / itemsPerPage);

    return (
        <div className="AdminPlayerOffers">
            <h1><i className="bi bi-briefcase"></i> Player Offers</h1>
            <p></p>
            <div className="d-flex align-items-center mb-3">
                {/* Search */}
                <div>
                    <Form.Label><strong>Search</strong></Form.Label>
                    <input
                        type="text"
                        className="form-control"
                        placeholder="Search"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>
                {/* Status */}
                <div className="ms-auto">
                    <Form.Label><strong>Ad Status</strong></Form.Label>
                    <Form.Select
                        as="select"
                        value={selectedStatus}
                        onChange={(e) => setSelectedStatus(e.target.value)}
                    >
                        <option value="all">All</option>
                        <option value="active">Active</option>
                        <option value="inactive">Inactive</option>
                    </Form.Select>
                </div>
                {/* Filter Positions */}
                <div className="ms-auto">
                    <Form.Label><strong>Filter Positions</strong></Form.Label>
                    <Form.Select
                        as="select"
                        value={selectedPosition}
                        onChange={(e) => setSelectedPosition(e.target.value)}
                    >
                        <option value="">All Positions</option>
                        {positions.map((position) => (
                            <option key={position.id} value={position.id}>
                                {position.positionName}
                            </option>
                        ))}
                    </Form.Select>
                </div>
                {/* Sort */}
                <div className="ms-auto">
                    <Form.Label><strong>Sort by</strong></Form.Label>
                    <Form.Select
                        aria-label="Sort by"
                        value={sortCriteria}
                        onChange={(e) => setSortCriteria(e.target.value)}
                    >
                        <option value="creationDateAsc">Creation Date Ascending</option>
                        <option value="creationDateDesc">Creation Date Descending</option>
                        <option value="positionAsc">Position Ascending</option>
                        <option value="positionDesc">Position Descending</option>
                        <option value="clubNameAsc">Club Name Ascending</option>
                        <option value="clubNameDesc">Club Name Descending</option>
                        <option value="leagueAsc">League Ascending</option>
                        <option value="leagueDesc">League Descending</option>
                        <option value="regionAsc">Region Ascending</option>
                        <option value="regionDesc">Region Descending</option>
                        <option value="salaryAsc">Salary Ascending</option>
                        <option value="salaryDesc">Salary Descending</option>
                    </Form.Select>
                </div>
            </div>
            <div className="table-responsive">
                <Table striped bordered hover variant="light">
                    <thead className="table-dark">
                        <tr>
                            <th>Creation Date</th>
                            <th>Ad Status</th>
                            <th>Offer Status</th>
                            <th>Player</th>
                            <th>Club</th>
                            <th>Club Member</th>
                            <th>Position</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {currentClubOfferItems.length > 0 ? (
                            currentClubOfferItems.map((offer, index) => (
                                <tr key={index}>
                                    <td className="offer-row">{TimeService.formatDateToEUR(offer.creationDate)}</td>
                                    <td className="offer-row">
                                        {new Date(offer.clubAdvertisement.endDate).getTime() >= Date.now() ? (
                                            <>
                                                <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i> Active
                                            </>
                                        ) : (
                                            <>
                                                <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i> Inactive
                                            </>
                                        )}
                                    </td>
                                    <td className="offer-row">
                                        {offer.offerStatus.statusName === "Offered" && (
                                            <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                        )}
                                        {offer.offerStatus.statusName === "Accepted" && (
                                            <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                        )}
                                        {offer.offerStatus.statusName === "Rejected" && (
                                            <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                        )}
                                        {offer.offerStatus.statusName}
                                    </td>
                                    <td className="offer-row">{offer.player.firstName} {offer.player.lastName}</td>
                                    <td className="offer-row">{offer.clubAdvertisement.clubName}</td>
                                    <td className="offer-row">{offer.clubAdvertisement.clubMember.firstName} {offer.clubAdvertisement.clubMember.lastName}</td>
                                    <td className="offer-row">{offer.playerPosition.positionName}</td>
                                    <td className="offer-row">
                                        <Button variant="primary" className="button-spacing" onClick={() => handleShowOfferDetails(offer)}>
                                            <i className="bi bi-info-circle"></i> Offer
                                        </Button>
                                        <Button variant="danger" className="button-spacing" onClick={() => handleShowDeleteModal(offer.id)}>
                                            <i className="bi bi-trash"></i>
                                        </Button>
                                        <span className="button-spacing">|</span>
                                        <Button variant="dark" className="button-spacing" onClick={() => moveToClubAdvertisementPage(offer.clubAdvertisementId)}>
                                            <i className="bi bi-info-square"></i> Ad
                                        </Button>
                                        {(offer.playerId !== userId) && (
                                            <>
                                                <span className="button-spacing">|</span>
                                                <Button variant="info" onClick={() => handleOpenChat(offer.playerId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={8} className="text-center">No player offfer available</td>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </div>

            {/* Pagination */}
            <div className="admin-pagination-container">
                <Pagination className="pagination-blue">
                    <Pagination.Prev
                        onClick={() => currentPage > 1 && handlePageChange(currentPage - 1)}
                        disabled={currentPage === 1}
                    />
                    {[...Array(totalPages)].map((_, index) => (
                        <Pagination.Item
                            key={index + 1}
                            active={index + 1 === currentPage}
                            onClick={() => handlePageChange(index + 1)}
                        >
                            {index + 1}
                        </Pagination.Item>
                    ))}
                    <Pagination.Next
                        onClick={() => currentPage < totalPages && handlePageChange(currentPage + 1)}
                        disabled={currentPage === totalPages}
                    />
                </Pagination>
            </div>

            {/* Details of Player Offer */}
            <Modal size="lg" show={showOfferDetailsModal} onHide={() => setShowOfferDetailsModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Offer Details</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {selectedOffer && (
                        <div className="modal-content-centered">
                            <p><Form.Label className="offer-name-label">{(selectedOffer.clubAdvertisement.clubName).toUpperCase()}</Form.Label></p>
                            <p><Form.Label className="offer-position-label">{selectedOffer.playerPosition.positionName}</Form.Label></p>
                            <Row>
                                <Col>
                                    <Form.Label className="offer-section">OFFER INFO</Form.Label>
                                    <p><strong>Offered Date</strong> {TimeService.formatDateToEUR(selectedOffer.creationDate)}</p>
                                    <p><strong>End Date (days left/passed)</strong> {TimeService.formatDateToEUR(selectedOffer.clubAdvertisement.endDate)} ({TimeService.calculateDaysLeftPassed(selectedOffer.clubAdvertisement.endDate)})</p>
                                    <p>
                                        <strong>Offer status: </strong>
                                        {selectedOffer.offerStatus.statusName === "Offered" && (
                                            <i className="bi bi-question-diamond-fill" style={{ color: '#b571ff' }}></i>
                                        )}
                                        {selectedOffer.offerStatus.statusName === "Accepted" && (
                                            <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i>
                                        )}
                                        {selectedOffer.offerStatus.statusName === "Rejected" && (
                                            <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i>
                                        )}
                                        {selectedOffer.offerStatus.statusName}
                                    </p>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Form.Label className="offer-section">CONTACT INFO</Form.Label>
                                    <p><strong>E-mail:</strong> {selectedOffer.player.email}</p>
                                    <p><strong>Phone number:</strong> {selectedOffer.player.phoneNumber}</p>
                                    <p><strong>Location:</strong> {selectedOffer.player.location}</p>
                                </Col>
                                <Col>
                                    <Form.Label className="offer-section">PLAYER PROFILE</Form.Label>
                                    <p><strong>Age:</strong> {selectedOffer.age}</p>
                                    <p><strong>Height:</strong> {selectedOffer.height}</p>
                                    <p><strong>Foot:</strong> {selectedOffer.playerFoot.footName}</p>
                                </Col>
                                <Col>
                                    <Form.Label className="offer-section">PREFERENCES</Form.Label>
                                    <p><strong>Position:</strong> {selectedOffer.playerPosition.positionName}</p>
                                    <p><strong>Salary (zł.) / month:</strong> {selectedOffer.salary}</p>
                                    <p><strong>Additional Information:</strong> {selectedOffer.additionalInformation}</p>
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
                    <Button variant="secondary" onClick={() => setShowOfferDetailsModal(false)}>Close</Button>
                </Modal.Footer>
            </Modal>

            {/* Delete Offer Modal */}
            <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this offer?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeleteOffer}>Delete</Button>
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
        </div>
    );
}

export default AdminPlayerOffers;
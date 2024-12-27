import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Pagination, Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import TimeService from '../../services/time/TimeService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import ClubOfferService from '../../services/api/ClubOfferService';
import ChatService from '../../services/api/ChatService';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import ClubOfferModel from '../../models/interfaces/ClubOffer';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import '../../App.css';
import '../../styles/admin/AdminClubOffers.css';

const AdminClubOffers = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [clubOffers, setClubOffers] = useState<ClubOfferModel[]>([]);
    const [showOfferDetailsModal, setShowOfferDetailsModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [selectedOffer, setSelectedbOffer] = useState<ClubOfferModel | null>(null);
    const [deleteOfferId, setDeleteOfferId] = useState<number | null>(null);
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
                const _clubOffers = await ClubOfferService.getClubOffers();
                setClubOffers(_clubOffers);
            }
            catch (error) {
                console.error('Failed to fetch club offers:', error);
                toast.error('Failed to load club offers.');
            }
        };

        fetchUserData();
        fetchPositions();
        fetchClubOffers();
    }, [location, userId]);

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
    };

    const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
        navigate(`/player-advertisement/${playerAdvertisementId}`, { state: { playerAdvertisementId } });
    };

    const handleShowOfferDetails = (clubOffer: ClubOfferModel) => {
        setSelectedbOffer(clubOffer);
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
            await ClubOfferService.deleteClubOffer(deleteOfferId);
            setShowDeleteModal(false);
            toast.success("Offer has been deleted successfully.");
            //Refresh data
            const _clubOffers = await ClubOfferService.getClubOffers();
            setClubOffers(_clubOffers);
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

    const searchOffers = (offers: ClubOfferModel[]) => {
        if (!searchTerm) {
            return offers;
        }
        const lowerCaseSearchTerm = searchTerm.toLowerCase();
        return offers.filter(offer =>
            (offer.clubMember.firstName + ' ' + offer.clubMember.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
            (offer.playerAdvertisement.player.firstName + ' ' + offer.playerAdvertisement.player.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
            offer.clubName.toLowerCase().includes(lowerCaseSearchTerm) ||
            offer.league.toLowerCase().includes(lowerCaseSearchTerm) ||
            offer.region.toLowerCase().includes(lowerCaseSearchTerm)
        );
    };

    const filterOffersByStatus = (offers: ClubOfferModel[]) => {
        if (selectedStatus === 'all') {
            return offers;
        }

        if (selectedStatus === 'active') {
            return offers.filter(o => new Date(o.playerAdvertisement.endDate).getTime() >= Date.now());
        }
        else if (selectedStatus === 'inactive') {
            return offers.filter(o => new Date(o.playerAdvertisement.endDate).getTime() < Date.now());
        }
        else {
            return offers;
        }
    };

    const filterOffersByPosition = (offers: ClubOfferModel[]) => {
        if (!selectedPosition) {
            return offers;
        }
        return offers.filter(o => o.playerPosition.id === parseInt(selectedPosition, 10));
    };

    const sortOffers = (offers: ClubOfferModel[]) => {
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
                return [...offers].sort((a, b) => a.clubName.localeCompare(b.clubName));
            case 'clubNameDesc':
                return [...offers].sort((a, b) => b.clubName.localeCompare(a.clubName));
            case 'leagueAsc':
                return [...offers].sort((a, b) => a.league.localeCompare(b.league));
            case 'leagueDesc':
                return [...offers].sort((a, b) => b.league.localeCompare(a.league));
            case 'regionAsc':
                return [...offers].sort((a, b) => a.region.localeCompare(b.region));
            case 'regionDesc':
                return [...offers].sort((a, b) => b.region.localeCompare(a.region));
            case 'salaryAsc':
                return [...offers].sort((a, b) => a.salary - b.salary);
            case 'salaryDesc':
                return [...offers].sort((a, b) => b.salary - a.salary);
            default:
                return offers;
        }
    };

    const searchedOffers = searchOffers(clubOffers);
    const filteredOffersByStatus = filterOffersByStatus(searchedOffers);
    const filteredOffersByPosition = filterOffersByPosition(filteredOffersByStatus);
    const sortedOffers = sortOffers(filteredOffersByPosition);
    const currentClubOfferItems = sortedOffers.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(sortedOffers.length / itemsPerPage);

    return (
        <div className="AdminClubOffers">
            <h1><i className="bi bi-briefcase-fill"></i> Club Offers</h1>
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
                            <th>Club Member</th>
                            <th>Club</th>
                            <th>Player</th>
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
                                        {new Date(offer.playerAdvertisement.endDate).getTime() >= Date.now() ? (
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
                                    <td className="offer-row">{offer.clubMember.firstName} {offer.clubMember.lastName}</td>
                                    <td className="offer-row">{offer.clubName}</td>
                                    <td className="offer-row">{offer.playerAdvertisement.player.firstName} {offer.playerAdvertisement.player.lastName}</td>
                                    <td className="offer-row">{offer.playerPosition.positionName}</td>
                                    <td className="offer-row">
                                        <Button variant="primary" className="button-spacing" onClick={() => handleShowOfferDetails(offer)}>
                                            <i className="bi bi-info-circle"></i> Offer
                                        </Button>
                                        <Button variant="danger" className="button-spacing" onClick={() => handleShowDeleteModal(offer.id)}>
                                            <i className="bi bi-trash"></i>
                                        </Button>
                                        <span className="button-spacing">|</span>
                                        <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(offer.playerAdvertisementId)}>
                                            <i className="bi bi-info-square"></i> Ad
                                        </Button>
                                        {(offer.clubMemberId !== userId) && (
                                            <>
                                                <span className="button-spacing">|</span>
                                                <Button variant="info" onClick={() => handleOpenChat(offer.clubMemberId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={8} className="text-center">No club offfer available</td>
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

            {/* Details of Club Offer */}
            <Modal size="lg" show={showOfferDetailsModal} onHide={() => setShowOfferDetailsModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Offer Details</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {selectedOffer && (
                        <div className="modal-content-centered">
                            <p><Form.Label className="offer-name-label">{(selectedOffer.clubName).toUpperCase()}</Form.Label></p>
                            <p><Form.Label className="offer-position-label">{selectedOffer.playerPosition.positionName}</Form.Label></p>
                            <Form.Label className="offer-section">OFFER INFO</Form.Label>
                            <p><strong>Offered Date</strong> {TimeService.formatDateToEUR(selectedOffer.creationDate)}</p>
                            <p><strong>End Date (days left/passed)</strong> {TimeService.formatDateToEUR(selectedOffer.playerAdvertisement.endDate)} ({TimeService.calculateDaysLeftPassed(selectedOffer.playerAdvertisement.endDate)})</p>
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
                            <Form.Label className="offer-section">DETAILS</Form.Label>
                            <p><strong>Club Name:</strong> {selectedOffer.clubName}</p>
                            <p><strong>League:</strong> {selectedOffer.league}</p>
                            <p><strong>Region:</strong> {selectedOffer.region}</p>
                            <p><strong>Position:</strong> {selectedOffer.playerPosition.positionName}</p>
                            <p><strong>Salary (zł.) / month:</strong> {selectedOffer.salary}</p>
                            <p><strong>Additional Information:</strong> {selectedOffer.additionalInformation}</p>
                            <Form.Label className="offer-section">RECEIVED FROM</Form.Label>
                            <p><strong>Name:</strong> {selectedOffer.clubMember.firstName} {selectedOffer.clubMember.lastName}</p>
                            <p><strong>E-mail:</strong> {selectedOffer.clubMember.email}</p>
                            <p><strong>Phone number:</strong> {selectedOffer.clubMember.phoneNumber}</p>
                            <Form.Label className="offer-section">SENT TO</Form.Label>
                            <p><strong>Name:</strong> {selectedOffer.playerAdvertisement.player.firstName} {selectedOffer.playerAdvertisement.player.lastName}</p>
                            <p><strong>E-mail:</strong> {selectedOffer.playerAdvertisement.player.email}</p>
                            <p><strong>Phone number:</strong> {selectedOffer.playerAdvertisement.player.phoneNumber}</p>
                            <p><strong>Location:</strong> {selectedOffer.playerAdvertisement.player.location}</p>
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
        </div>
    );
}

export default AdminClubOffers;
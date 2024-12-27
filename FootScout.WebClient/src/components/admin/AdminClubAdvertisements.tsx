import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Pagination, Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import TimeService from '../../services/time/TimeService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import ClubAdvertisementService from '../../services/api/ClubAdvertisementService';
import ChatService from '../../services/api/ChatService';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import ClubAdvertisement from '../../models/interfaces/ClubAdvertisement';
import ChatCreateDTO from '../../models/dtos/ChatCreateDTO';
import '../../App.css';
import '../../styles/admin/AdminClubAdvertisements.css';

const AdminClubAdvertisements = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [clubAdvertisements, setClubAdvertisements] = useState<ClubAdvertisement[]>([]);
    const [showFinishModal, setShowFinishModal] = useState<boolean>(false);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [finishAdvertisementId, setFinishAdvertisementId] = useState<number | null>(null);
    const [deleteAdvertisementId, setDeleteAdvertisementId] = useState<number | null>(null);
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
            } catch (error) {
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

        const fetchClubAdvertisements = async () => {
            try {
                const _clubAdvertisements = await ClubAdvertisementService.getAllClubAdvertisements();
                setClubAdvertisements(_clubAdvertisements);
            }
            catch (error) {
                console.error('Failed to fetch club advertisements:', error);
                toast.error('Failed to load club advertisements.');
            }
        };

        fetchUserData();
        fetchPositions();
        fetchClubAdvertisements();
    }, [location, userId]);

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
    };

    const moveToPlayerAdvertisementPage = (clubAdvertisementId: number) => {
        navigate(`/club-advertisement/${clubAdvertisementId}`, { state: { clubAdvertisementId } });
    };

    const handleShowFinishModal = (advertisementId: number) => {
        setFinishAdvertisementId(advertisementId);
        setShowFinishModal(true);
    };

    const handleFinishClubAdvertisement = async () => {
        if (!finishAdvertisementId)
            return;

        try {
            const clubAdvertisement = await ClubAdvertisementService.getClubAdvertisement(finishAdvertisementId);
            const currentDate = new Date().toISOString();

            const finishFormData = {
                ...clubAdvertisement,
                endDate: currentDate
            };

            await ClubAdvertisementService.updateClubAdvertisement(finishAdvertisementId, finishFormData);
            setShowFinishModal(false);
            toast.success("Advertisement has been finished successfully.");
            //Refresh data
            const _clubAdvertisements = await ClubAdvertisementService.getAllClubAdvertisements();
            setClubAdvertisements(_clubAdvertisements);
        }
        catch (error) {
            console.error('Failed to delete advertisement:', error);
            toast.error('Failed to delete advertisement.');
        }
    };

    const handleShowDeleteModal = (advertisementId: number) => {
        setDeleteAdvertisementId(advertisementId);
        setShowDeleteModal(true);
    };

    const handleDeleteAdvertisement = async () => {
        if (!deleteAdvertisementId)
            return;

        try {
            await ClubAdvertisementService.deleteClubAdvertisement(deleteAdvertisementId);
            setShowDeleteModal(false);
            toast.success("Advertisement has been deleted successfully.");
            //Refresh data
            const _clubAdvertisements = await ClubAdvertisementService.getAllClubAdvertisements();
            setClubAdvertisements(_clubAdvertisements);
        }
        catch (error) {
            console.error('Failed to delete advertisement:', error);
            toast.error('Failed to delete advertisement.');
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

    const searchAdvertisements = (advertisements: ClubAdvertisement[]) => {
        if (!searchTerm) {
            return advertisements;
        }
        const lowerCaseSearchTerm = searchTerm.toLowerCase();
        return advertisements.filter(advertisement =>
            (advertisement.clubMember.firstName + ' ' + advertisement.clubMember.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
            advertisement.league.toLowerCase().includes(lowerCaseSearchTerm) ||
            advertisement.region.toLowerCase().includes(lowerCaseSearchTerm)
        );
    };

    const filterAdvertisementsByStatus = (advertisements: ClubAdvertisement[]) => {
        if (selectedStatus === 'all') {
            return advertisements;
        }

        if (selectedStatus === 'active') {
            return advertisements.filter(ad => new Date(ad.endDate).getTime() >= Date.now());
        }
        else if (selectedStatus === 'inactive') {
            return advertisements.filter(ad => new Date(ad.endDate).getTime() < Date.now());
        }
        else {
            return advertisements;
        }
    };

    const filterAdvertisementsByPosition = (advertisements: ClubAdvertisement[]) => {
        if (!selectedPosition) {
            return advertisements;
        }
        return advertisements.filter(ad => ad.playerPosition.id === parseInt(selectedPosition, 10));
    };

    const sortAdvertisements = (advertisements: ClubAdvertisement[]) => {
        switch (sortCriteria) {
            case 'creationDateAsc':
                return [...advertisements].sort((a, b) => new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime());
            case 'creationDateDesc':
                return [...advertisements].sort((a, b) => new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime());
            case 'positionAsc':
                return [...advertisements].sort((a, b) => a.playerPosition.positionName.localeCompare(b.playerPosition.positionName));
            case 'positionDesc':
                return [...advertisements].sort((a, b) => b.playerPosition.positionName.localeCompare(a.playerPosition.positionName));
            case 'clubNameAsc':
                return [...advertisements].sort((a, b) => a.clubName.localeCompare(b.clubName));
            case 'clubNameDesc':
                return [...advertisements].sort((a, b) => b.clubName.localeCompare(a.clubName));
            case 'leagueAsc':
                return [...advertisements].sort((a, b) => a.league.localeCompare(b.league));
            case 'leagueDesc':
                return [...advertisements].sort((a, b) => b.league.localeCompare(a.league));
            case 'regionAsc':
                return [...advertisements].sort((a, b) => a.region.localeCompare(b.region));
            case 'regionDesc':
                return [...advertisements].sort((a, b) => b.region.localeCompare(a.region));
            case 'salaryAsc':
                return [...advertisements].sort((a, b) => a.salaryRange.min - b.salaryRange.min);
            case 'salaryDesc':
                return [...advertisements].sort((a, b) => b.salaryRange.min - a.salaryRange.min);
            default:
                return advertisements;
        }
    };

    const searchedAdvertisements = searchAdvertisements(clubAdvertisements);
    const filteredAdvertisementsByStatus = filterAdvertisementsByStatus(searchedAdvertisements);
    const filteredAdvertisementsByPosition = filterAdvertisementsByPosition(filteredAdvertisementsByStatus);
    const sortedAdvertisements = sortAdvertisements(filteredAdvertisementsByPosition);
    const currentClubAdvertisementItems = sortedAdvertisements.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(sortedAdvertisements.length / itemsPerPage);

    return (
        <div className="AdminClubAdvertisements">
            <h1><i className="bi bi-list-nested"></i> Club Advertisements</h1>
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
                    <Form.Label><strong>Status</strong></Form.Label>
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
                            <th>Status</th>
                            <th>Position</th>
                            <th>Club Name</th>
                            <th>League</th>
                            <th>Region</th>
                            <th>Salary (zł.) / month</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {currentClubAdvertisementItems.length > 0 ? (
                            currentClubAdvertisementItems.map((advertisement, index) => (
                                <tr key={index}>
                                    <td className="ad-row">{TimeService.formatDateToEUR(advertisement.creationDate)}</td>
                                    <td className="ad-row">
                                        {new Date(advertisement.endDate).getTime() >= Date.now() ? (
                                            <>
                                                <i className="bi bi-check-circle-fill" style={{ color: 'green' }}></i> Active
                                            </>
                                        ) : (
                                            <>
                                                <i className="bi bi-x-circle-fill" style={{ color: 'red' }}></i> Inactive
                                            </>
                                        )}
                                    </td>
                                    <td className="ad-row">{advertisement.playerPosition.positionName}</td>
                                    <td className="ad-row">{advertisement.clubName}</td>
                                    <td className="ad-row">{advertisement.league}</td>
                                    <td className="ad-row">{advertisement.region}</td>
                                    <td className="ad-row">{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                    <td className="ad-row">
                                        <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(advertisement.id)}>
                                            <i className="bi bi-info-square"></i>
                                        </Button>
                                        <Button variant="secondary" className="button-spacing" onClick={() => handleShowFinishModal(advertisement.id)}>
                                            <i className="bi bi-calendar-x"></i>
                                        </Button>
                                        <Button variant="danger" className="button-spacing" onClick={() => handleShowDeleteModal(advertisement.id)}>
                                            <i className="bi bi-trash"></i>
                                        </Button>
                                        {(advertisement.clubMemberId !== userId) && (
                                            <>
                                                <span className="button-spacing">|</span>
                                                <Button variant="info" onClick={() => handleOpenChat(advertisement.clubMemberId)}>
                                                    <i className="bi bi-chat-fill"></i>
                                                </Button>
                                            </>
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={8} className="text-center">No club advertisement available</td>
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

            {/* Finish Club Advertisement */}
            <Modal show={showFinishModal} onHide={() => setShowFinishModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to finish this advertisement?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowFinishModal(false)}>Cancel</Button>
                    <Button variant="dark" onClick={handleFinishClubAdvertisement}>Finish</Button>
                </Modal.Footer>
            </Modal>

            {/* Delete Advertisement Modal */}
            <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this advertisement?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeleteAdvertisement}>Delete</Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
}

export default AdminClubAdvertisements;
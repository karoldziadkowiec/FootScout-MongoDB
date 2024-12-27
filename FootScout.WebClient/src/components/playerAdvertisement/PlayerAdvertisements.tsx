import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Pagination, Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import PlayerAdvertisementService from '../../services/api/PlayerAdvertisementService';
import FavoritePlayerAdvertisementService from '../../services/api/FavoritePlayerAdvertisementService';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import PlayerAdvertisement from '../../models/interfaces/PlayerAdvertisement';
import FavoritePlayerAdvertisement from '../../models/interfaces/FavoritePlayerAdvertisement';
import FavoritePlayerAdvertisementCreateDTO from '../../models/dtos/FavoritePlayerAdvertisementCreateDTO';
import '../../App.css';
import '../../styles/playerAdvertisement/PlayerAdvertisements.css';

const PlayerAdvertisements = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [isAdminRole, setIsAdminRole] = useState<boolean | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [playerAdvertisements, setPlayerAdvertisements] = useState<PlayerAdvertisement[]>([]);
    const [favoritePlayerAdvertisements, setFavoritePlayerAdvertisements] = useState<FavoritePlayerAdvertisement[]>([]);
    const [favoritePlayerAdvertisementIds, setFavoritePlayerAdvertisementIds] = useState<number[]>([]);
    const [showDeleteFavoriteModal, setShowDeleteFavoriteModal] = useState<boolean>(false);
    const [deleteFavoriteId, setDeleteFavoriteId] = useState<number | null>(null);
    const [favoritePlayerAdvertisementDTO, setFavoritePlayerAdvertisementDTO] = useState<FavoritePlayerAdvertisementCreateDTO>({
        playerAdvertisementId: 0,
        userId: ''
    });
    // Searching term
    const [searchTerm, setSearchTerm] = useState('');
    // Filtering position
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

                    const isAdmin = await AccountService.isRoleAdmin();
                    setIsAdminRole(isAdmin);
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

        const fetchPlayerAdvertisements = async () => {
            try {
                const _playerAdvertisements = await PlayerAdvertisementService.getActivePlayerAdvertisements();
                setPlayerAdvertisements(_playerAdvertisements);
            } catch (error) {
                console.error('Failed to fetch player advertisements:', error);
                toast.error('Failed to load player advertisements.');
            }
        };

        const fetchFavoritePlayerAdvertisements = async () => {
            try {
                if (userId) {
                    const _favPlayerAdvertisements = await UserService.getUserActivePlayerAdvertisementFavorites(userId);
                    setFavoritePlayerAdvertisements(_favPlayerAdvertisements);

                    const favoriteIds = _favPlayerAdvertisements.map(fav => fav.playerAdvertisementId);
                    setFavoritePlayerAdvertisementIds(favoriteIds);
                }
            } catch (error) {
                console.error('Failed to fetch favorite player advertisements:', error);
                toast.error('Failed to load favorite player advertisements.');
            }
        };

        if (userId) {
            fetchFavoritePlayerAdvertisements();
        }

        fetchUserData();
        fetchPositions();
        fetchPlayerAdvertisements();
    }, [location, userId]);

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
    };

    const moveToPlayerAdvertisementPage = (playerAdvertisementId: number) => {
        navigate(`/player-advertisement/${playerAdvertisementId}`, { state: { playerAdvertisementId } });
    };

    const handleShowDeleteFavoriteModal = (favoriteAdvertisementId: number) => {
        setDeleteFavoriteId(favoriteAdvertisementId);
        setShowDeleteFavoriteModal(true);
    };

    const handleDeleteFromFavorites = async () => {
        if (!userId || !deleteFavoriteId)
            return;

        try {
            await FavoritePlayerAdvertisementService.deleteFromFavorites(deleteFavoriteId);
            toast.success('Your followed advertisement has been deleted from favorites successfully.');
            setShowDeleteFavoriteModal(false);
            setDeleteFavoriteId(null);
            // Refresh the user data
            const _playerAdvertisements = await PlayerAdvertisementService.getActivePlayerAdvertisements();
            setPlayerAdvertisements(_playerAdvertisements);

            const _favPlayerAdvertisements = await UserService.getUserActivePlayerAdvertisementFavorites(userId);
            setFavoritePlayerAdvertisements(_favPlayerAdvertisements);
            const favoriteIds = _favPlayerAdvertisements.map(fav => fav.playerAdvertisementId);
            setFavoritePlayerAdvertisementIds(favoriteIds);
        }
        catch (error) {
            console.error('Failed to delete advertisement from favorites:', error);
            toast.error('Failed to delete advertisement from favorites.');
        }
    };

    const handleAddToFavorite = async (playerAdvertisement: PlayerAdvertisement) => {
        if (!playerAdvertisement || !userId)
            return;

        try {
            const createFormData = { ...favoritePlayerAdvertisementDTO, playerAdvertisementId: playerAdvertisement.id, userId: userId };
            setFavoritePlayerAdvertisementDTO(createFormData);

            await FavoritePlayerAdvertisementService.addToFavorites(createFormData);
            toast.success('Player advertisement has been added to favorites successfully.');

            // Refresh the user data
            const _playerAdvertisements = await PlayerAdvertisementService.getActivePlayerAdvertisements();
            setPlayerAdvertisements(_playerAdvertisements);

            const _favPlayerAdvertisements = await UserService.getUserActivePlayerAdvertisementFavorites(userId);
            setFavoritePlayerAdvertisements(_favPlayerAdvertisements);
            const favoriteIds = _favPlayerAdvertisements.map(fav => fav.playerAdvertisementId);
            setFavoritePlayerAdvertisementIds(favoriteIds);
        }
        catch (error) {
            console.error('Failed to add advertisement to favorites:', error);
            toast.error('Failed to add advertisement to favorites.');
        }
    };

    const searchAdvertisements = (advertisements: PlayerAdvertisement[]) => {
        if (!searchTerm) {
            return advertisements;
        }
        const lowerCaseSearchTerm = searchTerm.toLowerCase();
        return advertisements.filter(advertisement =>
            (advertisement.player.firstName + ' ' + advertisement.player.lastName).toLowerCase().includes(lowerCaseSearchTerm) ||
            advertisement.league.toLowerCase().includes(lowerCaseSearchTerm) ||
            advertisement.region.toLowerCase().includes(lowerCaseSearchTerm)
        );
    };

    const filterAdvertisementsByPosition = (advertisements: PlayerAdvertisement[]) => {
        if (!selectedPosition) {
            return advertisements;
        }
        return advertisements.filter(ad => ad.playerPosition.id === parseInt(selectedPosition, 10));
    };

    const sortAdvertisements = (advertisements: PlayerAdvertisement[]) => {
        switch (sortCriteria) {
            case 'creationDateAsc':
                return [...advertisements].sort((a, b) => new Date(a.creationDate).getTime() - new Date(b.creationDate).getTime());
            case 'creationDateDesc':
                return [...advertisements].sort((a, b) => new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime());
            case 'positionAsc':
                return [...advertisements].sort((a, b) => a.playerPosition.positionName.localeCompare(b.playerPosition.positionName));
            case 'positionDesc':
                return [...advertisements].sort((a, b) => b.playerPosition.positionName.localeCompare(a.playerPosition.positionName));
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

    const searchedAdvertisements = searchAdvertisements(playerAdvertisements);
    const filteredAdvertisements = filterAdvertisementsByPosition(searchedAdvertisements);
    const sortedAdvertisements = sortAdvertisements(filteredAdvertisements);
    const currentPlayerAdvertisementItems = sortedAdvertisements.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(sortedAdvertisements.length / itemsPerPage);

    return (
        <div className="PlayerAdvertisements">
            <h1><i className="bi bi-list-nested"></i> Player Advertisements</h1>
            <Button variant="success" className="form-button" onClick={() => navigate('/new-player-advertisement')}>
                <i className="bi bi-file-earmark-plus-fill"></i>
                New Advertisement
            </Button>
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
                            <th>Player</th>
                            <th>Position</th>
                            <th>Preferred League</th>
                            <th>Region</th>
                            <th>Salary (zł.) / month</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {currentPlayerAdvertisementItems.length > 0 ? (
                            currentPlayerAdvertisementItems.map((advertisement, index) => (
                                <tr key={index}>
                                    <td className="ad-row">{TimeService.formatDateToEUR(advertisement.creationDate)}</td>
                                    <td className="ad-row">{advertisement.player.firstName} {advertisement.player.lastName}</td>
                                    <td className="ad-row">{advertisement.playerPosition.positionName}</td>
                                    <td className="ad-row">{advertisement.league}</td>
                                    <td className="ad-row">{advertisement.region}</td>
                                    <td className="ad-row">{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                    <td className="ad-row">
                                        <Button variant="dark" className="button-spacing" onClick={() => moveToPlayerAdvertisementPage(advertisement.id)}>
                                            <i className="bi bi-info-square"></i>
                                        </Button>
                                        {(advertisement.playerId !== userId && !isAdminRole) && (
                                            favoritePlayerAdvertisementIds.includes(advertisement.id) ? (
                                                <Button
                                                    variant="danger"
                                                    onClick={() => {
                                                        const favorite = favoritePlayerAdvertisements.find(fav => fav.playerAdvertisementId === advertisement.id);
                                                        if (favorite) {
                                                            handleShowDeleteFavoriteModal(favorite.id);
                                                        }
                                                    }}>
                                                    <i className="bi bi-heart-fill"></i>
                                                </Button>
                                            ) : (
                                                <Button variant="success" onClick={() => handleAddToFavorite(advertisement)}>
                                                    <i className="bi bi-heart"></i>
                                                </Button>
                                            )
                                        )}
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan={7} className="text-center">No player advertisement available</td>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </div>

            {/* Pagination */}
            <div className="pagination-container">
                <Pagination className="pagination-green">
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

            {/* Delete Favorite Player Advertisement Modal */}
            <Modal show={showDeleteFavoriteModal} onHide={() => setShowDeleteFavoriteModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirm action</Modal.Title>
                </Modal.Header>
                <Modal.Body>Are you sure you want to delete this advertisement from favorites?</Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowDeleteFavoriteModal(false)}>Cancel</Button>
                    <Button variant="danger" onClick={handleDeleteFromFavorites}>Delete</Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
}

export default PlayerAdvertisements;
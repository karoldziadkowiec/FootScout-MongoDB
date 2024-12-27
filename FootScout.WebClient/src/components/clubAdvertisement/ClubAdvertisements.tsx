import React, { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Table, Button, Modal, Pagination, Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import AccountService from '../../services/api/AccountService';
import UserService from '../../services/api/UserService';
import TimeService from '../../services/time/TimeService';
import PlayerPositionService from '../../services/api/PlayerPositionService';
import ClubAdvertisementService from '../../services/api/ClubAdvertisementService';
import FavoriteClubAdvertisementService from '../../services/api/FavoriteClubAdvertisementService';
import PlayerPosition from '../../models/interfaces/PlayerPosition';
import ClubAdvertisement from '../../models/interfaces/ClubAdvertisement';
import FavoriteClubAdvertisement from '../../models/interfaces/FavoriteClubAdvertisement';
import FavoriteClubAdvertisementCreateDTO from '../../models/dtos/FavoriteClubAdvertisementCreateDTO';
import '../../App.css';
import '../../styles/clubAdvertisement/ClubAdvertisements.css';

const ClubAdvertisements = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [userId, setUserId] = useState<string | null>(null);
    const [isAdminRole, setIsAdminRole] = useState<boolean | null>(null);
    const [positions, setPositions] = useState<PlayerPosition[]>([]);
    const [clubAdvertisements, setClubAdvertisements] = useState<ClubAdvertisement[]>([]);
    const [favoriteClubAdvertisements, setFavoriteClubAdvertisements] = useState<FavoriteClubAdvertisement[]>([]);
    const [favoriteClubAdvertisementIds, setFavoriteClubAdvertisementIds] = useState<number[]>([]);
    const [showDeleteFavoriteModal, setShowDeleteFavoriteModal] = useState<boolean>(false);
    const [deleteFavoriteId, setDeleteFavoriteId] = useState<number | null>(null);
    const [favoriteClubAdvertisementDTO, setFavoriteClubAdvertisementDTO] = useState<FavoriteClubAdvertisementCreateDTO>({
        clubAdvertisementId: 0,
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

        const fetchClubAdvertisements = async () => {
            try {
                const _clubAdvertisements = await ClubAdvertisementService.getActiveClubAdvertisements();
                setClubAdvertisements(_clubAdvertisements);
            } catch (error) {
                console.error('Failed to fetch club advertisements:', error);
                toast.error('Failed to load club advertisements.');
            }
        };

        const fetchFavoriteClubAdvertisements = async () => {
            try {
                if (userId) {
                    const _favClubAdvertisements = await UserService.getUserActiveClubAdvertisementFavorites(userId);
                    setFavoriteClubAdvertisements(_favClubAdvertisements);

                    const favoriteIds = _favClubAdvertisements.map(fav => fav.clubAdvertisementId);
                    setFavoriteClubAdvertisementIds(favoriteIds);
                }
            } catch (error) {
                console.error('Failed to fetch favorite club advertisements:', error);
                toast.error('Failed to load favorite club advertisements.');
            }
        };

        if (userId) {
            fetchFavoriteClubAdvertisements();
        }

        fetchUserData();
        fetchPositions();
        fetchClubAdvertisements();
    }, [location, userId]);

    const handlePageChange = (pageNumber: number) => {
        setCurrentPage(pageNumber);
    };

    const moveToClubAdvertisementPage = (clubAdvertisementId: number) => {
        navigate(`/club-advertisement/${clubAdvertisementId}`, { state: { clubAdvertisementId } });
    };

    const handleShowDeleteFavoriteModal = (favoriteAdvertisementId: number) => {
        setDeleteFavoriteId(favoriteAdvertisementId);
        setShowDeleteFavoriteModal(true);
    };

    const handleDeleteFromFavorites = async () => {
        if (!userId || !deleteFavoriteId)
            return;

        try {
            await FavoriteClubAdvertisementService.deleteFromFavorites(deleteFavoriteId);
            toast.success('Your followed advertisement has been deleted from favorites successfully.');
            setShowDeleteFavoriteModal(false);
            setDeleteFavoriteId(null);
            // Refresh the user data
            const _clubAdvertisements = await ClubAdvertisementService.getActiveClubAdvertisements();
            setClubAdvertisements(_clubAdvertisements);

            const _favClubAdvertisements = await UserService.getUserActiveClubAdvertisementFavorites(userId);
            setFavoriteClubAdvertisements(_favClubAdvertisements);
            const favoriteIds = _favClubAdvertisements.map(fav => fav.clubAdvertisementId);
            setFavoriteClubAdvertisementIds(favoriteIds);
        }
        catch (error) {
            console.error('Failed to delete advertisement from favorites:', error);
            toast.error('Failed to delete advertisement from favorites.');
        }
    };

    const handleAddToFavorite = async (clubAdvertisement: ClubAdvertisement) => {
        if (!clubAdvertisement || !userId)
            return;

        try {
            const createFormData = { ...favoriteClubAdvertisementDTO, clubAdvertisementId: clubAdvertisement.id, userId: userId };
            setFavoriteClubAdvertisementDTO(createFormData);

            await FavoriteClubAdvertisementService.addToFavorites(createFormData);
            toast.success('Club advertisement has been added to favorites successfully.');

            // Refresh the user data
            const _clubAdvertisements = await ClubAdvertisementService.getActiveClubAdvertisements();
            setClubAdvertisements(_clubAdvertisements);

            const _favClubAdvertisements = await UserService.getUserActiveClubAdvertisementFavorites(userId);
            setFavoriteClubAdvertisements(_favClubAdvertisements);
            const favoriteIds = _favClubAdvertisements.map(fav => fav.clubAdvertisementId);
            setFavoriteClubAdvertisementIds(favoriteIds);
        }
        catch (error) {
            console.error('Failed to add advertisement to favorites:', error);
            toast.error('Failed to add advertisement to favorites.');
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
    const filteredAdvertisements = filterAdvertisementsByPosition(searchedAdvertisements);
    const sortedAdvertisements = sortAdvertisements(filteredAdvertisements);
    const currentClubAdvertisementItems = sortedAdvertisements.slice(indexOfFirstItem, indexOfLastItem);
    const totalPages = Math.ceil(sortedAdvertisements.length / itemsPerPage);

    return (
        <div className="ClubAdvertisements">
            <h1><i className="bi bi-list-nested"></i> Club Advertisements</h1>
            <Button variant="success" className="form-button" onClick={() => navigate('/new-club-advertisement')}>
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
                                    <td className="ad-row">{advertisement.playerPosition.positionName}</td>
                                    <td className="ad-row">{advertisement.clubName}</td>
                                    <td className="ad-row">{advertisement.league}</td>
                                    <td className="ad-row">{advertisement.region}</td>
                                    <td className="ad-row">{advertisement.salaryRange.min} - {advertisement.salaryRange.max}</td>
                                    <td className="ad-row">
                                        <Button variant="dark" className="button-spacing" onClick={() => moveToClubAdvertisementPage(advertisement.id)}>
                                            <i className="bi bi-info-square"></i>
                                        </Button>
                                        {(advertisement.clubMemberId !== userId && !isAdminRole) && (
                                            favoriteClubAdvertisementIds.includes(advertisement.id) ? (
                                                <Button
                                                    variant="danger"
                                                    onClick={() => {
                                                        const favorite = favoriteClubAdvertisements.find(fav => fav.clubAdvertisementId === advertisement.id);
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
                                <td colSpan={7} className="text-center">No club advertisement available</td>
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

            {/* Delete Favorite Club Advertisement Modal */}
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

export default ClubAdvertisements;